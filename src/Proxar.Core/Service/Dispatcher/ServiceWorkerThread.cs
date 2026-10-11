/*
 * Copyright 2026 Proxar
 * SPDX-License-Identifier: Apache-2.0
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 *     http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */


using Proxar.Logging;
namespace Proxar.ServiceCore.Dispatch;

internal class ThreadDispatchMode
{
    public const int ConsumeAll = 1;
    public const int ConsumeHalf = 2;
    public const int ConsumeQuarter = 3;
    public const int ConsumeTwo = 4;
}

internal static class ThreadWorkerData
{
    // [FIX-A] 共享信号量已废弃：改为生产者按 {就绪服务数, 醒着线程数, 睡着线程数} 定向唤醒具体线程。
    // 保留空类型以免影响其它引用（如无引用可整体删除）。
}


internal class ServiceWorkerThread
{

#if DEBUG
    internal StatRecord? StatRecord = new StatRecord();
#else
    internal StatRecord? StatRecord = null;
#endif




    private ServiceBase? Service = null;
    public readonly int DispatchMode = 1;
    public readonly int InitFactor = 100;
    public int Factor = 100;
    public readonly int DispatchCount = 0;

    private int missServiceCnt = 0;
    private int singleHandleCnt = 0;
    private Func<ServiceBase?> ServiceDispatcher;

    // [STATS] 诊断计数器：只由本 worker 线程写，其他线程只读，无需原子操作
    internal long StatPark;
    internal long StatSpurious;
    internal long StatLoop;
    internal long StatPopHit;
    internal long StatPopMiss;
    internal long StatTakeService;
    internal long StatMsg;
    internal long StatDrop;
    internal long StatEmpty;
    internal long StatSpinHit;   // [FIX-A] 自旋期间直接接到活（零系统调用）
    internal long StatSpinMiss;  // [FIX-A] 自旋没接到活

    // [FIX-A] 每个 worker 自己的门铃 + 睡觉登记标志（生产者定向唤醒用）
    internal readonly AutoResetEvent ParkGate = new(false);
    internal int ParkedFlag;
    internal bool Hot;           // 上一轮干过活 → 睡觉前先自旋一小会儿

    internal Thread Thread { get; set; }
    internal CancellationTokenSource Cts = new();

    private ServiceManager serviceManagerCache;


    public bool ConsumeContinueDispatch = true;
    internal readonly SemaphoreSlim Semaphore = new(0);

    public ServiceWorkerThread(int factor, int dispatchCount)
    {
        this.ServiceDispatcher = this.DispatchWhereContineu;
        this.InitFactor = factor;
        this.Factor = factor;
        this.DispatchCount = dispatchCount;
        serviceManagerCache = ServiceManager.Instance;
        Thread = new Thread(ThreadDispatchService)
        {
            IsBackground = true,
            Name = "Frame Worker"
        };
        WorkerSignal.Register(this); // [FIX-A] 先登记再启动，保证生产者能看到本线程
        Thread.Start();
    }


    protected ServiceBase? PopService()
    {
        var serviceId = serviceManagerCache.DequeueGlobalQueue();
        if (serviceId == 0)
        {
            return null;
        }

        return serviceManagerCache.GetService(serviceId);
    }


    /// <summary>
    /// 消息调度
    /// 1. 若无正在处理服务，从全局队列取出
    /// 2. 从服务的消息队列，取出消息。
    ///     1）有消息，正常消费
    ///     2）无消息，设置InGlobalQueue标记为false。自旋锁确保设置内存屏障
    /// 3. 若服务无需继续调度（无消息），则解除线程对服务的引用。
    /// 4. 从全局队列取出一个新的服务，若成功，则替换当前服务，下次将消费这个服务的消息
    ///     从全局队列取出服务，使用线程安全队列，若成功，则会设置内存屏障
    /// 5. 若服务仍需继续调度，且第四步骤成功取到新服务，则将当前服务放回全局队列
    ///     将服务放回全局队列，会设置内存屏障
    /// </summary>
    /// <returns></returns>
    protected ThreadDispatchType DispatchService()
    {
        this.StatRecord?.AddStatLoop(); // [STATS]
        if (this.Service == null)
        {
            this.Service = this.PopService();
            if (this.Service == null)
            {
                this.StatRecord?.AddStatPopMiss();
            }
            else
            {
                this.StatRecord?.AddStatPopHit(); // [STATS]
            }
        }
        if (this.Service == null)
        {
            if (missServiceCnt > ThreadDispatchConfig.MaxMissServiceCnt)
            {
                return ThreadDispatchType.Sleep;
            }
            missServiceCnt++;
            return ThreadDispatchType.SpinWait;
        }
        missServiceCnt = 0;
        this.StatRecord?.AddStatTakeService(); // [STATS]

        var needContinueDispatch = false;
        try
        {
            var succ = this.Service.ThreadEnter(this);
            if (succ)
            {
                needContinueDispatch = this.Service.ThreadConsumeMessage();
            }
        }
        catch (Exception e)
        {
            needContinueDispatch = true;
            ProxarLogger.Error(e);
        }
        finally
        {
            ActorThreadScope.UnbindActor();
            var serviceId = this.Service.GetServiceId();
            ServiceBase? popService = null;
            if (!needContinueDispatch)
            {
                this.StatRecord?.AddStatDrop(); // [STATS]
                this.StatRecord?.AddStatEmpty(); // [STATS] 队列空导致本轮没有可继续派发的消息
                // [FIX-B] 原来这里会再 PopService() 预取一个：与下一轮 DispatchService 的取法完全重复，
                // 而且会抢走"刚刚被 OnServiceReady 唤醒的线程"要取的那一项（白醒来源）。去掉，交给下一轮。
                this.Service = null;
                singleHandleCnt = 0;
            }
            else
            {
                popService = this.ServiceDispatcher.Invoke();
            }
            if (popService != null)
            {
                this.Service = popService;
            }
            if (needContinueDispatch && popService != null)
            {
                serviceManagerCache.PushGlobalQueue(serviceId);
                WorkerSignal.OnServiceReady(); // [FIX-A] 手上有活但活比人多的，补一个 worker
            }
        }
        return ThreadDispatchType.None;
    }

    private ServiceBase? DispatchWhereContineu()
    {
        singleHandleCnt++;
        if (singleHandleCnt <= 100)
        {
            return this.PopService();
        }
        // 多次调用同一个
        singleHandleCnt = 0;

        this.Factor = 100;
        this.ServiceDispatcher = this.DispatchWhereContineuNext;
        return null;
    }

    private ServiceBase? DispatchWhereContineuNext()
    {
        // 多次调用同一个
        singleHandleCnt = 0;

        this.Factor = this.InitFactor;
        this.ServiceDispatcher = this.DispatchWhereContineu;
        return this.PopService();
    }

    // [FIX-A] 自旋取活：只在就绪队列非空时才去抢，避免多个线程反复 CAS 同一条队列头。
    // 预算约 5µs，只在"刚干过活"之后用一次；冷下来的线程不会自旋，直接睡。
    private const int SpinRounds = 8;
    private const int SpinPause = 32;

    private bool SpinTake()
    {
        for (int i = 0; i < SpinRounds; i++)
        {
            if (WorkerSignal.ReadyDepth > 0)
            {
                var svc = this.PopService();
                if (svc != null)
                {
                    this.Service = svc;
                    return true;
                }
            }
            Thread.SpinWait(SpinPause);
        }
        return false;
    }

    private void ThreadDispatchService(Object? stateInfo)
    {
        WorkerSignal.OnWorkerStarted(); // [FIX-A]
        bool parked = false;
        while (!Cts.Token.IsCancellationRequested)
        {
            var ret = this.DispatchService();
            // [STATS] 上一轮 park 之后本次仍然没拿到服务 → 这是一次"白醒"的虚假唤醒
            if (parked && ret != ThreadDispatchType.None)
            {
                this.StatRecord?.AddStatSpurious();
            }
            parked = false;
            switch (ret)
            {
                case ThreadDispatchType.None:
                    break;
                case ThreadDispatchType.SpinWait:
                case ThreadDispatchType.Sleep:
                    // [FIX-A] 睡之前三段式：
                    // 1) 刚干过活（热）→ 先自旋一小会儿，把随后到达的入队直接接住。
                    //    自旋期间本线程仍算 awake，生产侧 need 会把它算进去，因此不会多叫人，
                    //    这一段交接是零系统调用的。
                    // 2) 登记睡觉，再复查就绪队列（与生产侧构成防丢唤醒的先后顺序）。
                    // 3) 真没活才睡在自己的门铃上，由生产者定向唤醒（没有共享信号量、没有心跳）。
                    if (this.Hot)
                    {
                        this.Hot = false;
                        if (this.SpinTake())
                        {
                            this.StatRecord?.AddStatSpinHit(); // [STATS]
                            continue;
                        }
                        this.StatRecord?.AddStatSpinMiss(); // [STATS]
                    }
                    if (WorkerSignal.Park(this))
                    {
                        WorkerSignal.Unpark(this);
                        continue;
                    }
                    this.StatRecord?.AddStatPark();
                    this.ParkGate.WaitOne();
                    WorkerSignal.Unpark(this);
                    parked = true;
                    break;
                default:
                    break;
            }

        }
    }
}