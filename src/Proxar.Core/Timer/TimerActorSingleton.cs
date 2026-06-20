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


using Proxar.ActorSingletonCore;
using Proxar.Tasks;
using Proxar.Timer.Interfaces;

namespace Proxar.Timer;

/// <summary>
/// 服务内全局定时器实现，声明周期跟随服务的生命周期
/// </summary>
public class TimerActorSingleton : ActorSingleton<TimerActorSingleton>, ITimerObject
{
    private const bool DELAY_REMOVE_TIMER = true;
    private const int PER_REMOVE_TIMER_COUNT = 12;

    private TimerBase timer = new(DELAY_REMOVE_TIMER, PER_REMOVE_TIMER_COUNT);



    /// <inheritdoc/>
    public long TimerCall(long milliSecond, Action action)
    {
        return timer.TimerCall(milliSecond, action);
    }


    /// <inheritdoc/>
    public long TimerCall<T>(long milliSecond, Action<T> action, T args)
    {
        return timer!.TimerCall(milliSecond, action, args);

    }


    /// <inheritdoc/>
    public long IntervalTimerCall(long milliSecond, Action action)
    {
        return timer!.IntervalTimerCall(milliSecond, action);
    }


    /// <inheritdoc/>
    public long IntervalTimerCall<T>(long milliSecond, Action<T> action, T args)
    {
        return timer!.IntervalTimerCall(milliSecond, action, args);

    }


    /// <inheritdoc/>
    public async ZFTask Delay(long milliSecond)
    {
        await timer!.Delay(milliSecond);
    }


    /// <inheritdoc/>
    public void CancelTimer(long id)
    {
        timer!.CancelTimer(id);
    }


    /// <inheritdoc/>
    public void CancelAllTimer()
    {
        timer!.CancelAllTimer();
    }


    /// <inheritdoc/>
    public bool HasTimer(long id)
    {
        return timer!.HasTimer(id);
    }


    /// <inheritdoc/>
    public override void Dispose()
    {
        this.timer.Dispose();
    }
}