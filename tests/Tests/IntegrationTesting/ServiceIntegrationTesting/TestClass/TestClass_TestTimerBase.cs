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


using TestShared.TestClass;

namespace ServiceIntegrationTesting.TestClass;

public abstract class TestClass_TestTimerBase : TestClassBase
{
    [Theory]
    [InlineData(nameof(TestService_TimerProxy.Delay_ExecuteAfterDelay), -100)]
    [InlineData(nameof(TestService_TimerProxy.Delay_ExecuteAfterDelay), -1)]
    [InlineData(nameof(TestService_TimerProxy.Delay_ExecuteAfterDelay), 0)]
    [InlineData(nameof(TestService_TimerProxy.Delay_ExecuteAfterDelay), 1)]
    [InlineData(nameof(TestService_TimerProxy.Delay_ExecuteAfterDelay), 10)]
    [InlineData(nameof(TestService_TimerProxy.Delay_ExecuteAfterDelay), 1000)]
    [InlineData(nameof(TestService_TimerProxy.MullDelay_ExecuteAfterDelay))]
    [InlineData(nameof(TestService_TimerProxy.LoopDelay_ExecuteAfterDelay), 1)]
    [InlineData(nameof(TestService_TimerProxy.LoopDelay_ExecuteAfterDelay), 5)]

    [InlineData(nameof(TestService_TimerProxy.TimerCallBack_CallBackSucc), -1, 10)]
    [InlineData(nameof(TestService_TimerProxy.TimerCallBack_CallBackSucc), 0, 10)]
    [InlineData(nameof(TestService_TimerProxy.TimerCallBack_CallBackSucc), 1, 10)]
    [InlineData(nameof(TestService_TimerProxy.TimerCallBack_CallBackSucc), 10, 10)]
    [InlineData(nameof(TestService_TimerProxy.TimerGenericCallBack_CallBackSucc), -1, 10)]
    [InlineData(nameof(TestService_TimerProxy.TimerGenericCallBack_CallBackSucc), 0, 10)]
    [InlineData(nameof(TestService_TimerProxy.TimerGenericCallBack_CallBackSucc), 1, 10)]
    [InlineData(nameof(TestService_TimerProxy.TimerGenericCallBack_CallBackSucc), 10, 10)]
    [InlineData(nameof(TestService_TimerProxy.TimerCallBack_CallBackOnlyOne))]
    [InlineData(nameof(TestService_TimerProxy.TimerGenericCallBack_CallBackOnlyOne))]

    [InlineData(nameof(TestService_TimerProxy.IntervalTimerCall_CallBackNumberTimesAndCancelFinal), -1, 10)]
    [InlineData(nameof(TestService_TimerProxy.IntervalTimerCall_CallBackNumberTimesAndCancelFinal), 0, 10)]
    [InlineData(nameof(TestService_TimerProxy.IntervalTimerCall_CallBackNumberTimesAndCancelFinal), 1, 10)]
    [InlineData(nameof(TestService_TimerProxy.IntervalTimerCall_CallBackNumberTimesAndCancelFinal), 10, 10)]
    [InlineData(nameof(TestService_TimerProxy.GenericIntervalTimerCall_CallBackNumberTimesAndCancelFinal), -1, 10)]
    [InlineData(nameof(TestService_TimerProxy.GenericIntervalTimerCall_CallBackNumberTimesAndCancelFinal), 0, 10)]
    [InlineData(nameof(TestService_TimerProxy.GenericIntervalTimerCall_CallBackNumberTimesAndCancelFinal), 1, 10)]
    [InlineData(nameof(TestService_TimerProxy.GenericIntervalTimerCall_CallBackNumberTimesAndCancelFinal), 10, 10)]
    [InlineData(nameof(TestService_TimerProxy.CancelTimerCall_TimerStop), 10, 10)]
    [InlineData(nameof(TestService_TimerProxy.CancelTimerCall_TimerStop), 10, 1000)]
    [InlineData(nameof(TestService_TimerProxy.CancelGenericTimerCall_TimerStop), 10, 10)]
    [InlineData(nameof(TestService_TimerProxy.CancelGenericTimerCall_TimerStop), 10, 1000)]
    [InlineData(nameof(TestService_TimerProxy.CancelIntervalTimerCall_TimerStop), 10, 10)]
    [InlineData(nameof(TestService_TimerProxy.CancelIntervalTimerCall_TimerStop), 10, 1000)]
    [InlineData(nameof(TestService_TimerProxy.CancelGenericIntervalTimerCall_TimerStop), 10, 10)]
    [InlineData(nameof(TestService_TimerProxy.CancelGenericIntervalTimerCall_TimerStop), 10, 1000)]

    [InlineData(nameof(TestService_TimerProxy.CancelAllTimer_AllTimerStop), 10, 1)]
    [InlineData(nameof(TestService_TimerProxy.CancelAllTimer_AllTimerStop), 10, 10000)]

    [InlineData(nameof(TestService_TimerProxy.LongTimed_SussTimed), 100 * 24 * 60)]

    [InlineData(nameof(TestService_TimerProxy.MulServiceCreateTimer_Succ), 10, 1000, 10)]
    [InlineData(nameof(TestService_TimerProxy.MulServiceCreateTimer_Succ), 100, 1000, 10)]
    internal async Task CallMethodTest(string methodName, int args = int.MinValue, int args2 = int.MinValue,
        int args3 = int.MinValue,
        int args4 = int.MinValue
        )
    {
        await this.CallMethodByProxy<TestService_Timer, TestService_TimerProxy>(methodName, args, args2, args3, args4);
    }
}