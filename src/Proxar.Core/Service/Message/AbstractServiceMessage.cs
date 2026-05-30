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
using Proxar.ServiceCore.Interfaces;
using System.Buffers;
namespace Proxar.ServiceCore.Message;


public abstract class AbstractServiceMessage : IServiceMessage
{
    protected long toServiceId = 0;
    protected byte messageType = 0;
    protected long ServiceId = 0;
    protected long msgSeq = 0;
    protected int Proto = 0;
    protected long headerData = 0;

    public void SetHeadData(long serviceId, long toServiceId, long msgSeq, int proto,
        IMessageHeader? header = null)
    {
        this.ServiceId = serviceId;
        this.toServiceId = toServiceId;
        this.msgSeq = msgSeq;
        this.Proto = proto;
        if (header != null)
        {
            this.headerData = header.GetHeaderReadOnlySpan()[0];
        }
    }

    public byte GetNetMessageType()
    {
        return this.messageType;
    }

    public void SetNetMessageType(byte netMessageType)
    {
        this.messageType = netMessageType;
    }

    public void SetToServiceId(long toServiceId)
    {
        this.toServiceId = toServiceId;
    }

    public void SetFromServiceId(long fromServiceId)
    {
        this.ServiceId = fromServiceId;
    }

    public void SetMsgSeq(long msgSeq)
    {
        this.msgSeq = msgSeq;
    }

    public void SetProto(int proto)
    {
        this.Proto = proto;
    }

    public virtual void Dispose()
    {
    }

    public long GetToServiceId()
    {
        return this.toServiceId;
    }

    public long GetFromServiceId()
    {
        return this.ServiceId;
    }

    public long GetSeq()
    {
        return this.msgSeq;
    }

    public int GetProto()
    {
        return this.Proto;
    }

    public void Serialize()
    {
        var writer = this.GetSerializeArgeWriter();
        this.SerializeArgs(writer);
    }

    public abstract void SerializeArgs(IBufferWriter<byte> writer);

    public abstract IBufferWriter<byte> GetSerializeArgeWriter();

    public abstract MessagePackReader GetMessagePackReader();

    public virtual T DeserializeArgs<T>()
    {
        var reader = GetMessagePackReader();
        return MessagePackSerializer.Deserialize<T>(ref reader);
    }

    public virtual (T1, T2) DeserializeArgs<T1, T2>()
    {
        var reader = GetMessagePackReader();
        return MessagePackSerializer.Deserialize<(T1, T2)>(ref reader);
    }
    public virtual (T1, T2, T3) DeserializeArgs<T1, T2, T3>()
    {
        var reader = GetMessagePackReader();
        return MessagePackSerializer.Deserialize<(T1, T2, T3)>(ref reader);
    }
    public virtual (T1, T2, T3, T4) DeserializeArgs<T1, T2, T3, T4>()
    {
        var reader = GetMessagePackReader();
        return MessagePackSerializer.Deserialize<(T1, T2, T3, T4)>(ref reader);
    }
    public virtual (T1, T2, T3, T4, T5) DeserializeArgs<T1, T2, T3, T4, T5>()
    {
        var reader = GetMessagePackReader();
        return MessagePackSerializer.Deserialize<(T1, T2, T3, T4, T5)>(ref reader);
    }
    public virtual (T1, T2, T3, T4, T5, T6) DeserializeArgs<T1, T2, T3, T4, T5, T6>()
    {
        var reader = GetMessagePackReader();
        return MessagePackSerializer.Deserialize<(T1, T2, T3, T4, T5, T6)>(ref reader);
    }
    public virtual (T1, T2, T3, T4, T5, T6, T7) DeserializeArgs<T1, T2, T3, T4, T5, T6, T7>()
    {
        var reader = GetMessagePackReader();
        return MessagePackSerializer.Deserialize<(T1, T2, T3, T4, T5, T6, T7)>(ref reader);
    }
    public virtual (T1, T2, T3, T4, T5, T6, T7, T8) DeserializeArgs<T1, T2, T3, T4, T5, T6, T7, T8>()
    {
        var reader = GetMessagePackReader();
        return MessagePackSerializer.Deserialize<(T1, T2, T3, T4, T5, T6, T7, T8)>(ref reader);
    }
    public virtual (T1, T2, T3, T4, T5, T6, T7, T8, T9) DeserializeArgs<T1, T2, T3, T4, T5, T6, T7, T8, T9>()
    {
        var reader = GetMessagePackReader();
        return MessagePackSerializer.Deserialize<(T1, T2, T3, T4, T5, T6, T7, T8, T9)>(ref reader);
    }

    public virtual T ReadHead<T>()
    {
        var reader = GetMessagePackReader();
        var size = reader.Consumed;
        var result = MessagePackSerializer.Deserialize<T>(ref reader);
        var consumed = reader.Consumed - size;
        this.OnConsumed(consumed);
        return result;
    }

    public virtual void OnConsumed(long consumed)
    {
    }


    public abstract ReadOnlyMemory<byte> GetPayloadReadOnlyMemory();
}