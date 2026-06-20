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


using Proxar.Core;
namespace Proxar.IdGenerator;



/// <summary>
/// 提供线程安全的 <see cref="int"/> 类型递增操作，使用 <see cref="Interlocked.Increment(ref int)"/> 实现原子自增。
/// 继承自 <see cref="Singleton{Int32SafeIncrementer}"/>，保证全局唯一实例，通常作为多线程环境下 ID 生成器的增量组件。
/// </summary>
public class Int32SafeIncrementer : Singleton<Int32SafeIncrementer>, IIncrementer<int>
{
    /// <inheritdoc/>
    public int Increment(ref int value)
    {
        var res = Interlocked.Increment(ref value);
        return res;
    }
}