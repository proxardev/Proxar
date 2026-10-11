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


using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;

namespace Proxar.ServiceCore.Dispatch;

internal static class DispatchStats
{
    // 生产者可能来自任意线程（定时器线程/业务线程/网络线程），用 64 条分条避免 Interlocked 争用，
    // 否则计数器本身会污染观测结果。
    private const int Stripes = 64;
    private static readonly long[] _msgPush = new long[Stripes];     // PushMessage / PushQueue0Message 次数
    private static readonly long[] _schedPush = new long[Stripes];   // 服务由"非就绪→就绪"的次数（= 原 Semaphore.Release 次数）
    private static readonly long[] _timerReg = new long[Stripes];    // TimerCall → wheel.NewTimeout 次数

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int Stripe() => Environment.CurrentManagedThreadId & (Stripes - 1);

    [Conditional("DEBUG")]
    public static void OnMsgPush() => _msgPush[Stripe()]++;

    [Conditional("DEBUG")]
    public static void OnSchedulePush() => _schedPush[Stripe()]++;

    [Conditional("DEBUG")]
    public static void OnTimerRegister() => _timerReg[Stripe()]++;

    private static long Sum(long[] a)
    {
        long s = 0;
        for (int i = 0; i < a.Length; i++) s += a[i];
        return s;
    }

    [Conditional("DEBUG")]
    internal static void Init()
    {
        if (Environment.GetEnvironmentVariable("PROXAR_STATS") == "1")
        {
            EnableReport(1000);
        }
    }

    [Conditional("DEBUG")]
    public static void EnableReport(int intervalMs)
    {
        var t = new Thread(() => { try { Loop(intervalMs); } catch { } })
        {
            IsBackground = true,
            Name = "DispatchStats"
        };
        t.Start();
    }

    private static void Loop(int intervalMs)
    {
        var proc = Process.GetCurrentProcess();
        var sw = Stopwatch.StartNew();

        long pMsg = 0, pSched = 0, pTimer = 0;
        long pWakeReq = 0, pWakeSent = 0, pNoNeed = 0;
        TimeSpan pCpu = proc.TotalProcessorTime;
        long pMs = 0;
        var prev = new Dictionary<int, long[]>();

        Console.WriteLine("[stats] 调用: park=进入Semaphore.Wait次数  spur=醒来后没找到服务(虚假唤醒)  " +
                          "loop=DispatchService调用  popHit/popMiss=全局队列取服务  rsvc=取得服务次数  " +
                          "msg=派发消息  drop=释放服务  empty=队列空导致的空转");

        while (true)
        {
            Thread.Sleep(intervalMs);

            long msg = Sum(_msgPush), sched = Sum(_schedPush), timer = Sum(_timerReg);
            var workers = ServiceManager.Instance.ServiceWorkerThreads;

            long popHit = 0, popMiss = 0, loop = 0, rsvc = 0, msgs = 0, drop = 0, empty = 0, park = 0, spur = 0, spinHit = 0, spinMiss = 0;
            var sb = new StringBuilder();
            for (int i = 0; i < workers.Count; i++)
            {
                var statRecord = workers[i].StatRecord;
                if (statRecord == null)
                {
                    continue;
                }
                long[] cur =
                [
                    statRecord.StatPark, statRecord.StatSpurious, statRecord.StatLoop, statRecord.StatPopHit, statRecord.StatPopMiss,
                    statRecord.StatTakeService, statRecord.StatMsg, statRecord.StatDrop, statRecord.StatEmpty,
                    statRecord.StatSpinHit, statRecord.StatSpinMiss
                ];
                prev.TryGetValue(i, out var old);
                if (old == null) { prev[i] = cur; continue; }

                long dPark = cur[0] - old[0], dSpur = cur[1] - old[1], dLoop = cur[2] - old[2];
                long dHit = cur[3] - old[3], dMiss = cur[4] - old[4], dRsvc = cur[5] - old[5];
                long dMsg = cur[6] - old[6], dDrop = cur[7] - old[7], dEmpty = cur[8] - old[8];
                long dSpinHit = cur[9] - old[9], dSpinMiss = cur[10] - old[10];
                prev[i] = cur;

                park += dPark; spur += dSpur; loop += dLoop; popHit += dHit; popMiss += dMiss;
                rsvc += dRsvc; msgs += dMsg; drop += dDrop; empty += dEmpty;
                spinHit += dSpinHit; spinMiss += dSpinMiss;

                if (dPark != 0 || dMsg != 0 || dRsvc != 0)
                {
                    sb.AppendLine($"            w{i,2}: park={dPark,7} spur={dSpur,5} loop={dLoop,9} popHit={dHit,7} popMiss={dMiss,7} rsvc={dRsvc,7} msg={dMsg,7} drop={dDrop,7} empty={dEmpty,7}");
                }
            }

            proc.Refresh();
            long wallMs = sw.ElapsedMilliseconds;
            double cpuPct = (proc.TotalProcessorTime - pCpu).TotalMilliseconds / Math.Max(1, wallMs - pMs) * 100;
            pCpu = proc.TotalProcessorTime;
            pMs = wallMs;

            long wakeReq = WorkerSignal.WakeReq, wakeSent = WorkerSignal.WakeSent, noNeed = WorkerSignal.NoNeed;
            Console.WriteLine(
                $"[stats] {wallMs / 1000,4}s | msgPush={msg - pMsg,9} sched={sched - pSched,9} timerReg={timer - pTimer,9} | " +
                $"wakeReq={wakeReq - pWakeReq,8} wakeSent={wakeSent - pWakeSent,9} noNeed={noNeed - pNoNeed,9} | " +
                $"park={park,8} spur={spur,7} spinHit={spinHit,8} spinMiss={spinMiss,8} | " +
                $"loop={loop,10} popHit={popHit,9} popMiss={popMiss,9} | " +
                $"rsvc={rsvc,9} msg={msgs,9} drop={drop,9} empty={empty,9} | " +
                $"{WorkerSignal.Snapshot()} gqDepth={ServiceManager.Instance.GlobalQueueDepth,6} cpu={cpuPct,5:F1}% thr={proc.Threads.Count}");
            if (sb.Length != 0) Console.Write(sb.ToString());

            pMsg = msg; pSched = sched; pTimer = timer;
            pWakeReq = wakeReq; pWakeSent = wakeSent; pNoNeed = noNeed;
        }
    }
}
