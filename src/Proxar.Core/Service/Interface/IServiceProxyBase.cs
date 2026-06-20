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


namespace Proxar.ServiceCore.Interfaces;


/// <summary>
/// 定义服务代理的基础契约，提供目标服务标识和消息调用能力。
/// </summary>
/// <remarks>
/// 该接口是服务代理体系中的基础抽象，所有具体代理（如内部代理、外部代理）均应实现此接口。
/// 它统一了访问目标服务 ID 和获取消息调用器的入口，便于上层代码以多态方式处理不同代理类型。
/// </remarks>
public interface IServiceProxyBase
{
    /// <summary>
    /// 获取当前代理所关联的目标服务唯一标识符。
    /// </summary>
    /// <value>
    /// 一个 <see cref="long"/> 值，表示目标服务的 ID。
    /// </value>
    long ServiceId { get; }

    /// <summary>
    /// 获取消息调用器，用于向目标服务发送请求或接收响应。
    /// </summary>
    /// <value>
    /// 实现了 <see cref="IMessageInvoker"/> 接口的实例，负责具体的消息传输逻辑。
    /// </value>
    /// <remarks>
    /// 调用方可通过此属性获取消息调用器，从而执行同步或异步的消息交互。
    /// 具体实现可能返回全局默认调用器，也可能返回代理自身独立维护的调用器实例。
    /// </remarks>
    IMessageInvoker MessageInvoker { get; }

}