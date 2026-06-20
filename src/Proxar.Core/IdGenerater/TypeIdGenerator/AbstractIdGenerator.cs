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
using Proxar.IdGenerator.Interfaces;
namespace Proxar.IdGenerator;


/// <summary>
/// 基于增量器的 ID 生成器抽象基类。使用指定的 <typeparamref name="TIncrementer"/> 对内部计数器进行递增，
/// 生成类型为 <typeparamref name="T"/> 的唯一标识符。
/// </summary>
/// <typeparam name="T">生成的标识符类型，必须为值类型。</typeparam>
/// <typeparam name="TIncrementer">
/// 增量器类型，必须继承 <see cref="Singleton{TIncrementer}"/> 并实现 <see cref="IIncrementer{T}"/>，且具有无参数构造函数。
/// </typeparam>
public abstract class AbstractIdGenerator<T, TIncrementer> : IIdGenerator<T>
    where TIncrementer : Singleton<TIncrementer>, IIncrementer<T>, new()
    where T : struct
{
    private TIncrementer incrementer;
    private T Id;

    /// <summary>
    /// 设置 ID 计数器的初始值。
    /// </summary>
    public T InitValue
    {
        set => Id = value;
    }

    /// <summary>
    /// 使用默认增量器实例和指定的初始值初始化新实例。
    /// </summary>
    /// <param name="initValue">计数器的初始值。</param>
    public AbstractIdGenerator(T initValue = default) :
        this(Singleton<TIncrementer>.Instance, initValue)
    {
    }

    /// <summary>
    /// 使用指定的增量器和初始值初始化新实例。
    /// </summary>
    /// <param name="incrementer">用于递增操作的增量器实例。</param>
    /// <param name="initValue">计数器的初始值。</param>
    public AbstractIdGenerator(TIncrementer incrementer, T initValue = default)
    {
        this.incrementer = incrementer;
        this.Id = initValue;
    }

    /// <summary>
    /// 生成一个新的唯一标识符。每次调用均会递增内部计数器并返回递增后的值。
    /// </summary>
    /// <returns>新生成的唯一标识符。</returns>
    public T NewId()
    {
        var res = this.incrementer.Increment(ref this.Id);
        return res;
    }
}