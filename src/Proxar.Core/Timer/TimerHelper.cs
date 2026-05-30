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


using Proxar.Tasks;
namespace Proxar.Timer;

internal static class TimerHelper
{

    public static long TimerCall(int milliSecond, Action action)
    {
        var entity = TimerActorSingleton.Current;
        return entity!.TimerCall(milliSecond, action);
    }

    public static long TimerCall<T>(long milliSecond, Action<T> action, T args)
    {
        var entity = TimerActorSingleton.Current;
        return entity!.TimerCall(milliSecond, action, args);

    }

    public static long IntervalTimerCall(long milliSecond, Action action)
    {
        var entity = TimerActorSingleton.Current;
        return entity!.IntervalTimerCall(milliSecond, action);
    }

    public static long IntervalTimerCall<T>(long milliSecond, Action<T> action, T args)
    {
        var entity = TimerActorSingleton.Current;
        return entity!.IntervalTimerCall(milliSecond, action, args);

    }

    public static async ZFTask Delay(long milliSecond)
    {
        var entity = TimerActorSingleton.Current;
        await entity!.Delay(milliSecond);
    }

    public static void CancelTimer(long id)
    {
        var entity = TimerActorSingleton.Current;
        entity!.CancelTimer(id);
    }

    public static void CancelAllTimer()
    {
        var entity = TimerActorSingleton.Current;
        entity!.CancelAllTimer();
    }

    public static bool HasTimer(long id)
    {
        var entity = TimerActorSingleton.Current;
        return entity!.HasTimer(id);
    }

}