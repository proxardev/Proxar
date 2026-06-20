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
/// 表示服务的外部代理实现，用于向不同集群的外部服务发送消息。
/// </summary>
/// <remarks>
/// 该类实现了 <see cref="IExternalProxy"/> 接口，支持通过指定的 <see cref="IMessageInvoker"/> 
/// 与目标服务进行通信。如果未显式提供消息调用器，则会使用全局配置 <see cref="ExternalProxyConfig.MessageInvoker"/>。
/// </remarks>
public partial class ServiceBase_ExternalProxy : IExternalProxy
{

    /// <inheritdoc/>
    public long ServiceId { get; }


    /// <inheritdoc/>
    public IMessageInvoker MessageInvoker => GetMessageInvoker();

    private IMessageInvoker messageInvoker { get; } = null!;

    /// <summary>
    /// 使用指定的服务 ID 初始化 <see cref="ServiceBase_ExternalProxy"/> 的新实例。
    /// </summary>
    /// <param name="serviceId">目标服务的唯一标识符。</param>
    /// <remarks>
    /// 此构造函数将使用全局默认的 <see cref="IMessageInvoker"/>（来自 <see cref="ExternalProxyConfig.MessageInvoker"/>）。
    /// </remarks>
    public ServiceBase_ExternalProxy(long serviceId)
    {
        this.ServiceId = serviceId;
    }

    /// <summary>
    /// 使用指定的服务 ID 和消息调用器初始化 <see cref="ServiceBase_ExternalProxy"/> 的新实例。
    /// </summary>
    /// <param name="serviceId">目标服务的唯一标识符。</param>
    /// <param name="messageInvoker">自定义的消息调用器，用于发送消息。</param>
    public ServiceBase_ExternalProxy(long serviceId, IMessageInvoker messageInvoker) : this(serviceId)
    {
        this.messageInvoker = messageInvoker;
    }

    /// <summary>
    /// 获取当前使用的消息调用器。
    /// </summary>
    /// <returns>
    /// 如果构造函数中提供了 <see cref="IMessageInvoker"/>，则返回该实例；
    /// 否则返回 <see cref="ExternalProxyConfig.MessageInvoker"/>。
    /// </returns>
    private IMessageInvoker GetMessageInvoker()
    {
        if (messageInvoker == null)
        {
            return ExternalProxyConfig.MessageInvoker;
        }
        return messageInvoker;
    }

    /// <inheritdoc/>
    public static IExternalProxy Create(long serviceId)
    {
        return new ServiceBase_ExternalProxy(serviceId);
    }



    /// <inheritdoc/>
    public static IExternalProxy Create(long serviceId, IMessageInvoker messageInvoker)
    {
        return new ServiceBase_ExternalProxy(serviceId, messageInvoker);
    }
}