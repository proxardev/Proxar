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


using Proxar.Logging;

namespace Proxar.Tasks;


/// <summary>
/// 提供用于 <see cref="ZFTask"/> 的辅助扩展方法。
/// </summary>
public static class ZFTaskHelper
{
    /// <summary>
    /// 按顺序等待列表里所有指定 <see cref="ZFTask{T}"/> 完成，并收集每个任务的结果。
    /// </summary>
    /// <typeparam name="T">任务结果的类型。</typeparam>
    /// <param name="taskList">要等待的 <see cref="ZFTask{T}"/> 列表。</param>
    /// <returns>
    /// 一个代表所有提供任务完成的任务，其结果是一个 <see cref="List{T}"/>，包含每个任务的结果，顺序与输入列表一致。
    /// </returns>
    /// <remarks>
    /// 此方法会按顺序依次等待每个任务，而不是并发等待。如果任一任务失败（返回 <see cref="ZFTaskState.Faulted"/>），
    /// 则整个方法将提前终止并抛出相应的异常（取决于 <see cref="ZFTask"/> 的异常传播行为）。
    /// </remarks>
    /// <example>
    /// <code>
    /// var tasks = new List&lt;ZFTask&lt;int&gt;&gt; { GetUserCountAsync(), GetOrderCountAsync() };
    /// var results = await ZFTaskHelper.WhenAll(tasks);
    /// // results[0] 是用户数，results[1] 是订单数
    /// </code>
    /// </example>
    public static async ZFTask<List<T>> WhenAll<T>(List<ZFTask<T>> taskList)
    {
        var resList = new List<T>();
        foreach (var task in taskList)
        {
            var res = await task;
            resList.Add(res);
        }
        return resList;
    }

    internal static async ZFTask WhenAll2<T>(List<ZFTask<T>> taskList)
    {
        var resList = new List<T>();
        foreach (var task in taskList)
        {
            var res = await task;
            resList.Add(res);
        }
    }

    internal static void SafeSetResult<T>(this List<ZFTask<T>> taskList, T value)
    {
        if (taskList == null)
        {
            return;
        }
        foreach (var task in taskList)
        {
            try
            {
                task.SetResult(value);
            }
            catch (Exception e)
            {
                ProxarLogger.Error(e);
            }
        }
    }

}