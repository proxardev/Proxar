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


namespace Proxar.IdGenerator.SnowflakeId;

/// <summary>
/// 时间单位枚举，用于指定雪花算法中时间戳的精度。
/// </summary>
public enum TimeUnit
{
    /// <summary>
    /// 秒级精度。
    /// </summary>
    Seconds,

    /// <summary>
    /// 毫秒级精度。
    /// </summary>
    Milliseconds
}