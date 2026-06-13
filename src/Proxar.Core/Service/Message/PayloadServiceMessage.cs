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


public abstract class PayloadServiceMessage : AbstractServiceMessage, IBufferWriter<byte>, INetMessage
{
    protected IMemoryOwner<byte>? localMemoryOwner = null;

    private int writtenCount = 0;

    protected bool isSerializeArgs { get; set; }


    public PayloadServiceMessage()
    {

    }

    public PayloadServiceMessage(IMemoryOwner<byte> LocalMemoryOwner, int size)
    {
        this.localMemoryOwner = LocalMemoryOwner;
        this.writtenCount = size;
    }


    public void SetIsSerializeArgs()
    {
        isSerializeArgs = true;
    }


    public bool ValidIsSerializeArgs()
    {
        return isSerializeArgs;
    }

    public int CalNeedMemorySize(int sizeHint)
    {
        var nowSize = this.writtenCount + sizeHint;
        var need = Math.Max(nowSize, 1024);
        if (need == nowSize)
        {
            need *= 2;
        }
        return need;
    }

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

    public override void Dispose()
    {
        if (this.localMemoryOwner != null)
        {
            var oldMemoryOwner = this.localMemoryOwner;
            this.localMemoryOwner = null;
            oldMemoryOwner.Dispose();
        }
    }

    public override MessagePackReader GetMessagePackReader()
    {
        if (this.localMemoryOwner == null)
        {
            throw new InvalidOperationException("not memory buffer data");
        }
        var reader = new MessagePackReader(this.localMemoryOwner.Memory);
        return reader;
    }

    public override IBufferWriter<byte> GetSerializeArgeWriter()
    {
        return this;
    }

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

    public ReadOnlyMemory<byte> NetMessageSerialize()
    {
        if (!isSerializeArgs)
        {
            this.Serialize();
        }
        return localMemoryOwner!.Memory.Slice(0, writtenCount);
    }

    public Span<byte> GetSpan(int sizeHint = 0)
    {
        return this.GetMemory(sizeHint).Span;
    }

    public void Advance(int count)
    {
        this.writtenCount += count;
    }

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

    public override T ReadHead<T>()
    {
        if (this.headerData is T t)
        {
            return t;
        }
        throw new InvalidOperationException();
    }

    public virtual void NetMessageDeserialize()
    {

    }

    public void NetMessageHandle()
    {
        throw new NotImplementedException();
    }
}