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


public class TestServiceFunctionality : TestClassBase
{

    [Theory]
    [InlineData(nameof(TestServiceProxy.Call_Should_Response))]

    [InlineData(nameof(TestServiceProxy.Call_MullCall_ReturnInOrder))]

    [InlineData(nameof(TestServiceProxy.Timer_CreateTimer_TimeoutExecMethodOnlyOneTimes))]
    [InlineData(nameof(TestServiceProxy.Timer_Delay_DelaySucc))]
    [InlineData(nameof(TestServiceProxy.Timer_InternalCall_CallOverOneTimes))]
    [InlineData(nameof(TestServiceProxy.Timer_CancelTimer_ShouldNotExecute))]
    [InlineData(nameof(TestServiceProxy.Timer_CancelAllTimer_AllTimerShouldStop), 100)]
    [InlineData(nameof(TestServiceProxy.Timer_CancelAllTimer_AllTimerShouldStop), 1000)]

    [InlineData(nameof(TestServiceProxy.ActorSingleton_GetActorSingleton_ActorSingletonShouldBelongThisService), 100, 10000, 1)]
    [InlineData(nameof(TestServiceProxy.ActorSingleton_GetActorSingleton_ActorSingletonShouldBelongThisService), 1000, 10000, 1)]
    internal async Task CallMethodTest(string methodName, int args = int.MinValue, int args2 = int.MinValue,
        int args3 = int.MinValue,
        int args4 = int.MinValue
        )
    {
        await this.CallMethodByProxy<TestService, TestServiceProxy>(methodName, args, args2, args3, args4);
    }
}