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


using Proxar.CachePool.Interfaces;

namespace Proxar.CachePool;

/// <summary>
/// 提供 <see cref="IPoolable{T}"/> 接口的默认实现，
/// 通过 <see cref="Proxar.CachePool.ActorObjectPoolSingleton{T}"/> 自动管理对象池的租用与归还。
/// </summary>
/// <typeparam name="T">具体的池化对象类型，必须具有无参数构造函数且继承自 <see cref="AbstractPoolable{T}"/>。</typeparam>
public abstract class AbstractPoolable<T> : IPoolable<T>
    where T : notnull, AbstractPoolable<T>, new()
{
    /// <inheritdoc/>
    public bool IsDiscarded { get; set; }

    /// <inheritdoc/>
    public bool IsRented { get; set; }

    /// <inheritdoc/>
    public int PoolExpireAtTime { get; set; }

    /// <summary>
    /// 当对象从对象池中被租用时调用。派生类可重写以执行自定义初始化或重置操作。
    /// </summary>
    public virtual void OnRented()
    {
    }

    /// <summary>
    /// 将对象归还到其对应的 <see cref="ActorObjectPoolSingleton{T}"/> 对象池中。
    /// 归还后对象将被重置，以便下次租用时重用。
    /// </summary>
    public void ReturnToPool()
    {
        ActorObjectPoolSingleton<T>.Current.Return((this as T)!);
    }

    /// <summary>
    /// 丢弃当前对象。丢弃的对象不会被归还到对象池，而是等待垃圾回收。
    /// 只有在对象处于租用状态时才能执行丢弃。
    /// </summary>
    /// <returns>如果成功丢弃返回 <c>true</c>，如果对象未被租用则返回 <c>false</c>。</returns>
    public bool Discard()
    {
        if (!IsRented)
        {
            return false;
        }
        IsRented = false;
        IsDiscarded = true;
        return true;
    }
}