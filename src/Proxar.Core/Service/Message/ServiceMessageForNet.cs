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


public class ServiceMessageForNet : AbstractServiceMessage, INetMessage
{
    private ReadOnlySequence<byte> readOnlySequence;
    private int readOffset = 0;
    private byte[] bytes = null!;

    public ServiceMessageForNet(byte[] bytes)
    {
        this.bytes = bytes;
        this.readOffset = 0;
        this.readOnlySequence = new ReadOnlySequence<byte>(bytes);
    }

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

    public override IBufferWriter<byte> GetSerializeArgeWriter()
    {
        throw new NotImplementedException();
    }

    public ReadOnlyMemory<byte> NetMessageSerialize()
    {
        throw new NotImplementedException();
    }

    public void NetMessageDeserialize()
    {
        var reader = this.GetMessagePackReader();

        var toServiceId = reader.ReadInt64();
        var (serviceId, msgSeq, proto) = Service.ReadHeander(ref reader);
        this.SetHeadData(serviceId, toServiceId, msgSeq, proto);
        readOffset = (int)reader.Consumed;
    }

    public override void SerializeArgs(IBufferWriter<byte> writer)
    {
        throw new NotImplementedException();
    }

    public void NetMessageHandle()
    {
        //Service.MessageInvoker.SendLocal(0, toServiceId, this);
        this.PushMessage();
    }

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

    public void NetMessageSerialize(IBufferWriter<byte> writer)
    {
        throw new NotImplementedException();
    }

    public override ReadOnlyMemory<byte> GetPayloadReadOnlyMemory()
    {
        throw new NotImplementedException();
    }

    public override void OnConsumed(long consumed)
    {
        readOffset += (int)consumed;
    }
}