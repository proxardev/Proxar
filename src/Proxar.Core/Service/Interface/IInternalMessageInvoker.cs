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
/// 定义内部消息调用器，提供优先级队列投递和原始字节发送能力。
/// 用于框架内部服务间的高性能通信，支持跳过序列化的零拷贝转发和优先队列处理。
/// </summary>
public interface IInternalMessageInvoker
{
    /// <summary>
    /// 将消息投递到目标服务的优先队列（Queue0），优先于普通队列处理。
    /// </summary>
    /// <param name="ProxyId">网关代理标识，用于外部消息路由。</param>
    /// <param name="serviceId">目标服务的唯一标识符。</param>
    /// <param name="serviceMessage">要投递的服务消息。</param>
    void PushQueue0Message(long ProxyId, long serviceId, IServiceMessage serviceMessage);

    /// <summary>
    /// 发送原始消息负载，不对参数进行序列化。
    /// </summary>
    /// <param name="ProxyId">网关代理标识，用于外部消息路由。</param>
    /// <param name="serviceId">目标服务的唯一标识符。</param>
    /// <param name="serviceMessage">要发送的服务消息，其负载已为原始字节。</param>
    void SendRaw(long ProxyId, long serviceId, IServiceMessage serviceMessage);

    /// <summary>
    /// 将原始消息投递到目标服务的优先队列。
    /// </summary>
    /// <param name="ProxyId">网关代理标识，用于外部消息路由。</param>
    /// <param name="serviceId">目标服务的唯一标识符。</param>
    /// <param name="serviceMessage">要投递的服务消息，其负载已为原始字节。</param>
    void PushQueue0MessageRaw(long ProxyId, long serviceId, IServiceMessage serviceMessage);
}