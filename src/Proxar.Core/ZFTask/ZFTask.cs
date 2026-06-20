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

/// <summary>
/// 表示一个无返回值的可等待异步操作。使用自定义异步方法构建器 <see cref="AsyncZFTaskMethodBuilder"/> 支持 <see langword="async"/> 方法。
/// 继承自 <see cref="ZFTaskBase{ZFTask}"/>，提供对象池回收和延续执行能力。
/// </summary>
[AsyncMethodBuilder(typeof(AsyncZFTaskMethodBuilder))]
public sealed class ZFTask : ZFTaskBase<ZFTask>, ICriticalNotifyCompletion, ITask
{
    /// <summary>
    /// 用于创建 <see cref="ZFTask"/> 实例的工厂方法。默认使用对象池并由当前服务上下文管理生命周期。
    /// </summary>
    public static Func<ZFTask> CreateZFTask { get; set; } = InternalZFTaskHelper.CreateZFTaskOwnedByService;

    /// <summary>
    /// 在任务结果被消费后执行的回调，通常用于将任务实例归还对象池。
    /// </summary>
    public static Action<ZFTask>? OnResultConsumed { get; set; } = InternalZFTaskHelper.ReturnZFTaskToPool;

    private static ZFTask completedTask = CreateCompletedTask();

    private static ZFTask CreateCompletedTask()
    {
        var task = new ZFTask();
        task.state = ZFTaskState.Succeeded;
        task.Id = -1;
        return task;
    }

    /// <summary>
    /// 获取一个已成功完成的 <see cref="ZFTask"/> 实例，供需要返回已完成任务的方法使用。
    /// </summary>
    public static ZFTask CompletedTask => completedTask;

    /// <summary>
    /// 用于调度下一帧延续的处理程序。由框架在初始化时注入。
    /// </summary>
    public static Action<Action>? NextFrameHander;

    /// <summary>
    /// 创建一个在下一帧完成的 <see cref="ZFTask"/>，用于异步等待当前帧结束。
    /// </summary>
    /// <returns>一个将在下一帧完成的 <see cref="ZFTask"/>。</returns>
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

    /// <summary>
    /// 获取此任务的 awaiter，支持 <see langword="await"/> 关键字。
    /// </summary>
    /// <returns>当前实例，即自身的 awaiter。</returns>
    public ZFTask GetAwaiter()
    {
        return this;
    }

    /// <summary>
    /// 检查任务结果状态，如果任务失败则重新抛出异常。成功后触发 <see cref="OnResultConsumed"/> 回调。
    /// </summary>
    public void GetResult()
    {
        this.CheckResult();
        if (this != CompletedTask)
        {
            OnResultConsumed?.Invoke(this);
        }
    }

    /// <summary>
    /// 将任务标记为成功完成并触发延续回调。
    /// </summary>
    public void SetResult()
    {
        this.SetResultCore();
    }

    /// <inheritdoc/>
    public override void OnRented()
    {
        this.Id = ZFTaskInt64IdGeneratorActorSingleton.Current.NewId();
    }
}