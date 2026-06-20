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


using System.Buffers;

namespace Proxar.Network;



/// <summary>
/// 定义网络消息的通用行为，包括序列化、反序列化和处理。
/// 所有需要通过网络传输的消息对象都应实现此接口。
/// </summary>
public interface INetMessage : IDisposable
{
    //public byte GetNetMessageType();

    //public void SetNetMessageType(byte netMessageType);


    /// <summary>
    /// 将消息序列化到指定的 <see cref="IBufferWriter{Byte}"/> 中，以便通过网络发送。
    /// </summary>
    /// <param name="writer">用于写入序列化数据的缓冲区写入器。</param>
    public void NetMessageSerialize(IBufferWriter<byte> writer);

    /// <summary>
    /// 将消息序列化为字节数据并返回只读内存块。
    /// </summary>
    /// <returns>包含序列化后数据的 <see cref="ReadOnlyMemory{Byte}"/>。</returns>
    public ReadOnlyMemory<byte> NetMessageSerialize();

    /// <summary>
    /// 从接收到的网络数据中反序列化消息内容。
    /// </summary>
    public void NetMessageDeserialize();

    /// <summary>
    /// 处理接收到的消息，执行与消息类型对应的业务逻辑。
    /// </summary>
    public void NetMessageHandle();
}