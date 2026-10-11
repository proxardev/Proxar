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



namespace Proxar.ServiceCore.Interfaces;

/// <summary>
/// 消息头扩展槽位（位值）。
/// 位图固定 1 字节，最多 8 个槽（<see cref="IMessageHeader.MaxSlotCount"/>）；
/// 每个槽位对应消息头中的一个 <see cref="long"/> 值，未置位槽不写入字节流。
/// </summary>
[Flags]
public enum HeaderSlot : byte
{
    /// <summary>位 0: RPC 关联 ID</summary>
    Seq = 1 << 0,

    /// <summary>位 1: 目标玩家身份</summary>
    TargetId = 1 << 1,

    /// <summary>位 2: 标志位（预留）</summary>
    Flags = 1 << 2,

    /// <summary>位 3: 预留</summary>
    Reserved = 1 << 3,

    /// <summary>位 4: 预留扩展槽 4</summary>
    Slot4 = 1 << 4,

    /// <summary>位 5: 预留扩展槽 5</summary>
    Slot5 = 1 << 5,

    /// <summary>位 6: 预留扩展槽 6</summary>
    Slot6 = 1 << 6,

    /// <summary>位 7: 预留扩展槽 7</summary>
    Slot7 = 1 << 7,
}
