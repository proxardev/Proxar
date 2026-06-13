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


using Proxar.Tasks.Interfaces;
using System.Runtime.CompilerServices;

namespace Proxar.Tasks;



[AsyncMethodBuilder(typeof(AsyncZFTaskGenericMethodBuilder<>))]
public sealed class ZFTask<TRet> : ZFTaskBase<ZFTask<TRet>>, ICriticalNotifyCompletion, ITask
{
    private TRet? value;
    internal static readonly ZFTask<TRet> DefaultZFTaskResult = CreateCompletedTask();
    //internal static readonly ZFTask<TRet> FromResult = CreateFromResult<TRet>;
    public static Func<ZFTask<TRet>> CreateZFTask { get; set; } = InternalZFTaskHelper.CreateZFTaskOwnedByService<TRet>;
    public static Action<ZFTask<TRet>>? OnResultConsumed { get; set; } = InternalZFTaskHelper.ReturnZFTaskToPool<TRet>;

    public ZFTask()
    {
        this.Id = ZFTaskInt64IdGeneratorActorSingleton.Current
            .NewId();
    }

    private static ZFTask<TRet> CreateCompletedTask()
    {
        var task = new ZFTask<TRet>();
        task.state = ZFTaskState.Succeeded;
        task.value = default(TRet);
        return task;
    }

    public static ZFTask<TResult> FromResult<TResult>(TResult result)
    {
        var task = ZFTask<TResult>.CreateZFTask();
        task.value = result;
        task.state = ZFTaskState.Succeeded;
        return task;
    }

    //public new void Reset()
    //{
    //    base.Reset();
    //    this.state = ZFTaskState.Pending;
    //    this.action = null;
    //    this.Value = default(TRet);
    //    this.exceptionInfo = null;
    //}

    public ZFTask<TRet> GetAwaiter()
    {
        return this;
    }

    public TRet GetResult()
    {
        this.CheckResult();
        var value = this.value!;
        OnResultConsumed?.Invoke(this);
        return value;
    }

    public void SetResult(TRet result)
    {
        this.value = result;
        this.SetResultCore();
    }

    protected override void OnReset()
    {
        this.value = default(TRet);
    }

    public override void OnRented()
    {
        this.Id = ZFTaskInt64IdGeneratorActorSingleton.Current
            .NewId();
    }
}