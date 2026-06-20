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

using Proxar.CachePool;
using Proxar.CachePool.Interfaces;
using Proxar.Core;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;

namespace Proxar.Tasks;


/// <summary>
/// 异步任务基类，提供状态管理、延续执行、异常处理和池化支持。
/// 非泛型 <see cref="ZFTask"/> 和泛型 <see cref="ZFTask{TRet}"/> 均派生自此基类。
/// </summary>
/// <typeparam name="TSelf">派生类的具体类型，用于实现对象池的类型安全回收。</typeparam>
[ExcludeFromDoc]
public abstract class ZFTaskBase<TSelf> : AbstractPoolable<TSelf>, IResettablePooled
    where TSelf : ZFTaskBase<TSelf>, new()
{
    internal long Id { get; set; }
    private Action? action;
    private ExceptionDispatchInfo? exceptionInfo = null;
    protected ZFTaskState state;

    /// <summary>
    /// 获取一个值，指示任务是否已完成（成功、失败或取消）。
    /// </summary>
    public bool IsCompleted
    {
        get
        {
            return this.ValidCompleted();
        }
    }

    /// <summary>
    /// 获取一个值，指示任务是否因未处理异常而失败。
    /// </summary>
    public bool IsFaulted
    {
        get
        {
            return this.state == ZFTaskState.Faulted;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private bool ValidCompleted()
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

    /// <summary>
    /// 启动此任务的协程执行，自动捕获并上报未处理的异常。
    /// 如果任务已完成，立即检查异常；否则注册完成时的异常回调。
    /// </summary>
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

    /// <inheritdoc/>
    public void Reset()
    {
        this.state = ZFTaskState.Pending;
        this.action = null;
        this.exceptionInfo = null;
        this.OnReset();
    }

    /// <summary>
    /// 派生类可重写此方法，在 <see cref="Reset"/> 时重置自身特有的字段。
    /// </summary>
    protected virtual void OnReset()
    {
    }

    /// <summary>
    /// 注册任务完成时的延续回调。
    /// </summary>
    /// <param name="continuation">要注册的回调。</param>
    public void OnCompleted(Action continuation)
    {
        this.UnsafeOnCompleted(continuation);
    }

    /// <summary>
    /// 注册任务完成时的延续回调（高效路径）。
    /// 如果任务已完成，立即调用回调；否则存储回调，在任务完成时触发。
    /// </summary>
    /// <param name="continuation">要注册的回调。</param>
    public void UnsafeOnCompleted(Action continuation)
    {
        if (this.ValidCompleted())
        {
            continuation?.Invoke();
            return;
        }
        this.action = continuation;
    }

    /// <summary>
    /// 将任务标记为成功完成并触发延续回调。仅供派生类在设置结果后调用。
    /// </summary>
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

    /// <summary>
    /// 检查任务结果状态，如果任务失败则重新抛出异常。
    /// </summary>
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

    /// <summary>
    /// 将任务标记为失败，存储异常信息并触发延续回调。
    /// </summary>
    /// <param name="ex">导致任务失败的异常。</param>
    public void SetException(Exception ex)
    {
        exceptionInfo = ExceptionDispatchInfo.Capture(ex);
        state = ZFTaskState.Faulted;
        var continuation = this.action;
        this.action = null;
        continuation?.Invoke();
    }

    /// <summary>
    /// 将任务标记为已取消，清除所有延续回调。
    /// </summary>
    public void Cancel()
    {
        this.state = ZFTaskState.Canceled;
        this.action = null;
    }
}