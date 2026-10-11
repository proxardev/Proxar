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
using Proxar.ServiceCore.Message;

namespace Proxar.ServiceCore.Interfaces;

/// <summary>
/// 定义消息头的通用接口，提供对头部数据的只读访问。
/// <list type="bullet">
/// <item>最大槽位数 <see cref="MaxSlotCount"/>（1 字节位图 → 8 槽）；</item>
/// <item>槽位读写 <see cref="Get"/> / <see cref="Set"/>；</item>
/// <item>编解码 <see cref="WriteTo"/> / <see cref="ReadFrom"/></item>
/// </list>
/// </summary>
public interface IMessageHeader
{
    /// <summary>
    /// 最大槽位数
    /// </summary>
    const int MaxSlotCount = 8;

    /// <summary>
    /// 当前置位掩码
    /// </summary>
    byte Bitmap { get; }

    /// <summary>
    /// 读取指定槽位的值；槽未置位时返回 0。
    /// </summary>
    long Get(HeaderSlot slot);

    /// <summary>
    /// 写入指定槽位的值；写入 0 等价于清除该位。
    /// </summary>
    void Set(HeaderSlot slot, long value);

    /// <summary>
    /// 将头部序列化到 writer：先写 1 字节位图（0x00 也写，保证读端无歧义），再按位图循环写入非零槽值。
    /// </summary>
    void WriteTo(ref MessagePackWriter writer);

    /// <summary>
    /// 从 reader 反序列化头部：先读 1 字节位图，再按位图循环读取槽值。
    /// </summary>
    void ReadFrom(ref MessagePackReader reader);
}
