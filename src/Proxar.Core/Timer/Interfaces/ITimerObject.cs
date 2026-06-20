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


using Proxar.Tasks;

namespace Proxar.Timer.Interfaces;

/// <summary>
/// 提供定时器的创建、取消和管理功能。
/// </summary>
/// <remarks>
/// 该接口允许对象注册一次性或循环定时回调，并支持基于 <see cref="ZFTask"/> 的异步延迟等待。
/// </remarks>
public interface ITimerObject
{
    /// <summary>
    /// 在指定的延迟后执行一次回调操作。
    /// </summary>
    /// <param name="milliSecond">延迟时间，单位毫秒。</param>
    /// <param name="action">要执行的回调委托。</param>
    /// <returns>定时器的唯一标识符，可用于后续取消操作。</returns>
    public long TimerCall(long milliSecond, Action action);

    /// <summary>
    /// 在指定的延迟后执行一次带参数的回调操作。
    /// </summary>
    /// <typeparam name="T">回调参数的类型。</typeparam>
    /// <param name="milliSecond">延迟时间，单位毫秒。</param>
    /// <param name="action">要执行的回调委托，参数类型为 <typeparamref name="T"/>。</param>
    /// <param name="args">传递给回调委托的参数。</param>
    /// <returns>定时器的唯一标识符，可用于后续取消操作。</returns>
    public long TimerCall<T>(long milliSecond, Action<T> action, T args);

    /// <summary>
    /// 以指定的间隔重复执行回调操作，直到取消为止。
    /// </summary>
    /// <param name="milliSecond">间隔时间，单位毫秒。</param>
    /// <param name="action">要执行的回调委托。</param>
    /// <returns>定时器的唯一标识符，可用于后续取消操作。</returns>
    public long IntervalTimerCall(long milliSecond, Action action);

    /// <summary>
    /// 以指定的间隔重复执行带参数的回调操作，直到取消为止。
    /// </summary>
    /// <typeparam name="T">回调参数的类型。</typeparam>
    /// <param name="milliSecond">间隔时间，单位毫秒。</param>
    /// <param name="action">要执行的回调委托，参数类型为 <typeparamref name="T"/>。</param>
    /// <param name="args">传递给回调委托的参数。</param>
    /// <returns>定时器的唯一标识符，可用于后续取消操作。</returns>
    public long IntervalTimerCall<T>(long milliSecond, Action<T> action, T args);

    /// <summary>
    /// 创建一个异步延迟任务，在指定的时间后完成。
    /// </summary>
    /// <param name="milliSecond">延迟时间，单位毫秒。</param>
    /// <returns>一个 <see cref="ZFTask"/>，在延迟结束后完成。</returns>
    /// <remarks>
    /// 该 <see cref="ZFTask"/> 不受外部取消影响，需自行管理超时逻辑。
    /// 通常用于 <c>await</c> 语法，例如 <c>await Delay(1000);</c>。
    /// </remarks>
    public ZFTask Delay(long milliSecond);

    /// <summary>
    /// 取消指定标识符的定时器。
    /// </summary>
    /// <param name="id">由 <see cref="TimerCall"/> 或 <see cref="IntervalTimerCall"/> 返回的定时器标识符。</param>
    public void CancelTimer(long id);

    /// <summary>
    /// 取消当前对象注册的所有定时器。
    /// </summary>
    public void CancelAllTimer();

    /// <summary>
    /// 检查指定标识符的定时器是否仍然处于活动状态。
    /// </summary>
    /// <param name="id">定时器标识符。</param>
    /// <returns>如果定时器存在且尚未触发或取消，则为 <c>true</c>；否则为 <c>false</c>。</returns>
    public bool HasTimer(long id);

}