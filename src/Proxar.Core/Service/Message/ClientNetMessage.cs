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


namespace Proxar.ServiceCore.Message;

/// <summary>
/// 表示一个来自客户端的网络消息，主要用于在 Gateway 和内部服务之间转发时重定向消息头字段。
/// </summary>
/// <remarks>
/// 当外部客户端将消息发送到 Gateway 后，Gateway 会使用此类重新设置目标服务、源服务和序列号，然后再转发给实际的业务服务。
/// 基类 <see cref="ServiceMessageForNet"/> 已提供完整的网络消息解析能力；此处额外提供了重定向方法和被隐藏的 <see cref="NetMessageHandle"/> 实现。
/// </remarks>
internal class ClientNetMessage : ServiceMessageForNet
{
    /// <summary>
    /// 使用指定的原始字节数据初始化 <see cref="ClientNetMessage"/> 的新实例。
    /// </summary>
    /// <param name="bytes">从网络接收的原始字节数组。</param>
    public ClientNetMessage(byte[] bytes) : base(bytes)
    {
    }

    /// <summary>
    /// 隐藏基类的处理方法。在 Gateway 转发场景下，消息的实际处理由 Gateway 负责，此方法不执行任何操作。
    /// </summary>
    public new void NetMessageHandle()
    {
    }

    /// <summary>
    /// 重定向目标服务 ID（将消息发往新的目标服务）。
    /// </summary>
    /// <param name="serviceId">新的目标服务唯一标识符。</param>
    public void ReDirectToServiceId(long serviceId)
    {
        this.toServiceId = serviceId;
    }

    /// <summary>
    /// 重定向源服务 ID（修改消息的发送方标识）。
    /// </summary>
    /// <param name="serviceId">新的源服务唯一标识符。</param>
    public void ReDirectFromServiceId(long serviceId)
    {
        this.ServiceId = serviceId;
    }

    /// <summary>
    /// 重定向消息序列号（MsgSeq），用于请求-响应匹配。
    /// </summary>
    /// <param name="msgSeq">新的消息序列号。</param>
    public void ReDirectMsgSeq(long msgSeq)
    {
        this.msgSeq = msgSeq;
    }
}