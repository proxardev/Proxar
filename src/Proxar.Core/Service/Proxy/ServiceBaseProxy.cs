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


using Proxar.ServiceCore.Interfaces;

namespace Proxar.ServiceCore;

/// <summary>
/// 所有服务代理的基类，提供 <see cref="ServiceId"/> 和 <see cref="IMessageInvoker"/> 的基本实现。
/// </summary>
/// <remarks>
/// 源生成器会为每个服务生成一个继承自此类的具体代理类型。
/// </remarks>
public partial class ServiceBaseProxy : IServiceProxy
{

    /// <inheritdoc/>
    public long ServiceId { get; }

    /// <inheritdoc/>
    public IMessageInvoker MessageInvoker => messageInvoker;

    private IMessageInvoker messageInvoker { get; } = null!;

    /// <summary>
    /// 使用指定的服务 ID 初始化 <see cref="ServiceBaseProxy"/> 的新实例。
    /// 消息发送器将使用 <see cref="Service.MessageInvoker"/> 的默认实例。
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <param name="serviceId">目标服务的唯一标识符。</param>
    public ServiceBaseProxy(long serviceId)
    {
        this.ServiceId = serviceId;
    }

    /// <summary>
    /// 使用指定的服务 ID 和自定义 <see cref="IMessageInvoker"/> 初始化 <see cref="ServiceBaseProxy"/> 的新实例。
    /// </summary>
    /// <param name="serviceId">目标服务的唯一标识符。</param>
    /// <param name="messageInvoker">用于发送消息的自定义调用器。</param>
    public ServiceBaseProxy(long serviceId, IMessageInvoker messageInvoker) : this(serviceId)
    {
        this.messageInvoker = messageInvoker;
    }

    /// <inheritdoc/>
    public long GetServiceId()
    {
        return this.ServiceId;
    }

    /// <inheritdoc/>
    public static IServiceProxy Create(long serviceId)
    {
        return new ServiceBaseProxy(serviceId);
    }

    /// <inheritdoc/>
    public static IServiceProxy Create(long serviceId, IMessageInvoker messageInvoker)
    {
        throw new NotImplementedException();
    }
}