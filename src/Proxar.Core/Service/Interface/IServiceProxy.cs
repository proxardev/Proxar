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
/// 表示内部服务代理的接口，用于在当前服务边界内进行通信。
/// </summary>
/// <remarks>
/// <para>该接口继承自 <see cref="IServiceProxyBase"/>，并添加了静态工厂方法，用于创建代理实例。</para>
/// <para>内部代理通常用于同一集群内不同服务之间的消息传递。</para>
/// <para>此类代理的创建可通过 <see cref="Create(long)"/> 或 <see cref="Create(long, IMessageInvoker)"/> 完成。</para>
/// </remarks>
public interface IServiceProxy : IServiceProxyBase
{
    /// <summary>
    /// 创建一个新的 <see cref="IServiceProxy"/> 实例，使用默认的消息调用器。
    /// </summary>
    /// <param name="serviceId">目标服务的唯一标识符。</param>
    /// <returns>实现了 <see cref="IServiceProxy"/> 的代理实例。</returns>
    /// <remarks>
    /// 此方法将使用全局默认的 <see cref="IMessageInvoker"/> 进行消息发送。
    /// 适用于大多数内部通信场景。
    /// </remarks>
    abstract static IServiceProxy Create(long serviceId);

    /// <summary>
    /// 创建一个新的 <see cref="IServiceProxy"/> 实例，使用指定的自定义消息调用器。
    /// </summary>
    /// <param name="serviceId">目标服务的唯一标识符。</param>
    /// <param name="messageInvoker">自定义的消息调用器，可注入特殊处理逻辑。</param>
    /// <returns>实现了 <see cref="IServiceProxy"/> 的代理实例。</returns>
    /// <remarks>
    /// 使用此方法可以在创建代理时定制消息传输行为（例如模拟测试环境等）。
    /// </remarks>
    abstract static IServiceProxy Create(long serviceId, IMessageInvoker messageInvoker);
}