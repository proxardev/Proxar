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


using Proxar.CachePool;
using Proxar.CachePool.Interfaces;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;

namespace Proxar.Tasks;

public abstract class ZFTaskBase<TSelf> : AbstractPoolable<TSelf>, IResettablePooled
    where TSelf : ZFTaskBase<TSelf>, new()
{
    internal long Id { get; set; }
    protected Action? action;
    protected ZFTaskState state;
    protected ExceptionDispatchInfo? exceptionInfo = null;

    public bool IsCompleted
    {
        get
        {
            return this.ValidCompleted();
        }
    }

    public bool IsFaulted
    {
        get
        {
            return this.state == ZFTaskState.Faulted;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool ValidCompleted()
    {
        return this.state != ZFTaskState.Pending;
    }

    private void CheckHandledException()
    {
        if (!IsFaulted)
        {
            return;
        }
        if (this.exceptionInfo == null)
        {
            return;
        }
        ZFTaskConfig.UnhandledExceptionHandler?.Invoke(exceptionInfo.SourceException);
    }

    public void Coroutine()
    {
        ArgumentNullException.ThrowIfNull(ZFTaskConfig.UnhandledExceptionHandler);

        // 如果已完成，立即检查异常
        if (IsCompleted)
        {
            CheckHandledException();
            return;
        }

        // 未完成则注册延续，完成时检查异常
        UnsafeOnCompleted(CheckHandledException);
    }

    public void Reset()
    {
        this.state = ZFTaskState.Pending;
        this.action = null;
        this.exceptionInfo = null;
        this.OnReset();
    }

    protected virtual void OnReset()
    {

    }

    public void OnCompleted(Action continuation)
    {
        this.UnsafeOnCompleted(continuation);
    }

    public void UnsafeOnCompleted(Action continuation)
    {
        if (this.ValidCompleted())
        {
            continuation?.Invoke();
            return;
        }
        this.action = continuation;
    }

    protected void SetResultCore()
    {
        if (this.ValidCompleted())
        {
            throw new InvalidOperationException($"{this.GetType().FullName} Task Already Completed");
        }

        this.state = ZFTaskState.Succeeded;
        var continuation = this.action;
        this.action = null;
        continuation?.Invoke();
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected void CheckResult()
    {
        // TODO 完善下异常传播，当有多个await的时候
        switch (this.state)
        {
            case ZFTaskState.Succeeded:
                break;
            case ZFTaskState.Faulted:
                exceptionInfo?.Throw();
                break;
            case ZFTaskState.Canceled:
                throw new OperationCanceledException();
            default:
                throw new InvalidOperationException("任务状态无效");
        }
    }

    public void SetException(Exception ex)
    {
        exceptionInfo = ExceptionDispatchInfo.Capture(ex);
        state = ZFTaskState.Faulted;
        var continuation = this.action;
        this.action = null;
        continuation?.Invoke();
    }

    public void Cancel()
    {
        this.state = ZFTaskState.Canceled;
        this.action = null;
    }
}