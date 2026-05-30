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


using Proxar.IdGenerator;
using Proxar.ServiceSynchronizationContext;
using Proxar.Tasks;
using Proxar.Timer.Interfaces;
using Proxar.Utilities;
namespace Proxar.Timer;

public class TimerBase : BaseObject, ITimerObject
{
    private IIdGenerator<long> timerIdGenerator;
    private Dictionary<long, TimerTaskObject> timerDict = new Dictionary<long, TimerTaskObject>();
    private Dictionary<long, HWT.Timeout> timeoutDict = new Dictionary<long, HWT.Timeout>();
    private Queue<long> waitRemoveTimerIdQueue = new Queue<long>();
    private bool delayRemoveOnCancel = false;
    private bool registerDelayRemoveAction = false;
    private int delayRemovePerCallRemoveCount = 5;

    public TimerBase()
        : base()
    {
        this.timerIdGenerator = GetTimerIdGenerator();
    }

    public TimerBase(bool delayRemoveOnCancel,
        int delayRemovePerCallRemoveCount)
        : this()
    {
        this.delayRemovePerCallRemoveCount = Math.Max(this.delayRemovePerCallRemoveCount,
            delayRemovePerCallRemoveCount);
        this.delayRemoveOnCancel = false;
    }

    private static Int64IdGenerator GetTimerIdGenerator()
    {
        var synchronizationContext = SynchronizationContextHelper
            .GetSynchronization<ActorSynchronizationContext>()!;
        return synchronizationContext.GetTimerIdGenerator();
    }

    private HWT.Timeout TimerCall(long milliSecond, TimerTaskObject timer, bool clearInfo = true)
    {
        var hashedWheelTimer = TimerManager.Instance.GetHashedWheelTimer();
        return hashedWheelTimer.NewTimeout(timer, TimeSpan.FromMilliseconds(milliSecond));
    }

    private long TimerCall(long milliSecond, Action action, bool runClearInfo)
    {
        var id = timerIdGenerator.NewId();
        var timer = new TimerTaskObject(id, action, this.RemoveTimer, runClearInfo);
        var timeout = this.TimerCall(milliSecond, timer);
        this.timerDict[id] = timer;
        this.timeoutDict[id] = timeout;
        return id;
    }

    public long TimerCall(long milliSecond, Action action)
    {
        return this.TimerCall(milliSecond, action, true);
    }

    public long TimerCall<T>(long milliSecond, Action<T> action, T args)
    {
        var action2 = () =>
        {
            action.Invoke(args);
        };
        return this.TimerCall(milliSecond, action2);

    }

    public long IntervalTimerCall(long milliSecond, Action action)
    {
        long id = 0;
        TimerTaskObject timer = null!;
        var nextCallTime = TimeHelper.GetMSSecond() + milliSecond;
        var action2 = () =>
        {
            nextCallTime += milliSecond;
            var next = Math.Max(0, nextCallTime - TimeHelper.GetMSSecond());
            var timeout = this.TimerCall(next, timer);
            this.timerDict.Add(id, timer!);
            this.timeoutDict[id] = timeout;

            action.Invoke();
        };
        id = this.TimerCall(milliSecond, action2, false);
        timer = this.timerDict[id];
        return id;
    }

    public long IntervalTimerCall<T>(long milliSecond, Action<T> action, T args)
    {
        var action2 = () =>
        {
            action.Invoke(args);
        };
        return this.IntervalTimerCall(milliSecond, action2);
    }

    public async ZFTask Delay(long milliSecond)
    {
        var ts = ZFTask.CreateZFTask();
        var func = () =>
        {
            ts.SetResult();
        };
        this.TimerCall(milliSecond, func);
        await ts;
    }

    private async Task DelayAction(int milliSecond, ZFTask ts)
    {
        var hashedWheelTimer = TimerManager.Instance.GetHashedWheelTimer();
        await hashedWheelTimer.Delay(milliSecond);

        this.GetSynchronization()?.Post(ts.SetResult);
    }

    private void RemoveTimer(long id)
    {
        this.timerDict.Remove(id);
        this.timeoutDict.Remove(id);
    }

    public void CancelTimer(long id)
    {
        if (!this.timerDict.ContainsKey(id))
        {
            return;
        }
        var timer = this.timerDict[id];
        timer.Cancel();
        this.timerDict.Remove(id);

        //remove
        if (!this.delayRemoveOnCancel)
        {
            this.timeoutDict[id].Cancel();
            this.timeoutDict.Remove(id);
            return;
        }
        // daley remove
        this.waitRemoveTimerIdQueue.Enqueue(id);
        this.RegisterDelayRemove();
    }

    public void CancelAllTimer()
    {
        var timerInfo = this.timerDict;
        this.timerDict = new Dictionary<long, TimerTaskObject>();
        if (!this.delayRemoveOnCancel)
        {
            this.CancelAllTimerAndImmediatelyRemove(timerInfo);
        }
        else
        {
            this.CancelAllTimerAndDelayRemove(timerInfo);
        }
    }

    private void CancelAllTimerAndImmediatelyRemove(Dictionary<long, TimerTaskObject> timerInfo)
    {
        foreach (var (id, timer) in timerInfo)
        {
            timer.Cancel();
            this.timeoutDict[id].Cancel();
            this.timeoutDict.Remove(id);
        }
    }

    private void CancelAllTimerAndDelayRemove(Dictionary<long, TimerTaskObject> timerInfo)
    {
        var capacity = this.waitRemoveTimerIdQueue.Count
            + timerInfo.Count;
        this.waitRemoveTimerIdQueue.EnsureCapacity(capacity);
        foreach (var (id, timer) in timerInfo)
        {
            timer.Cancel();
            this.waitRemoveTimerIdQueue.Enqueue(id);
        }
        this.RegisterDelayRemove();
    }

    public override void DisposeResources()
    {
        this.DisposeTimerResources();
    }

    private void RemoveAllTimer()
    {
        this.CancelAllTimer();
        this.ExecuteRemoveAll();
    }

    private void ExecuteRemoveAll()
    {
        var count = this.waitRemoveTimerIdQueue.Count;
        this.ExecuteRemoveTimer(count);
    }

    private void ExecuteRemoveTimer(int count)
    {
        for (int i = 0; i < count; i++)
        {
            var id = this.waitRemoveTimerIdQueue.Dequeue();
            var get = this.timeoutDict.TryGetValue(id, out var timeout);
            this.timeoutDict.Remove(id);
            if (!get || timeout == null)
            {
                continue;
            }
            timeout.Cancel();
        }
    }

    private void ExecuteDelayRemove()
    {
        this.registerDelayRemoveAction = false;

        var count = Math.Min(this.delayRemovePerCallRemoveCount,
            this.waitRemoveTimerIdQueue.Count);
        this.ExecuteRemoveTimer(count);

        if (this.waitRemoveTimerIdQueue.Count != 0)
        {
            this.RegisterDelayRemove();
        }
    }
    private void RegisterDelayRemove()
    {
        if (this.registerDelayRemoveAction)
        {
            return;
        }
        this.registerDelayRemoveAction = true;
        this.RegisterDelayRemoveTimer(this.ExecuteDelayRemove);
    }

    private void RegisterDelayRemoveTimer(Action action)
    {
        this.GetSynchronization()?.Post(action);
    }

    public void DisposeTimerResources()
    {
        this.RemoveAllTimer();
    }

    public bool HasTimer(long id)
    {
        if (!this.timerDict.ContainsKey(id))
        {
            return false;
        }
        var timer = this.timerDict[id];
        if (timer.IsCancel())
        {
            return false;
        }
        return true;
    }
}