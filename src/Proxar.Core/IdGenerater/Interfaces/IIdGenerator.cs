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

namespace Proxar.IdGenerator.Interfaces;

/// <summary>
/// 定义 ID 生成器接口，用于生成指定类型的唯一标识符。
/// </summary>
/// <typeparam name="T">生成的标识符类型。</typeparam>
public interface IIdGenerator<T>
{
    /// <summary>
    /// 生成一个新的唯一标识符。
    /// </summary>
    /// <returns>新生成的唯一标识符。</returns>
    public T NewId();
}