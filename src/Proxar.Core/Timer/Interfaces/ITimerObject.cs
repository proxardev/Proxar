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
namespace Proxar.Timer.Interfaces;

public interface ITimerObject : IDisposable
{

    public long TimerCall(long milliSecond, Action action);

    public long TimerCall<T>(long milliSecond, Action<T> action, T args);

    public long IntervalTimerCall(long milliSecond, Action action);

    public long IntervalTimerCall<T>(long milliSecond, Action<T> action, T args);

    public ZFTask Delay(long milliSecond);

    public void CancelTimer(long id);

    public void CancelAllTimer();

    public bool HasTimer(long id);

    public void DisposeTimerResources();
}