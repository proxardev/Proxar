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


public static class ActorObjectPoolConfig
{
    // 全局唯一的配置委托
    public static Action? CustomInitialize { get; set; }
}


public class ActorObjectPoolSingleton<T> :
    ActorSingleton<ActorObjectPoolSingleton<T>>,
    ISingletonInitializer, IObjectCachePool
    where T : notnull, IPoolable<T>, new()

{
    public const int DefaultPoolSize = 10_000;
    private Stack<T> stack;

    public ActorObjectPoolSingleton()
    {
        stack = new Stack<T>(10);
    }

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

    public void Initialize()
    {
        if (ActorObjectPoolConfig.CustomInitialize != null)
        {
            ActorObjectPoolConfig.CustomInitialize.Invoke();
            return;
        }
        ActorObjectPoolManager.Current
            .AddPool(this);
    }

    public override void Dispose()
    {
    }
}