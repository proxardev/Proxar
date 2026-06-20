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


using Proxar.AppHost;
using Proxar.ServiceCore.Interfaces;

namespace Proxar.ServiceCore.GateWay;

/// <summary>
/// 向外部系统或集群发送消息的消息调用器。
/// 负责将内部服务的消息通过网关转发到目标客户端。
/// </summary>
public class Server2ExternalProxyMessageInvoker : IMessageInvoker
{
    /// <summary>
    /// 将服务消息转发到目标客户端。
    /// 从 <paramref name="serviceMessage"/> 中提取 RPC 序列号、协议编号和已序列化的负载，
    /// 通过 <see cref="IGateMessageInvoker.ForwardServiceMessage2Client"/> 完成最终转发，转发后立即释放消息。
    /// </summary>
    /// <param name="proxyId">外部代理标识，用于客户端反序列化路由。</param>
    /// <param name="targetId">目标客户端的唯一标识（如 roleId）。</param>
    /// <param name="serviceMessage">包含完整头信息和序列化负载的服务消息。</param>
    public void Send(long proxyId, long targetId, IServiceMessage serviceMessage)
    {
        var rpcId = serviceMessage.GetSeq();
        var proto = serviceMessage.GetProto();
        var payload = serviceMessage.GetPayloadReadOnlyMemory();

        var fromServiceId = serviceMessage.GetFromServiceId();

        ProxarHost.Instance.GateMessageInvoker.ForwardServiceMessage2Client(fromServiceId, targetId, proxyId, rpcId, proto, payload.ToArray());
        serviceMessage.Dispose();
    }

    /// <summary>
    /// 此调用器不支持本地发送。
    /// </summary>
    /// <exception cref="NotImplementedException">始终抛出。</exception>
    public void SendLocal(long ProxyId, long targetId, IServiceMessage serviceMessage)
    {
        throw new NotImplementedException();
    }
}