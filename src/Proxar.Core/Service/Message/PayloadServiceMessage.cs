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
    protected IMemoryOwner<byte>? LocalMemoryOwner = null;

    private int WrittenCount = 0;

    protected bool IsSerializeArgs { get; set; }


    public PayloadServiceMessage()
    {

    }

    public PayloadServiceMessage(IMemoryOwner<byte> LocalMemoryOwner, int size)
    {
        this.LocalMemoryOwner = LocalMemoryOwner;
        this.WrittenCount = size;
    }


    public void SetIsSerializeArgs()
    {
        IsSerializeArgs = true;
    }


    public bool ValidIsSerializeArgs()
    {
        return IsSerializeArgs;
    }

    public int CalNeedMemorySize(int sizeHint)
    {
        var nowSize = this.WrittenCount + sizeHint;
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
        var oldMemoryOwner = this.LocalMemoryOwner;
        this.LocalMemoryOwner = newMemoryOwner;
        if (oldMemoryOwner != null)
        {
            oldMemoryOwner.Memory.CopyTo(newMemoryOwner.Memory);
            oldMemoryOwner.Dispose();
        }

    }

    public override void Dispose()
    {
        if (this.LocalMemoryOwner != null)
        {
            this.LocalMemoryOwner.Dispose();
        }
    }

    public override MessagePackReader GetMessagePackReader()
    {
        if (this.LocalMemoryOwner == null)
        {
            throw new InvalidOperationException("not memory buffer data");
        }
        var reader = new MessagePackReader(this.LocalMemoryOwner.Memory);
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
        if (IsSerializeArgs)
        {
            if (WrittenCount == 0)
            {
                return;
            }
            var memory = writer.GetMemory(WrittenCount);
            LocalMemoryOwner!.Memory.Slice(0, WrittenCount).CopyTo(memory);
            writer.Advance(WrittenCount);
        }
        else
        {
            this.SerializeArgs(writer);
        }
    }

    public override ReadOnlyMemory<byte> GetPayloadReadOnlyMemory()
    {
        if (!IsSerializeArgs)
        {
            SerializeArgs(this);
        }

        if (WrittenCount == 0)
        {
            return ReadOnlyMemory<byte>.Empty;
        }
        return LocalMemoryOwner!.Memory.Slice(0, WrittenCount);

    }

    public ReadOnlyMemory<byte> NetMessageSerialize()
    {
        if (!IsSerializeArgs)
        {
            this.Serialize();
        }
        return LocalMemoryOwner!.Memory.Slice(0, WrittenCount);
    }

    public Span<byte> GetSpan(int sizeHint = 0)
    {
        return this.GetMemory(sizeHint).Span;
    }

    public void Advance(int count)
    {
        this.WrittenCount += count;
    }

    public Memory<byte> GetMemory(int sizeHint = 0)
    {
        if (this.LocalMemoryOwner == null)
        {
            this.ReAllocateMemory(sizeHint);
        }
        var left = this.LocalMemoryOwner!.Memory.Length - this.WrittenCount;
        if (left <= 0 || left < sizeHint)
        {
            this.ReAllocateMemory(sizeHint);
        }
        return this.LocalMemoryOwner.Memory.Slice(this.WrittenCount);
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