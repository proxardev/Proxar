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


namespace ServiceIntegrationTesting.TestClass;

public class TestClass_TestEntityTimer : TestClass_TestTimerBase
{
    protected override void CreateServiceCallBack(long serviceId)
    {
        TimerClassAbstract.Current.InitTestTimerObject(1);
    }

    protected override string GetRegisterServiceName<T>()
    {
        var name = typeof(T).Name;
        return name + this.GetType().Name;
    }

    [Theory]
    [InlineData(nameof(TestService_TimerProxy.DestoryTimerEntity_TimerEndLifeTime), 100)]
    [InlineData(nameof(TestService_TimerProxy.RecursionDestoryTimerEntity_TimerEndLifeTime), 100, 1000)]
    [InlineData(nameof(TestService_TimerProxy.RecursionDestoryTimerEntity_TimerEndLifeTime), 1000, 1000)]
    internal async Task CallMethodTest2(string methodName, int args = int.MinValue, int args2 = int.MinValue,
        int args3 = int.MinValue,
        int args4 = int.MinValue
        )
    {
        await this.CallMethodByProxy<TestService_Timer, TestService_TimerProxy>(methodName, args, args2, args3, args4);
    }

}