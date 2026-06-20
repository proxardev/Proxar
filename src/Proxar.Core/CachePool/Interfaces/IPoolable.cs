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


namespace Proxar.CachePool.Interfaces;

/// <summary>
/// 定义对象池中可池化对象的核心行为，管理对象的租用、归还、丢弃以及过期时间。
/// </summary>
/// <typeparam name="T">对象的具体类型，通常是实现此接口的类自身。</typeparam>
public interface IPoolable<T>
{
    /// <summary>
    /// 获取或设置一个值，指示对象当前是否处于租用状态。
    /// </summary>
    bool IsRented { get; set; }

    /// <summary>
    /// 获取或设置对象的过期时间戳，超过此时限的对象可能被池清理。
    /// </summary>
    int PoolExpireAtTime { get; set; }

    /// <summary>
    /// 获取或设置一个值，指示对象是否已被丢弃，丢弃的对象将不会被回收。
    /// </summary>
    bool IsDiscarded { get; set; }

    /// <summary>
    /// 将对象归还到其对应的对象池中，使其可被再次租用。
    /// </summary>
    public void ReturnToPool();

    /// <summary>
    /// 当对象从对象池中被租用时调用，用于初始化或重置对象状态。
    /// </summary>
    public void OnRented();
}