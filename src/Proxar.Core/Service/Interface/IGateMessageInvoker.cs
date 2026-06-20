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


using Proxar.Network.Interfaces;

namespace Proxar.ServiceCore.Interfaces;

/// <summary>
/// 定义网关消息调用器，管理客户端通道的注册与移除、目标到通道的路由映射、
/// 客户端数据的接收以及服务端消息向客户端的转发。
/// </summary>
public interface IGateMessageInvoker
{
    /// <summary>
    /// 注册一个客户端通信通道。
    /// </summary>
    /// <param name="channel">客户端连接通道。</param>
    void AddChannel(IChannel channel);

    /// <summary>
    /// 移除指定的客户端通信通道。
    /// </summary>
    /// <param name="channel">要移除的客户端通道。</param>
    void RemoveChannel(IChannel channel);

    /// <summary>
    /// 建立目标标识（如 roleId）到通道的映射关系，用于后续消息转发。
    /// </summary>
    /// <param name="targetId">目标标识，通常为 roleId 或 playerId。</param>
    /// <param name="channelId">通道唯一标识。</param>
    void SetTarget2Channel(long targetId, long channelId);

    /// <summary>
    /// 移除指定目标标识的通道映射。
    /// </summary>
    /// <param name="targetId">目标标识。</param>
    void RemoveTarget2Channel(long targetId);

    /// <summary>
    /// 建立目标标识与内部服务间的类型路由映射。
    /// </summary>
    /// <param name="targetId">目标标识，通常为 roleId。</param>
    /// <param name="type">代理类型标识（ProxyId）。</param>
    /// <param name="serviceId">内部目标服务的唯一标识符。</param>
    void SetTargetServiceMapping(long targetId, long type, long serviceId);

    /// <summary>
    /// 接收来自客户端通道的数据，根据路由信息解析并路由到内部服务。
    /// </summary>
    /// <param name="channel">数据来源的客户端通道。</param>
    /// <param name="data">接收到的原始字节数据。</param>
    void ReceiveChannelData(IChannel channel, byte[] data);

    /// <summary>
    /// 将来自内部服务的消息转发到指定目标客户端。
    /// </summary>
    /// <param name="fromServiceId">消息来源服务的唯一标识符。</param>
    /// <param name="targetId">目标客户端标识（通常为 roleId）。</param>
    /// <param name="proxyId">外部代理标识，用于客户端反序列化路由。</param>
    /// <param name="rpcId">RPC 调用标识，用于请求-响应匹配。</param>
    /// <param name="proto">协议方法编号。</param>
    /// <param name="payloadBytes">消息负载的原始字节。</param>
    void ForwardServiceMessage2Client(long fromServiceId, long targetId, long proxyId, long rpcId, int proto, byte[] payloadBytes);
}