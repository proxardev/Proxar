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


using Proxar.ServiceCore.Message;
using System.Buffers;

namespace Proxar.ServiceCore.GateWay;

/// <summary>
/// 表示一个在网关服务中用于消息转发的专用消息。
/// </summary>
/// <remarks>
/// 此消息在构造时即携带已序列化的二进制负载。
/// 通过 <see cref="PayloadServiceMessage"/> 提供的内存缓冲能力，将外部传入的只读内存复制到内部托管内存中，
/// 并标记为已序列化状态，从而在后续网络传输中避免重复序列化。
/// </remarks>
public class GateServiceMessage : PayloadServiceMessage
{
    /// <summary>
    /// 使用指定的只读内存初始化 <see cref="GateServiceMessage"/> 的新实例。
    /// </summary>
    /// <param name="readOnlyMemory">要包装为消息负载的已序列化数据。</param>
    /// <remarks>
    /// 构造函数会将外部传入的只读内存复制到从内存池租用的内部缓冲区中，
    /// 并调用 <see cref="PayloadServiceMessage.SetIsSerializeArgs"/> 标记参数已序列化，
    /// 从而在后续调用 <see cref="PayloadServiceMessage.GetPayloadReadOnlyMemory"/> 或网络发送时直接使用此数据。
    /// </remarks>
    public GateServiceMessage(ReadOnlyMemory<byte> readOnlyMemory) : base(CreateMemory(readOnlyMemory), readOnlyMemory.Length)
    {
        this.SetIsSerializeArgs();
    }

    private static IMemoryOwner<byte> CreateMemory(ReadOnlyMemory<byte> readOnlyMemory)
    {
        var memoryOwner = MemoryPool<byte>.Shared.Rent(readOnlyMemory.Length);
        readOnlyMemory.CopyTo(memoryOwner.Memory);
        return memoryOwner;
    }

    /// <summary>
    /// 此消息类型不支持参数序列化。数据已在构造时提供。
    /// </summary>
    /// <param name="writer">缓冲区写入器（未使用）。</param>
    /// <exception cref="NotImplementedException">此方法在此消息类型中不支持。</exception>
    public override void SerializeArgs(IBufferWriter<byte> writer)
    {
        throw new NotImplementedException();
    }
}