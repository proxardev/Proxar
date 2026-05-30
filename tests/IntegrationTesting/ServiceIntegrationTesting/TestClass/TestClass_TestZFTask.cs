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

public class TestClass_TestZFTask : TestClassBase
{
    [Theory]

    [InlineData(nameof(TestService_ZFTaskProxy.ZFTask_ValueTaskSyncReturn_CompletesImmediately))]
    [InlineData(nameof(TestService_ZFTaskProxy.ZFTask_VoidTaskSyncReturn_CompletesImmediately))]
    [InlineData(nameof(TestService_ZFTaskProxy.ZFTask_NestedCallSyncReturn_OuterReceivesInnerTask))]

    [InlineData(nameof(TestService_ZFTaskProxy.ZFTask_ValueTypeAsyncReturn_WaitMomentComplete))]
    [InlineData(nameof(TestService_ZFTaskProxy.ZFTask_VoidTaskASyncReturn_WaitMomentComplete))]
    [InlineData(nameof(TestService_ZFTaskProxy.ZFTask_NestedCallASyncReturn_WaitMomentComplete))]

    [InlineData(nameof(TestService_ZFTaskProxy.ZFTask_MultipleAwait_Sequential_ExecutesInOrder))]
    [InlineData(nameof(TestService_ZFTaskProxy.ZFTask_NestedAwait_DoubleLayer_BothComplete))]

    [InlineData(nameof(TestService_ZFTaskProxy.ZFTask_ConditionalAwait_AwaitsAndSyncComplete))]

    [InlineData(nameof(TestService_ZFTaskProxy.ZFTask_LoopAwait_ForLoop_ExecutesAllIterations))]
    [InlineData(nameof(TestService_ZFTaskProxy.GeneralZFTask_LoopAwait_ForLoop_ExecutesAllIterations))]
    [InlineData(nameof(TestService_ZFTaskProxy.ZFTask_LoopAwait_WhileLoop_BreakEarly))]
    [InlineData(nameof(TestService_ZFTaskProxy.GeneralZFTask_LoopAwait_WhileLoop_BreakEarly))]

    [InlineData(nameof(TestService_ZFTaskProxy.ZFTask_Exception_SyncThrow_CapturedInTask))]
    [InlineData(nameof(TestService_ZFTaskProxy.ZFTask_Exception_AsyncThrow_CapturedInTask))]
    [InlineData(nameof(TestService_ZFTaskProxy.GenericZFTask_Exception_SyncThrow_CapturedInTask))]
    [InlineData(nameof(TestService_ZFTaskProxy.GenericZFTask_Exception_AsyncThrow_CapturedInTask))]
    [InlineData(nameof(TestService_ZFTaskProxy.GenericZFTask_LoopException_AsyncThrow_CapturedInTask))]
    [InlineData(nameof(TestService_ZFTaskProxy.GenericZFTask_GenericZFTaskLoopException_AsyncThrow_CapturedInTask))]
    [InlineData(nameof(TestService_ZFTaskProxy.ZFTask_NestedTaskLoopException_AsyncThrow_CapturedInTask))]
    [InlineData(nameof(TestService_ZFTaskProxy.GenericZFTask_NestedGenericZFTaskLoopException_AsyncThrow_CapturedInTask))]
    [InlineData(nameof(TestService_ZFTaskProxy.GenericZFTask_NestedStringGenericZFTaskLoopException_AsyncThrow_CapturedInTask))]

    [InlineData(nameof(TestService_ZFTaskProxy.ZFTask_ComplexFlow_AsyncExceptionTryCatchFinally_ExecutesFinally), 0)]
    [InlineData(nameof(TestService_ZFTaskProxy.ZFTask_ComplexFlow_AsyncExceptionTryCatchFinally_ExecutesFinally), 1)]
    [InlineData(nameof(TestService_ZFTaskProxy.ZFTask_ComplexFlow_SyncExceptionTryCatchFinally_ExecutesFinally), 0)]
    [InlineData(nameof(TestService_ZFTaskProxy.ZFTask_ComplexFlow_SyncExceptionTryCatchFinally_ExecutesFinally), 1)]
    [InlineData(nameof(TestService_ZFTaskProxy.GenericZFTask_ComplexFlow_SyncExceptionTryCatchFinally_ExecutesFinally), 0)]
    [InlineData(nameof(TestService_ZFTaskProxy.GenericZFTask_ComplexFlow_SyncExceptionTryCatchFinally_ExecutesFinally), 1)]
    [InlineData(nameof(TestService_ZFTaskProxy.GenericZFTask_ComplexFlow_AsyncExceptionTryCatchFinally_ExecutesFinally), 0)]
    [InlineData(nameof(TestService_ZFTaskProxy.GenericZFTask_ComplexFlow_AsyncExceptionTryCatchFinally_ExecutesFinally), 1)]

    [InlineData(nameof(TestService_ZFTaskProxy.ZFTask_InfiniteAwaitLoop_DoesNotStackOverflow))]
    [InlineData(nameof(TestService_ZFTaskProxy.GenericZFTask_InfiniteAwaitLoop_DoesNotStackOverflow))]
    [InlineData(nameof(TestService_ZFTaskProxy.ZFTask_DeepRecursion_DoesNotCrash))]
    [InlineData(nameof(TestService_ZFTaskProxy.GenericZFTask_DeepRecursion_DoesNotCrash))]


    internal async Task CallMethodTest(string methodName, int args = int.MinValue, int args2 = int.MinValue,
        int args3 = int.MinValue,
        int args4 = int.MinValue
        )
    {
        await this.CallMethodByProxy<TestService_ZFTask, TestService_ZFTaskProxy>(methodName, args, args2, args3, args4);
    }
}