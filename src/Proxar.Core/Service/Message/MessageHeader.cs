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

namespace Proxar.ServiceCore.Message;



/// <summary>
/// <see cref="IMessageHeader"/> 的默认实现：固定 8 槽（<see cref="IMessageHeader.MaxSlotCount"/>）
/// </summary>
public class MessageHeader : IMessageHeader
{
    private readonly long[] _data = new long[IMessageHeader.MaxSlotCount];

    public MessageHeader()
    {
    }

    public MessageHeader(long rpcId)
    {
        this.Set(HeaderSlot.Seq, rpcId);
    }

    /// <inheritdoc/>
    public byte Bitmap
    {
        get
        {
            byte bitmap = 0;
            for (int i = 0; i < IMessageHeader.MaxSlotCount; i++)
            {
                if (this._data[i] != 0)
                {
                    bitmap |= (byte)(1 << i);
                }
            }
            return bitmap;
        }
    }

    /// <inheritdoc/>
    public long Get(HeaderSlot slot)
    {
        return this._data[GetIndex(slot)];
    }

    /// <inheritdoc/>
    public void Set(HeaderSlot slot, long value)
    {
        this._data[GetIndex(slot)] = value;
    }

    /// <summary>
    /// 槽位位值 → 数组索引：0x01→0，0x02→1，0x04→2，…，0x80→7。
    /// 非 2 的幂（未知槽位）抛异常。
    /// </summary>
    private static int GetIndex(HeaderSlot slot)
    {
        var v = (byte)slot;
        if (v == 0 || (v & (v - 1)) != 0)
        {
            throw new ArgumentOutOfRangeException(nameof(slot), slot, "未知槽位");
        }
        var index = 0;
        while ((v & 0x01) == 0)
        {
            v >>= 1;
            index++;
        }
        return index;
    }



    /// <summary>
    /// 从其他头部复制全部槽值（用于将外部 header 拷贝进消息对象，隔离生命周期）。
    /// </summary>
    public void CopyFrom(IMessageHeader other)
    {
        for (int i = 0; i < IMessageHeader.MaxSlotCount; i++)
        {
            this._data[i] = other.Get((HeaderSlot)(1 << i));
        }
    }

    /// <inheritdoc/>
    public void WriteTo(ref MessagePackWriter writer)
    {
        var bitmap = this.Bitmap;
        writer.Write(bitmap);
        for (int i = 0; i < IMessageHeader.MaxSlotCount; i++)
        {
            if ((bitmap & (1 << i)) != 0)
            {
                writer.Write(this._data[i]);
            }
        }
    }

    /// <inheritdoc/>
    public void ReadFrom(ref MessagePackReader reader)
    {
        var bitmap = reader.ReadByte();
        for (int i = 0; i < IMessageHeader.MaxSlotCount; i++)
        {
            if ((bitmap & (1 << i)) != 0)
            {
                this._data[i] = reader.ReadInt64();
            }
        }
    }
}
