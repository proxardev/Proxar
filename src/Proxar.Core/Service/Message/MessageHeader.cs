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


using Proxar.ServiceCore.Interfaces;

namespace Proxar.ServiceCore.Message;



/// <summary>
/// 消息头的默认实现，用于在服务间传递轻量级的元数据（最多支持4个 <see cref="long"/> 值）。
/// </summary>
public class MessageHeader : IMessageHeader
{
    private long[] _data;

    /// <summary>
    /// 使用单个头部数据初始化 <see cref="MessageHeader"/> 的新实例。
    /// </summary>
    /// <param name="data">头部数据1。</param>
    public MessageHeader(long data)
    {
        _data = new long[] { data };
    }

    /// <summary>
    /// 使用两个头部数据初始化 <see cref="MessageHeader"/> 的新实例。
    /// </summary>
    /// <param name="data">头部数据1。</param>
    /// <param name="data2">头部数据2。</param>
    public MessageHeader(long data, long data2)
    {
        _data = new long[] { data, data2 };
    }

    /// <summary>
    /// 使用三个头部数据初始化 <see cref="MessageHeader"/> 的新实例。
    /// </summary>
    /// <param name="data">头部数据1。</param>
    /// <param name="data2">头部数据2。</param>
    /// <param name="data3">头部数据3。</param>
    public MessageHeader(long data, long data2, long data3)
    {
        _data = new long[] { data, data2, data3 };
    }

    /// <summary>
    /// 使用四个头部数据初始化 <see cref="MessageHeader"/> 的新实例。
    /// </summary>
    /// <param name="data">头部数据1。</param>
    /// <param name="data2">头部数据2。</param>
    /// <param name="data3">头部数据3。</param>
    /// <param name="data4">头部数据4。</param>
    public MessageHeader(long data, long data2, long data3, long data4)
    {
        _data = new long[] { data, data2, data3, data4 };
    }

    /// <inheritdoc/>
    public ReadOnlySpan<long> GetHeaderReadOnlySpan()
    {
        return _data.AsSpan().Slice(0, _data.Length);
    }
}