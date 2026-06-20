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


using MessagePack;
using Proxar.Network;
using System.Buffers;
namespace Proxar.ServiceCore.Message;

/// <summary>
/// 从网络接收的原始字节构造的服务消息，负责解析消息头并将消息投递到目标服务的消息队列。
/// </summary>
/// <remarks>
/// 此消息类型用于内部服务或客户端发送的网络消息。
/// 它直接从字节数组读取，通过 <see cref="NetMessageDeserialize"/> 解析头部信息，并通过 <see cref="NetMessageHandle"/> 将消息推送到目标服务的队列。
/// </remarks>
public class ServiceMessageForNet : AbstractServiceMessage, INetMessage
{
    private ReadOnlySequence<byte> readOnlySequence;
    private int readOffset = 0;
    private byte[] bytes = null!;

    /// <summary>
    /// 使用指定的字节数组初始化 <see cref="ServiceMessageForNet"/> 的新实例。
    /// </summary>
    /// <param name="bytes">从网络接收的原始消息字节。</param>
    public ServiceMessageForNet(byte[] bytes)
    {
        this.bytes = bytes;
        this.readOffset = 0;
        this.readOnlySequence = new ReadOnlySequence<byte>(this.bytes);
    }

    /// <inheritdoc/>
    public override MessagePackReader GetMessagePackReader()
    {
        MessagePackReader reader;
        if (readOffset == 0)
        {
            reader = new MessagePackReader(readOnlySequence);
            return reader;
        }
        var seq = readOnlySequence.Slice(readOffset);
        reader = new MessagePackReader(seq);
        return reader;
    }

    /// <inheritdoc/>
    public override IBufferWriter<byte> GetSerializeArgeWriter()
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public ReadOnlyMemory<byte> NetMessageSerialize()
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public void NetMessageDeserialize()
    {
        var reader = this.GetMessagePackReader();

        var toServiceId = reader.ReadInt64();
        var (serviceId, msgSeq, proto) = Service.ReadHeander(ref reader);
        this.SetHeadData(serviceId, toServiceId, msgSeq, proto);
        readOffset = (int)reader.Consumed;
    }

    /// <inheritdoc/>
    public override void SerializeArgs(IBufferWriter<byte> writer)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public void NetMessageHandle()
    {
        this.PushMessage();
    }

    /// <summary>
    /// 根据 <see cref="AbstractServiceMessage.GetToServiceId"/> 获取目标服务，并将当前消息推送到该服务的队列。
    /// </summary>
    private void PushMessage()
    {
        var toServiceId = this.GetToServiceId();
        var service = ServiceManager.Instance.GetService(toServiceId);
        if (service == null)
        {
            return;
        }
        service.PushMessage(this);
    }

    /// <inheritdoc/>
    public void NetMessageSerialize(IBufferWriter<byte> writer)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public override ReadOnlyMemory<byte> GetPayloadReadOnlyMemory()
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    protected override void OnConsumed(long consumed)
    {
        readOffset += (int)consumed;
    }
}