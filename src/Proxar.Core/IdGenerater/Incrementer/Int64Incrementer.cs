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
/// 提供对 <see cref="long"/> 类型值的递增操作，通常作为 ID 生成器的增量组件。
/// 通过 <see cref="Singleton{Int64Incrementer}"/> 保证全局唯一实例。
/// 此实现非线程安全。
/// </summary>
public class Int64Incrementer : Singleton<Int64Incrementer>, IIncrementer<long>
{
    /// <inheritdoc/>
    public long Increment(ref long value)
    {
        var val = value;
        value++;
        return val;
    }
}