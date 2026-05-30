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

public interface IPoolable
{
    bool IsRented { get; set; }

    int returnTime { get; set; }

    public abstract void Reset();

    public void ReturnToPool()
    {
        //this.IsRented = false;
        //returnTime = TimeHelper.GetSecond();
        //ObjectCachePool2<T>.Instance.Return((this as T)!);
    }

    public void OnRented()
    {
        this.IsRented = true;
    }

    public int GetReturnTime()
    {
        return returnTime;
    }
}