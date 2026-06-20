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


using Proxar.ServiceCore;
using Proxar.ServiceCore.Interfaces;
using Proxar.Tasks;

namespace Proxar.AppHost.Interfaces;

/// <summary>
/// 定义服务组，管理一组共享相同通信基础设施的服务实例。
/// 每个服务组拥有独立的内部通信、网关转发和外部代理所需的各类消息调用器，
/// 并支持以组级别环境启动执行回调，允许在单进程内模拟多个逻辑上隔离的服务集群。
/// </summary>
public interface IServiceGroup
{
    /// <summary>
    /// 服务组的唯一标识符。
    /// </summary>
    int GroupId { get; }

    /// <summary>
    /// 获取或设置组内服务间通信使用的消息调用器，用于普通消息的发送与接收。
    /// </summary>
    IMessageInvoker Invoker { get; set; }

    /// <summary>
    /// 获取或设置内部消息调用器，支持优先级队列投递和原始字节发送，
    /// 用于框架内部服务间的高性能通信。
    /// </summary>
    IInternalMessageInvoker InternalInvoker { get; set; }

    /// <summary>
    /// 获取或设置网关消息调用器，管理客户端通道的注册、路由映射以及消息的接收与转发，
    /// 用于外部客户端与内部服务之间的通信。
    /// </summary>
    IGateMessageInvoker GateMessageInvoker { get; set; }

    /// <summary>
    /// 获取或设置外部代理消息调用器，用于外部代理（ExternalProxy）向其他服务组发起跨组调用。
    /// </summary>
    IMessageInvoker ExternalProxyInvoker { get; set; }

    /// <summary>
    /// 获取或设置服务组的标志，通常用于区分服务组类型（如 "Server" 或 "Client"）。
    /// </summary>
    string Flag { get; set; }

    /// <summary>
    /// 注册服务组启动时执行的回调。所有注册的回调将在该组的每个服务启动后依次执行。
    /// </summary>
    /// <param name="func">启动回调，参数为当前服务 ID，返回一个 <see cref="ZFTask"/>。</param>
    void AddServiceGroupStartAction(Func<long, ZFTask> func);

    /// <summary>
    /// 获取所有已注册的服务组启动回调。
    /// </summary>
    /// <returns>启动回调列表。</returns>
    List<Func<long, ZFTask>> GetServiceGroupStartActions();

    /// <summary>
    /// 在当前服务组上下文中执行指定操作，确保操作执行期间 <see cref="ActorThreadScope"/> 中的服务组引用正确。
    /// 操作完成后自动恢复原有的服务组上下文，即使发生异常也会恢复。
    /// </summary>
    /// <param name="action">要执行的操作。</param>
    void ServiceGroupExecute(Action action)
    {
        var group = ActorThreadScope.ServiceGroup;
        try
        {
            ActorThreadScope.ThreadServiceGroup = this;
            action.Invoke();
        }
        catch (Exception)
        {
            ActorThreadScope.ThreadServiceGroup = group;
            throw;
        }
        finally
        {
            ActorThreadScope.ThreadServiceGroup = group;
        }
    }
}