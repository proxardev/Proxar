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

public abstract class AbstractPoolable<T> : IPoolable<T>
    where T : notnull, AbstractPoolable<T>, new()
{
    public bool IsDiscarded { get; set; }
    public bool IsRented { get; set; }
    public int PoolExpireAtTime { get; set; }

    public virtual void OnRented()
    {
    }

    public void ReturnToPool()
    {
        ActorObjectPoolSingleton<T>.Current.Return((this as T)!);
    }

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