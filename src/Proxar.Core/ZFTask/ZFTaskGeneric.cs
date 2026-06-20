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
/// 表示一个可等待的异步操作，返回类型为 <typeparamref name="TRet"/>。
/// 使用自定义异步方法构建器 <see cref="AsyncZFTaskGenericMethodBuilder{TRet}"/> 支持 <see langword="async"/> 方法。
/// 继承自 <see cref="ZFTaskBase{TSelf}"/>，提供结果存储和对象池回收能力。
/// </summary>
/// <typeparam name="TRet">异步操作的返回值类型。</typeparam>
[AsyncMethodBuilder(typeof(AsyncZFTaskGenericMethodBuilder<>))]
public sealed class ZFTask<TRet> : ZFTaskBase<ZFTask<TRet>>, ICriticalNotifyCompletion, ITask
{
    private TRet? value;
    internal static readonly ZFTask<TRet> DefaultZFTaskResult = CreateCompletedTask();

    /// <summary>
    /// 获取或设置用于创建 <see cref="ZFTask{TRet}"/> 实例的工厂方法。
    /// 默认使用对象池创建并由当前服务上下文管理生命周期。
    /// </summary>
    public static Func<ZFTask<TRet>> CreateZFTask { get; set; } = InternalZFTaskHelper.CreateZFTaskOwnedByService<TRet>;

    /// <summary>
    /// 获取或设置在任务结果被消费后执行的回调，通常用于将任务实例归还对象池。
    /// </summary>
    public static Action<ZFTask<TRet>>? OnResultConsumed { get; set; } = InternalZFTaskHelper.ReturnZFTaskToPool<TRet>;

    /// <summary>
    /// 初始化 <see cref="ZFTask{TRet}"/> 的新实例，并生成唯一标识符。
    /// </summary>
    public ZFTask()
    {
        this.Id = ZFTaskInt64IdGeneratorActorSingleton.Current.NewId();
    }

    private static ZFTask<TRet> CreateCompletedTask()
    {
        var task = new ZFTask<TRet>();
        task.state = ZFTaskState.Succeeded;
        task.value = default(TRet);
        return task;
    }

    /// <summary>
    /// 创建一个已成功完成并包含指定结果的 <see cref="ZFTask{TResult}"/> 实例。
    /// </summary>
    /// <typeparam name="TResult">结果的类型。</typeparam>
    /// <param name="result">要封装的结果值。</param>
    /// <returns>一个已完成的 <see cref="ZFTask{TResult}"/>。</returns>
    public static ZFTask<TResult> FromResult<TResult>(TResult result)
    {
        var task = ZFTask<TResult>.CreateZFTask();
        task.value = result;
        task.state = ZFTaskState.Succeeded;
        return task;
    }

    /// <summary>
    /// 获取此任务的 awaiter，支持 <see langword="await"/> 关键字。
    /// </summary>
    /// <returns>当前实例，即自身的 awaiter。</returns>
    public ZFTask<TRet> GetAwaiter()
    {
        return this;
    }

    /// <summary>
    /// 获取异步操作的返回值，如果任务未成功完成则抛出异常。
    /// 调用后会触发 <see cref="OnResultConsumed"/> 回调以便回收对象。
    /// </summary>
    /// <returns>异步操作的结果。</returns>
    public TRet GetResult()
    {
        this.CheckResult();
        var value = this.value!;
        OnResultConsumed?.Invoke(this);
        return value;
    }

    /// <summary>
    /// 设置任务的结果并将任务标记为成功完成。
    /// </summary>
    /// <param name="result">要设置的结果值。</param>
    public void SetResult(TRet result)
    {
        this.value = result;
        this.SetResultCore();
    }

    /// <summary>
    /// 在对象池重置时调用，清除结果值。
    /// </summary>
    protected override void OnReset()
    {
        this.value = default(TRet);
    }

    /// <summary>
    /// 在对象从池中租用时调用，生成新的唯一标识符。
    /// </summary>
    public override void OnRented()
    {
        this.Id = ZFTaskInt64IdGeneratorActorSingleton.Current.NewId();
    }
}