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

/*
 * [FIX-A] 生产者侧"按需 + 定量"唤醒。没有任何周期性唤醒、没有心跳线程。
 *
 * 只看三个量的关系来决定唤醒几个线程：
 *      需要数 need = 就绪队列里待取的服务数(ready) − 已醒着、会自己去取活的线程数(awake)
 *      实际唤醒  toWake = clamp(need, 0, 已睡着的线程数(parked))
 *
 *   - 醒着的线程已能覆盖就绪服务 → 一个都不叫（绝大多数消息走这条路径，零唤醒）；
 *   - 活比人多 → 一次把差额补齐（可以一次叫醒多个），而不是"来一条消息叫一个"；
 *   - 没人睡着（都在忙）→ 无处可叫；正在跑的线程干完手上这轮会自己去全局队列取。
 *
 * 唤醒是"定向"的：直接 Set 某个睡着线程自己的 AutoResetEvent，不用共享信号量，
 * 因此不存在配额错配、不会唤醒不相干的线程；AutoResetEvent.Set 幂等，也不会累积配额。
 *
 * 防丢唤醒的协议（不需要任何心跳）：
 *   生产侧：先写 ready(入队) → 再读 parked/awake → 决定是否唤醒
 *   消费侧：先写 parked(登记睡觉) → 再读 ready → 有活就不睡
 * 两侧的先后顺序保证：要么生产侧看到"有人睡着"从而叫醒它，要么消费侧看到"有活"从而不睡。
 */

namespace Proxar.ServiceCore.Dispatch;

internal static class WorkerSignal
{
    private static int _awake;       // 醒着的 worker 数（正在干活，或正在取活/自旋，都能自己去拿）
    private static int _parked;      // 已登记睡觉的 worker 数
    private static int _readyDepth;  // 全局就绪队列里待取的服务数
    private static int _cursor;      // 唤醒轮转起点，避免总叫同一个线程

    private static ServiceWorkerThread?[] _workers = new ServiceWorkerThread?[64];
    private static int _workerCount;

    // [STATS]
    private const int Stripes = 64;
    private static readonly long[] _sWakeReq = new long[Stripes];   // 判定为"需要唤醒"的次数
    private static readonly long[] _sWakeSent = new long[Stripes];  // 实际 Set 的门铃数
    private static readonly long[] _sNoNeed = new long[Stripes];    // 醒着的够用 → 不唤醒的次数
    private static long Sum(long[] a) { long s = 0; for (int i = 0; i < a.Length; i++) s += a[i]; return s; }
    internal static long WakeReq => Sum(_sWakeReq);
    internal static long WakeSent => Sum(_sWakeSent);
    internal static long NoNeed => Sum(_sNoNeed);
    internal static string Snapshot() => $"awake={Volatile.Read(ref _awake)} parked={Volatile.Read(ref _parked)} ready={Volatile.Read(ref _readyDepth)}";
    private static int Stripe() => Environment.CurrentManagedThreadId & (Stripes - 1);

    internal static int ReadyDepth => Volatile.Read(ref _readyDepth);

    internal static void Register(ServiceWorkerThread w)
    {
        int i = Interlocked.Increment(ref _workerCount) - 1;
        if (i < _workers.Length)
        {
            _workers[i] = w;
        }
    }

    internal static void OnWorkerStarted() => Interlocked.Increment(ref _awake);

    /// <summary>服务进入全局就绪队列后调用（在入队之后）。</summary>
    internal static void OnServiceReady()
    {
        int ready = Volatile.Read(ref _readyDepth);
        int awake = Volatile.Read(ref _awake);
        int need = ready - awake;                 // 还缺几个线程才够覆盖就绪的服务数
        if (need <= 0)
        {
            _sNoNeed[Stripe()]++;
            return;                               // 醒着的够用 → 一个都不叫
        }
        int parked = Volatile.Read(ref _parked);
        if (parked <= 0)
        {
            return;                               // 没人睡着 → 无处可叫
        }
        int toWake = need < parked ? need : parked;
        toWake = toWake > 0 ? 1 : 0;
        _sWakeReq[Stripe()]++;
        WakeSome(toWake);
    }

    /// <summary>定向叫醒至多 count 个已经睡着的线程（轮转挑选，负载均摊）。</summary>
    private static void WakeSome(int count)
    {
        var workers = _workers;
        int n = Volatile.Read(ref _workerCount);
        if (n > workers.Length)
        {
            n = workers.Length;
        }
        if (n == 0)
        {
            return;
        }
        int start = Interlocked.Increment(ref _cursor);
        int sent = 0;
        for (int i = 0; i < n && sent < count; i++)
        {
            var w = workers[(start + i) % n];
            if (w == null || Volatile.Read(ref w.ParkedFlag) == 0)
            {
                continue;
            }
            w.ParkGate.Set();                     // 幂等；该线程若还没真正 Wait，门铃会留着
            sent++;
        }
        if (sent != 0)
        {
            _sWakeSent[Stripe()] += sent;
        }
    }

    internal static void OnEnqueued() => Interlocked.Increment(ref _readyDepth);
    internal static void OnDequeued() => Interlocked.Decrement(ref _readyDepth);

    /// <summary>
    /// 登记睡觉前的检查。返回 true 表示"已经有活，不要睡"。
    /// 顺序很重要：先登记 parked/awake，再复查 ready。
    /// </summary>
    internal static bool Park(ServiceWorkerThread w)
    {
        Volatile.Write(ref w.ParkedFlag, 1);
        Interlocked.Increment(ref _parked);
        Interlocked.Decrement(ref _awake);
        return Volatile.Read(ref _readyDepth) > 0;
    }

    internal static void Unpark(ServiceWorkerThread w)
    {
        Volatile.Write(ref w.ParkedFlag, 0);
        Interlocked.Decrement(ref _parked);
        Interlocked.Increment(ref _awake);
    }
}
