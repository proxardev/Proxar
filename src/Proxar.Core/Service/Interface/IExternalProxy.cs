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
/// 表示面向外部服务（属不同集群）的代理接口。
/// </summary>
/// <remarks>
/// <para>该接口继承自 <see cref="IServiceProxyBase"/>，专门用于与外部服务进行通信。</para>
/// <para>它定义了静态工厂方法 <see cref="Create(long)"/> 和 <see cref="Create(long, IMessageInvoker)"/>，
/// 用于创建代理实例。这种设计允许实现类灵活控制实例化过程，同时保持接口的抽象性。</para>
/// <para>外部代理通常用于封装对属不同集群的远程服务的调用，隐藏底层通信细节。</para>
/// </remarks>
public interface IExternalProxy : IServiceProxyBase
{
    /// <summary>
    /// 创建一个新的 <see cref="IExternalProxy"/> 实例，使用默认的消息调用器。
    /// </summary>
    /// <param name="serviceId">目标服务的唯一标识符。</param>
    /// <returns>实现了 <see cref="IExternalProxy"/> 的代理实例。</returns>
    /// <remarks>
    /// 此方法使用全局默认的 <see cref="IMessageInvoker"/>（通常由 <see cref="ExternalProxyConfig"/> 提供）。
    /// 适用于大多数标准场景，无需自定义消息行为。
    /// </remarks>
    abstract static IExternalProxy Create(long serviceId);

    /// <summary>
    /// 创建一个新的 <see cref="IExternalProxy"/> 实例，使用指定的自定义消息调用器。
    /// </summary>
    /// <param name="serviceId">目标服务的唯一标识符。</param>
    /// <param name="messageInvoker">自定义的消息调用器，用于发送和接收消息。</param>
    /// <returns>实现了 <see cref="IExternalProxy"/> 的代理实例。</returns>
    /// <remarks>
    /// 使用此方法可以注入特殊的消息处理逻辑（例如，拦截器或测试桩）。
    /// 适用于需要精细控制通信行为的场景。
    /// </remarks>
    abstract static IExternalProxy Create(long serviceId, IMessageInvoker messageInvoker);
}