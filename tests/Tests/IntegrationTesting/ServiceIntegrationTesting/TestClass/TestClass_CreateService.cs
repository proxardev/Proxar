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


public class TestClass_CreateService : TestClassBase
{

    [Theory]
    [InlineData(nameof(TestService_CreateProxy.CreateAndDestoryService_Succ), 1)]
    [InlineData(nameof(TestService_CreateProxy.CreateAndDestoryService_Succ), 100)]
    [InlineData(nameof(TestService_CreateProxy.CreateAndDestoryService_Succ), 10000)]

    [InlineData(nameof(TestService_CreateProxy.ParallelCreateService_Succ), 10, 1000)]
    [InlineData(nameof(TestService_CreateProxy.ParallelCreateService_Succ), 10, 10000)]

    [InlineData(nameof(TestService_CreateProxy.MulCallAndSend_CallSendSucc), 1000)]
    [InlineData(nameof(TestService_CreateProxy.MulCallAndSend_CallSendSucc), 100000)]
    internal async Task CallTargetProtoMethod(string methodName, int args = int.MinValue, int args2 = int.MinValue,
        int args3 = int.MinValue,
        int args4 = int.MinValue
        )
    {
        await this.CallMethodByProxy<TestService_Create, TestService_CreateProxy>(methodName, args, args2, args3, args4);
    }
}