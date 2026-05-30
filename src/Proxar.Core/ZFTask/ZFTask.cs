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



[AsyncMethodBuilder(typeof(AsyncZFTaskMethodBuilder))]
public class ZFTask : AbstractPoolable<ZFTask>,
    ICriticalNotifyCompletion, IResettablePooled
{
    public long Id { get; internal set; }
    protected Action? action;
    protected ZFTaskState state;
    protected ExceptionDispatchInfo? exceptionInfo = null;
    public static Action<Exception>? UnhandledExceptionHandler = null;
    public static Func<ZFTask> CreateZFTask = InternalZFTaskHelper.CreateZFTaskOwnedByService;
    public static Action<ZFTask>? OnResultConsumed = InternalZFTaskHelper.ReturnZFTaskToPool;

    private static ZFTask completedTask = CreateCompletedTask();

    private static ZFTask CreateCompletedTask()
    {
        var task = new ZFTask();
        task.state = ZFTaskState.Succeeded;
        task.Id = -1;
        return task;
    }

    private static ZFTask CreateExceptionTask()
    {
        var task = new ZFTask();
        task.state = ZFTaskState.Faulted;
        task.Id = -2;
        return task;
    }

    public static ZFTask CompletedTask => completedTask;


    public static Action<Action>? NextFrameHander;


    public static ZFTask Yield()
    {
        var task = CreateZFTask();
        NextFrame(task);
        return task;
    }

    private static void NextFrame(ZFTask task)
    {
        ArgumentNullException.ThrowIfNull(NextFrameHander);
        NextFrameHander.Invoke(task.SetResult);
    }

    private async ZFTaskVoid AwaitCoroutine()
    {
        try
        {
            await this;
        }
        catch (Exception e)
        {
            UnhandledExceptionHandler?.Invoke(e);
        }
    }


    public void Coroutine()
    {
        ArgumentNullException.ThrowIfNull(UnhandledExceptionHandler);
        AwaitCoroutine().Coroutine();
    }

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

    public void Reset()
    {
        this.state = ZFTaskState.Pending;
        this.action = null;
        this.exceptionInfo = null;
    }

    public ZFTask GetAwaiter()
    {
        return this;
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

    public void GetResult()
    {
        this.CheckResult();
        if (this != CompletedTask)
        {
            OnResultConsumed?.Invoke(this);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void CheckResult()
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
                //throw new OperationCanceledException();
                break;
            default:
                throw new InvalidOperationException("任务状态无效");
        }
    }

    public void SetResult()
    {
        if (this.ValidCompleted())
        {
            throw new InvalidOperationException("Task Already Completed");
        }

        this.state = ZFTaskState.Succeeded;

        var continuation = this.action;
        this.action = null;
        continuation?.Invoke();
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