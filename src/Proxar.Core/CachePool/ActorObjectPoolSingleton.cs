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


using Proxar.ActorSingletonCore;
using Proxar.CachePool.Interfaces;
using Proxar.Core;
using Proxar.ServiceCore;
using Proxar.Utilities;

namespace Proxar.CachePool;


internal static class ActorObjectPoolConfig
{
    // 全局唯一的配置委托
    public static Action? CustomInitialize { get; set; }
}

/// <summary>
/// 基于 <see cref="ActorSingleton{TSelf}"/> 的泛型对象池，为实现了 <see cref="IPoolable{T}"/> 的类型 <typeparamref name="T"/>
/// 提供租用（Rent）和归还（Return）管理。
/// 池的默认容量为 <see cref="DefaultPoolSize"/>，并通过定时检查释放过期对象。
/// </summary>
/// <typeparam name="T">池中存储的对象类型，必须实现 <see cref="IPoolable{T}"/> 并具有无参数构造函数。</typeparam>
public class ActorObjectPoolSingleton<T> :
    ActorSingleton<ActorObjectPoolSingleton<T>>,
    ISingletonInitializer, IObjectCachePool
    where T : notnull, IPoolable<T>, new()
{
    /// <summary>
    /// 池的默认最大容量。
    /// </summary>
    public const int DefaultPoolSize = 10_000;

    private Stack<T> stack;

    /// <summary>
    /// 初始化池实例，内部栈初始容量为 10。
    /// </summary>
    public ActorObjectPoolSingleton()
    {
        stack = new Stack<T>(10);
    }

    /// <summary>
    /// 从池中租用一个对象。如果池为空，则创建一个新实例；否则从栈中弹出并调用 <see cref="IPoolable{T}.OnRented"/> 进行重置。
    /// </summary>
    /// <returns>租用到的 <typeparamref name="T"/> 实例。</returns>
    public T Rent()
    {
        if (stack.Count == 0)
        {
            return new();
        }
        var obj = stack.Pop();
        obj.OnRented();
        return obj;
    }

    /// <summary>
    /// 将对象归还到池中。若池已满（达到 <see cref="DefaultPoolSize"/>）、对象已被标记为丢弃（<see cref="IPoolable{T}.IsDiscarded"/> 为 <c>true</c>），
    /// 若对象实现了 <see cref="IResettablePooled"/>，将先调用 <see cref="IResettablePooled.Reset"/> 重置状态，然后放回池中。
    /// </summary>
    /// <param name="obj">要归还的对象。</param>
    public void Return(T obj)
    {
        if (stack.Count >= DefaultPoolSize)
        {
            return;
        }
        if (obj.IsDiscarded)
        {
            return;
        }
        obj.PoolExpireAtTime = ActorThreadScope.ExpireAt;
        obj.IsRented = false;
        if (obj is IResettablePooled resettablePooled)
        {
            resettablePooled.Reset();
        }
        stack.Push(obj);
    }

    /// <summary>
    /// 检查并释放池中已过期的对象。过期时间由 <see cref="IPoolable{T}.PoolExpireAtTime"/> 与当前秒级时间戳比较决定。
    /// 该方法通常由定时器周期性调用，以控制池的规模。
    /// </summary>
    public void CheckRelease()
    {
        var count = stack.Count;
        if (count == 0)
        {
            return;
        }
        var list = stack.ToList();
        var index = count - 1;
        var nowTime = TimeHelper.GetSecond();

        for (; index >= 0; index--)
        {
            var obj = list[index];
            if (obj.PoolExpireAtTime > nowTime)
            {
                break;
            }
        }
        var expireTimeCount = count - 1 - index;
        if (expireTimeCount == 0)
        {
            return;
        }

        var list2 = list.Take(count - expireTimeCount).Reverse();
        stack = new Stack<T>(list2);
    }

    /// <summary>
    /// 初始化池实例，将其注册到 <see cref="ActorObjectPoolManager"/> 以便统一管理。
    /// </summary>
    public void Initialize()
    {
        if (ActorObjectPoolConfig.CustomInitialize != null)
        {
            ActorObjectPoolConfig.CustomInitialize.Invoke();
        }
        ActorObjectPoolManager.Current.AddPool(this);
    }

    /// <inheritdoc/>
    public override void Dispose()
    {
    }
}