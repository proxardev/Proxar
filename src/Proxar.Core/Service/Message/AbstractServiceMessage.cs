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


/// <summary>
/// 提供 <see cref="IServiceMessage"/> 接口的基础实现，管理消息头字段并提供 MessagePack 序列化/反序列化的默认行为。
/// </summary>
public abstract class AbstractServiceMessage : IServiceMessage
{
    /// <summary>
    /// 目标服务 ID。
    /// </summary>
    protected long toServiceId { get; set; } = 0;

    /// <summary>
    /// 网络消息类型标识。
    /// </summary>
    protected byte messageType = 0;

    /// <summary>
    /// 源服务 ID。
    /// </summary>
    protected long ServiceId = 0;

    /// <summary>
    /// 消息序列号。
    /// </summary>
    protected long msgSeq = 0;

    /// <summary>
    /// 协议方法编号。
    /// </summary>
    protected int Proto = 0;

    /// <summary>
    /// 消息头部原始数据（来自 <see cref="IMessageHeader"/>）。
    /// </summary>
    protected long headerData = 0;

    /// <summary>
    /// 设置消息头字段。
    /// </summary>
    /// <param name="serviceId">发送方服务 ID。</param>
    /// <param name="toServiceId">目标服务 ID。</param>
    /// <param name="msgSeq">消息序列号。</param>
    /// <param name="proto">协议方法编号。</param>
    /// <param name="header">可选的 <see cref="IMessageHeader"/>。</param>
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

    ///// <summary>
    ///// 获取消息的网络类型标识。
    ///// </summary>
    ///// <returns>表示消息类型的字节值。</returns>
    //public byte GetNetMessageType()
    //{
    //    return this.messageType;
    //}

    ///// <summary>
    ///// 设置消息的网络类型标识。
    ///// </summary>
    ///// <param name="netMessageType">消息类型。</param>
    //public void SetNetMessageType(byte netMessageType)
    //{
    //    this.messageType = netMessageType;
    //}

    /// <summary>
    /// 设置目标服务 ID。
    /// </summary>
    /// <param name="toServiceId">目标服务 ID。</param>
    public void SetToServiceId(long toServiceId)
    {
        this.toServiceId = toServiceId;
    }

    /// <summary>
    /// 设置发送方服务 ID。
    /// </summary>
    /// <param name="fromServiceId">发送方服务 ID。</param>
    public void SetFromServiceId(long fromServiceId)
    {
        this.ServiceId = fromServiceId;
    }

    /// <summary>
    /// 设置消息序列号。
    /// </summary>
    /// <param name="msgSeq">消息序列号。</param>
    public void SetMsgSeq(long msgSeq)
    {
        this.msgSeq = msgSeq;
    }

    /// <summary>
    /// 设置协议方法编号。
    /// </summary>
    /// <param name="proto">协议方法编号。</param>
    public void SetProto(int proto)
    {
        this.Proto = proto;
    }


    /// <inheritdoc/>
    public virtual void Dispose()
    {
    }

    /// <inheritdoc/>
    public long GetToServiceId()
    {
        return this.toServiceId;
    }

    /// <inheritdoc/>
    public long GetFromServiceId()
    {
        return this.ServiceId;
    }

    /// <inheritdoc/>
    public long GetSeq()
    {
        return this.msgSeq;
    }

    /// <inheritdoc/>
    public int GetProto()
    {
        return this.Proto;
    }

    /// <summary>
    /// 触发消息负载的序列化（由子类实现具体的写入逻辑）。
    /// </summary>
    /// <remarks>
    /// 此方法首先获取一个 <see cref="IBufferWriter{Byte}"/>，然后调用 <see cref="SerializeArgs"/> 将参数写入其中。
    /// </remarks>
    public void Serialize()
    {
        var writer = this.GetSerializeArgeWriter();
        this.SerializeArgs(writer);
    }

    /// <summary>
    /// 将消息的参数部分写入指定的 <see cref="IBufferWriter{Byte}"/>。
    /// </summary>
    /// <param name="writer">用于写入序列化数据的缓冲区写入器。</param>
    public abstract void SerializeArgs(IBufferWriter<byte> writer);

    /// <summary>
    /// 获取用于写入消息参数的 <see cref="IBufferWriter{Byte}"/>。
    /// </summary>
    /// <returns>一个 <see cref="IBufferWriter{Byte}"/> 实例。</returns>
    public abstract IBufferWriter<byte> GetSerializeArgeWriter();

    /// <summary>
    /// 获取用于反序列化消息负载的 <see cref="MessagePackReader"/>。
    /// </summary>
    /// <returns>一个 <see cref="MessagePackReader"/> 实例。</returns>
    public abstract MessagePackReader GetMessagePackReader();

    /// <inheritdoc/>
    public virtual T DeserializeArgs<T>()
    {
        var reader = GetMessagePackReader();
        return MessagePackSerializer.Deserialize<T>(ref reader);
    }

    /// <inheritdoc/>
    public virtual (T1, T2) DeserializeArgs<T1, T2>()
    {
        var reader = GetMessagePackReader();
        return MessagePackSerializer.Deserialize<(T1, T2)>(ref reader);
    }

    /// <inheritdoc/>
    public virtual (T1, T2, T3) DeserializeArgs<T1, T2, T3>()
    {
        var reader = GetMessagePackReader();
        return MessagePackSerializer.Deserialize<(T1, T2, T3)>(ref reader);
    }

    /// <inheritdoc/>
    public virtual (T1, T2, T3, T4) DeserializeArgs<T1, T2, T3, T4>()
    {
        var reader = GetMessagePackReader();
        return MessagePackSerializer.Deserialize<(T1, T2, T3, T4)>(ref reader);
    }

    /// <inheritdoc/>
    public virtual (T1, T2, T3, T4, T5) DeserializeArgs<T1, T2, T3, T4, T5>()
    {
        var reader = GetMessagePackReader();
        return MessagePackSerializer.Deserialize<(T1, T2, T3, T4, T5)>(ref reader);
    }

    /// <inheritdoc/>
    public virtual (T1, T2, T3, T4, T5, T6) DeserializeArgs<T1, T2, T3, T4, T5, T6>()
    {
        var reader = GetMessagePackReader();
        return MessagePackSerializer.Deserialize<(T1, T2, T3, T4, T5, T6)>(ref reader);
    }

    /// <inheritdoc/>
    public virtual (T1, T2, T3, T4, T5, T6, T7) DeserializeArgs<T1, T2, T3, T4, T5, T6, T7>()
    {
        var reader = GetMessagePackReader();
        return MessagePackSerializer.Deserialize<(T1, T2, T3, T4, T5, T6, T7)>(ref reader);
    }

    /// <inheritdoc/>
    public virtual (T1, T2, T3, T4, T5, T6, T7, T8) DeserializeArgs<T1, T2, T3, T4, T5, T6, T7, T8>()
    {
        var reader = GetMessagePackReader();
        return MessagePackSerializer.Deserialize<(T1, T2, T3, T4, T5, T6, T7, T8)>(ref reader);
    }

    /// <inheritdoc/>
    public virtual (T1, T2, T3, T4, T5, T6, T7, T8, T9) DeserializeArgs<T1, T2, T3, T4, T5, T6, T7, T8, T9>()
    {
        var reader = GetMessagePackReader();
        return MessagePackSerializer.Deserialize<(T1, T2, T3, T4, T5, T6, T7, T8, T9)>(ref reader);
    }

    /// <inheritdoc/>
    public virtual T ReadHead<T>()
    {
        var reader = GetMessagePackReader();
        var size = reader.Consumed;
        var result = MessagePackSerializer.Deserialize<T>(ref reader);
        var consumed = reader.Consumed - size;
        this.OnConsumed(consumed);
        return result;
    }

    /// <inheritdoc/>
    public abstract ReadOnlyMemory<byte> GetPayloadReadOnlyMemory();

    /// <summary>
    /// 在从消息中读取指定长度的数据后调用，用于跟踪读取进度。
    /// </summary>
    /// <param name="consumed">已读取的字节数。</param>
    protected virtual void OnConsumed(long consumed)
    {
    }
}