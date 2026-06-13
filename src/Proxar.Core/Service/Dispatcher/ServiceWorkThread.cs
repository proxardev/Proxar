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

internal class ServiceWorkThread
{
    private ServiceBase? Service = null;
    public readonly int DispatchMode = 1;
    public readonly int Factor = 100;
    public readonly int DispatchCount = 0;

    private int missServiceCnt = 0;


    public bool ConsumeContinueDispatch = true;

    public ServiceWorkThread(int factor, int dispatchCount)
    {
        this.Factor = factor;
        this.DispatchCount = dispatchCount;
    }


    protected ServiceBase? PopService()
    {
        var serviceId = ServiceManager.Instance
            .DequeueGlobalQueue();
        if (serviceId == 0)
        {
            return null;
        }

        return ServiceManager.Instance
            .GetService(serviceId);
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
    protected int DispatchService()
    {
        if (this.Service == null)
        {
            this.Service = this.PopService();
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
            if (!needContinueDispatch)
            {
                this.Service = null;
            }
            var popService = this.PopService();
            if (popService != null)
            {
                this.Service = popService;
            }
            if (needContinueDispatch && popService != null)
            {
                ServiceManager.Instance.PushGlobalQueue(serviceId);
            }
        }
        return ThreadDispatchType.None;
    }

    public void ThreadDispatchService(Object? stateInfo)
    {
        while (true)
        {
            var ret = this.DispatchService();
            switch (ret)
            {
                case ThreadDispatchType.None:
                    break;
                case ThreadDispatchType.SpinWait:
                    Thread.SpinWait(1);
                    break;
                case ThreadDispatchType.Sleep:
                    Thread.Sleep(1);
                    break;
                default:
                    break;
            }

        }
    }
}