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


using Proxar.ServiceCore.Message;
using Proxar.Tasks;
namespace Proxar.ServiceCore;



/// <summary>
/// 提供 <see cref="ZFTask"/> 的扩展方法。
/// </summary>
public static class ZFTaskExtensions
{
    /// <summary>
    /// 自动将 <see cref="ZFTask{T}"/> 的结果或异常以 RPC 回调形式发送给指定服务。
    /// </summary>
    /// <typeparam name="T">任务结果的类型。</typeparam>
    /// <param name="task">要等待并自动响应的任务。</param>
    /// <param name="serviceId">目标服务的唯一标识符。</param>
    /// <param name="msgIdx">消息索引，用于匹配请求和响应。</param>
    /// <returns>一个 <see cref="ZFTask"/>，表示整个操作（包括发送回调）的异步过程。</returns>
    /// <remarks>
    /// 此方法会等待目标 <paramref name="task"/> 完成：
    /// <list type="bullet">
    /// <item><description>如果任务成功完成，结果将封装在 <see cref="ProtoBase.RpcCallBack"/> 消息中发送。</description></item>
    /// <item><description>如果任务引发异常，异常消息将封装在 <see cref="ProtoBase.RpcCallbackError"/> 消息中发送，并且异常会被重新抛出。</description></item>
    /// </list>
    /// </remarks>
    /// <exception cref="Exception">当 <paramref name="task"/> 失败时，原始异常会在此方法中重新抛出。</exception>

    public static async ZFTask AutoResponse<T>(this ZFTask<T> task, long serviceId, long msgIdx)
    {
        try
        {
            var result = await task;
            var header = new MessageHeader(msgIdx);
            Service.Send(serviceId, ProtoBase.RpcCallBack, result, header: header);
        }
        catch (Exception exception)
        {
            var header = new MessageHeader(msgIdx);
            Service.Send(serviceId, ProtoBase.RpcCallbackError, exception.Message, header: header);
            throw;
        }
    }
}