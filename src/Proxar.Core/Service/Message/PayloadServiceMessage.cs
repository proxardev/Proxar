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
/// 提供消息负载序列化/反序列化能力的抽象基类，同时实现了 <see cref="IBufferWriter{Byte}"/> 和 <see cref="INetMessage"/>，
/// 支持将消息参数直接写入内存缓冲区，适用于网络传输和零拷贝场景。
/// </summary>
public abstract class PayloadServiceMessage : AbstractServiceMessage, IBufferWriter<byte>, INetMessage
{
    /// <summary>
    /// 消息负载的内存所有者，用于持有序列化后的字节数据。
    /// </summary>
    protected IMemoryOwner<byte>? localMemoryOwner = null;

    /// <summary>
    /// 已写入缓冲区的字节数。
    /// </summary>
    private int writtenCount = 0;

    /// <summary>
    /// 指示消息的参数是否已被序列化（标记为 true 后，可避免重复序列化）。
    /// </summary>
    protected bool isSerializeArgs { get; private set; }

    /// <summary>
    /// 初始化 <see cref="PayloadServiceMessage"/> 类的新实例。
    /// </summary>
    public PayloadServiceMessage()
    {
    }

    /// <summary>
    /// 初始化 <see cref="PayloadServiceMessage"/> 类的新实例，并指定外部提供的内存和已写入的大小。
    /// </summary>
    /// <param name="LocalMemoryOwner">预先分配的内存所有者。</param>
    /// <param name="size">已写入的数据大小。</param>
    public PayloadServiceMessage(IMemoryOwner<byte> LocalMemoryOwner, int size)
    {
        this.localMemoryOwner = LocalMemoryOwner;
        this.writtenCount = size;
    }

    /// <summary>
    /// 标记消息的参数已被序列化。之后调用 <see cref="GetPayloadReadOnlyMemory"/> 将直接返回缓存的数据，避免重复序列化。
    /// </summary>
    public void SetIsSerializeArgs()
    {
        isSerializeArgs = true;
    }

    /// <summary>
    /// 计算扩展缓冲区所需的最小容量。
    /// </summary>
    /// <param name="sizeHint">预计要写入的字节数。</param>
    /// <returns>建议的新缓冲区大小。</returns>
    private int CalNeedMemorySize(int sizeHint)
    {
        var nowSize = this.writtenCount + sizeHint;
        var need = Math.Max(nowSize, 1024);
        if (need == nowSize)
        {
            need *= 2;
        }
        return need;
    }

    /// <summary>
    /// 重新分配内部内存，将现有数据复制到新的更大的缓冲区，并释放旧缓冲区。
    /// </summary>
    /// <param name="sizeHint">预计要写入的字节数。</param>
    private void ReAllocateMemory(int sizeHint)
    {
        var needSize = this.CalNeedMemorySize(sizeHint);
        var newMemoryOwner = MemoryPool<byte>.Shared.Rent(needSize);
        var oldMemoryOwner = this.localMemoryOwner;
        this.localMemoryOwner = newMemoryOwner;
        if (oldMemoryOwner != null)
        {
            oldMemoryOwner.Memory.CopyTo(newMemoryOwner.Memory);
            oldMemoryOwner.Dispose();
        }
    }


    /// <inheritdoc/>
    public override void Dispose()
    {
        if (this.localMemoryOwner != null)
        {
            var oldMemoryOwner = this.localMemoryOwner;
            this.localMemoryOwner = null;
            oldMemoryOwner.Dispose();
        }
    }


    /// <inheritdoc/>
    public override MessagePackReader GetMessagePackReader()
    {
        if (this.localMemoryOwner == null)
        {
            throw new InvalidOperationException("not memory buffer data");
        }
        var reader = new MessagePackReader(this.localMemoryOwner.Memory);
        return reader;
    }


    /// <inheritdoc/>
    public override IBufferWriter<byte> GetSerializeArgeWriter()
    {
        return this;
    }


    /// <inheritdoc/>
    public void NetMessageSerialize(IBufferWriter<byte> writer)
    {
        var writer2 = new MessagePackWriter(writer);
        writer2.Write(this.toServiceId);
        Service.WriterHeander(ref writer2, ServiceId, msgSeq, Proto, headerData);
        if (isSerializeArgs)
        {
            if (writtenCount == 0)
            {
                return;
            }
            var memory = writer.GetMemory(writtenCount);
            localMemoryOwner!.Memory.Slice(0, writtenCount).CopyTo(memory);
            writer.Advance(writtenCount);
        }
        else
        {
            this.SerializeArgs(writer);
        }
    }

    /// <inheritdoc/>
    public override ReadOnlyMemory<byte> GetPayloadReadOnlyMemory()
    {
        if (!isSerializeArgs)
        {
            SerializeArgs(this);
        }

        if (writtenCount == 0)
        {
            return ReadOnlyMemory<byte>.Empty;
        }
        return localMemoryOwner!.Memory.Slice(0, writtenCount);
    }


    /// <inheritdoc/>
    public ReadOnlyMemory<byte> NetMessageSerialize()
    {
        if (!isSerializeArgs)
        {
            this.Serialize();
        }
        return localMemoryOwner!.Memory.Slice(0, writtenCount);
    }

    /// <inheritdoc/>
    public Span<byte> GetSpan(int sizeHint = 0)
    {
        return this.GetMemory(sizeHint).Span;
    }

    /// <inheritdoc/>
    public void Advance(int count)
    {
        this.writtenCount += count;
    }

    /// <inheritdoc/>
    public Memory<byte> GetMemory(int sizeHint = 0)
    {
        if (this.localMemoryOwner == null)
        {
            this.ReAllocateMemory(sizeHint);
        }
        var left = this.localMemoryOwner!.Memory.Length - this.writtenCount;
        if (left <= 0 || left < sizeHint)
        {
            this.ReAllocateMemory(sizeHint);
        }
        return this.localMemoryOwner.Memory.Slice(this.writtenCount);
    }


    /// <inheritdoc/>
    public override T ReadHead<T>()
    {
        if (this.headerData is T t)
        {
            return t;
        }
        throw new InvalidOperationException();
    }


    /// <inheritdoc/>
    public virtual void NetMessageDeserialize()
    {
    }


    /// <inheritdoc/>
    public void NetMessageHandle()
    {
        throw new NotImplementedException();
    }
}