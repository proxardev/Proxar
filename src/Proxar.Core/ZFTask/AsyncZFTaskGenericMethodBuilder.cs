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
#pragma warning disable CS1591

using Proxar.Core;
using System.Runtime.CompilerServices;

namespace Proxar.Tasks;


[ExcludeFromDoc]
public struct AsyncZFTaskGenericMethodBuilder<TRet>
{
    private ZFTask<TRet> task;
    public static AsyncZFTaskGenericMethodBuilder<TRet> Create()
    {
        return default;
    }


    public ZFTask<TRet> Task
    {
        get
        {
            return InitializeZFTask();
        }
    }

    private ZFTask<TRet> InitializeZFTask()
    {
        if (task != null)
        {
            return task;
        }
        return task = ZFTask<TRet>.CreateZFTask();
    }

    public void Start<TStateMachine>(ref TStateMachine stateMachine) where TStateMachine : IAsyncStateMachine
    {
        stateMachine.MoveNext();
    }

    public void SetResult(TRet ret)
    {
        if (task != null)
        {
            task.SetResult(ret);
            return;
        }
        task = ZFTask<TRet>.FromResult(ret);
    }


    public void SetException(Exception exception)
    {
        InitializeZFTask();
        task.SetException(exception);
    }

    public void AwaitOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter awaiter, ref TStateMachine stateMachine)
        where TAwaiter : INotifyCompletion
        where TStateMachine : IAsyncStateMachine
    {
        InitializeZFTask();
        awaiter.OnCompleted(stateMachine.MoveNext);
    }

    public void AwaitUnsafeOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter waiter, ref TStateMachine stateMachine)
        where TAwaiter : ICriticalNotifyCompletion
        where TStateMachine : IAsyncStateMachine
    {
        InitializeZFTask();
        waiter.OnCompleted(stateMachine.MoveNext);
    }

    public void SetStateMachine(IAsyncStateMachine stateMachine)
    {
        // 通常不需要在这里执行任何操作
    }
}