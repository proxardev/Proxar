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


using Proxar.AppHost.Interfaces;
using Proxar.ServiceCore.GateWay;
using Proxar.ServiceCore.Interfaces;
using Proxar.ServiceCore.Router;
using Proxar.Tasks;

namespace Proxar.AppHost;


/// <summary>
/// <see cref="IServiceGroup"/> 的默认实现，封装了一个服务组所需的全部消息调用器和启动回调。
/// 每个服务组拥有独立的内部通信、网关转发和外部代理调用器，用于在单进程内模拟多个逻辑隔离的服务集群。
/// </summary>
public class DefaultServiceGroup : IServiceGroup
{
    /// <inheritdoc/>
    public int GroupId { get; }

    /// <inheritdoc/>
    public IMessageInvoker Invoker { get; set; }

    /// <inheritdoc/>
    public IInternalMessageInvoker InternalInvoker { get; set; }

    /// <inheritdoc/>
    public IGateMessageInvoker GateMessageInvoker { get; set; }

    /// <inheritdoc/>
    public IMessageInvoker ExternalProxyInvoker { get; set; }

    /// <inheritdoc/>
    public string Flag { get; set; } = string.Empty;

    private readonly List<Func<long, ZFTask>> startActions = new List<Func<long, ZFTask>>();

    /// <summary>
    /// （内部使用）初始化服务组的新实例，并创建默认的通信组件。
    /// </summary>
    /// <param name="clusterId">服务组的唯一标识符。</param>
    /// <remarks>
    /// 默认创建的组件包括：
    /// <see cref="ServiceRouter"/>（普通消息调用器）、
    /// <see cref="ServiceInternalRouter"/>（内部消息调用器）、
    /// <see cref="GateWay"/>（网关消息调用器）、
    /// <see cref="Server2ExternalProxyMessageInvoker"/>（外部代理调用器）。
    /// </remarks>
    internal DefaultServiceGroup(int clusterId)
    {
        GroupId = clusterId;
        Invoker = new ServiceRouter();
        InternalInvoker = new ServiceInternalRouter();
        GateMessageInvoker = new GateWay();
        ExternalProxyInvoker = new Server2ExternalProxyMessageInvoker();
    }

    /// <inheritdoc/>
    public void AddServiceGroupStartAction(Func<long, ZFTask> func)
    {
        startActions.Add(func);
    }

    /// <inheritdoc/>
    public List<Func<long, ZFTask>> GetServiceGroupStartActions()
    {
        return startActions;
    }
}