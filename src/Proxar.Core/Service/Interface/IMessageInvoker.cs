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
/// 定义消息调用器，用于向目标服务发送消息。
/// 通过不同的实现（如本地总线、网络传输）解耦消息发送方式，支持单元测试和透明扩展。
/// </summary>
public interface IMessageInvoker
{
    /// <summary>
    /// 向指定服务发送消息。
    /// </summary>
    /// <param name="ProxyId">网关代理标识，用于路由外部消息；内部通信时可传 0。</param>
    /// <param name="serviceId">目标服务的唯一标识符。</param>
    /// <param name="serviceMessage">要发送的服务消息。</param>
    void Send(long ProxyId, long serviceId, IServiceMessage serviceMessage);

    /// <summary>
    /// 将消息直接投递到本地进程内的目标服务，不经过网络传输。
    /// </summary>
    /// <param name="ProxyId">网关代理标识，内部通信时可传 0。</param>
    /// <param name="serviceId">目标服务的唯一标识符。</param>
    /// <param name="serviceMessage">要发送的服务消息。</param>
    void SendLocal(long ProxyId, long serviceId, IServiceMessage serviceMessage);
}