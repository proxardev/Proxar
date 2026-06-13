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
using Proxar.Timer;

namespace Proxar.CachePool;


internal sealed class ActorObjectPoolManager :
    ActorSingleton<ActorObjectPoolManager>, ISingletonInitializer
{
    private Dictionary<Type, IObjectCachePool> cache = new();


    public void AddPool<T>(ActorObjectPoolSingleton<T> pool)
        where T : notnull, IPoolable<T>, new()
    {
        var type = typeof(T);
        cache[type] = pool;
    }

    private void CheckRelease()
    {
        foreach (var cache in cache.Values)
        {
            cache.CheckRelease();
        }
    }

    public void Initialize()
    {
        TimerHelper.IntervalTimerCall(ActorThreadScope.CheckReleaseMilliTime,
            this.CheckRelease);
    }

    public override void Dispose()
    {
    }
}