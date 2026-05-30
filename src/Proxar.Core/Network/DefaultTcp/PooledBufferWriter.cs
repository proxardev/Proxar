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

internal sealed class PooledBufferWriter : IBufferWriter<byte>, IDisposable
{
    private IMemoryOwner<byte>? _owner;
    private Memory<byte> _memory;
    private int _written;

    public int WrittenCount => _written;

    public int Capacity => _memory.Length;

    public Memory<byte> WrittenMemory => _memory.Slice(0, _written);

    public ReadOnlySpan<byte> WrittenSpan => _memory.Span.Slice(0, _written);


    public PooledBufferWriter() : this(256, MemoryPool<byte>.Shared)
    {
    }

    public PooledBufferWriter(int initialCapacity, MemoryPool<byte>? pool = null)
    {
        if (initialCapacity <= 0)
            throw new ArgumentOutOfRangeException(nameof(initialCapacity));
        pool ??= MemoryPool<byte>.Shared;

        _owner = pool.Rent(initialCapacity);
        _memory = _owner.Memory;
        _written = 0;
    }

    /// <summary>
    /// 返回用于写入的 <see cref="Memory{T}"/>，至少保证 requestedSize 大小。
    /// 如果当前剩余空间不足，会自动扩容。
    /// </summary>
    public Memory<byte> GetMemory(int sizeHint = 0)
    {

        var remaining = _memory.Length - _written;
        if (remaining >= sizeHint)
            return _memory.Slice(_written);

        // 需要扩容
        var newSize = CalculateNewCapacity(_written, sizeHint);
        Resize(newSize);
        return _memory.Slice(_written);
    }

    /// <summary>
    /// 返回用于写入的 <see cref="Span{T}"/>，至少保证 requestedSize 大小。
    /// </summary>
    public Span<byte> GetSpan(int sizeHint = 0)
        => GetMemory(sizeHint).Span;

    /// <summary>
    /// 通知写入器已经向缓冲区写入了 count 个字节。
    /// </summary>
    public void Advance(int count)
    {
        _written += count;
    }


    private void Resize(int newCapacity)
    {
        if (newCapacity <= _memory.Length) return;

        var newOwner = MemoryPool<byte>.Shared.Rent(newCapacity);
        var newMemory = newOwner.Memory;
        // 复制现有数据
        _memory.Slice(0, _written).CopyTo(newMemory);
        // 释放原内存
        _owner?.Dispose();
        _owner = newOwner;
        _memory = newMemory;
    }

    private int CalculateNewCapacity(int written, int sizeHint)
    {
        var currentSize = _memory.Length;
        var required = written + (sizeHint > 0 ? sizeHint : 1);
        if (required <= currentSize) return currentSize;

        // 扩容策略：至少翻倍，或直接满足所需容量
        var newSize = Math.Max(currentSize * 2, required);
        // 限制最大分配大小（可选，这里设为 1GB 可配置）
        const int MaxPooledSize = 1024 * 1024 * 1024; // 1GB
        return newSize > MaxPooledSize ? required : newSize;
    }

    public void Dispose()
    {
        var owner = _owner;
        if (owner != null)
        {
            _owner = null;
            _memory = default;
            _written = 0;
            owner.Dispose();
        }
    }
}