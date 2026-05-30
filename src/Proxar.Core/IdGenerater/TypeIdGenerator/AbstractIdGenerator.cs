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

public interface IIdGenerator<T>
{

    public T NewId();
}

public abstract class AbstractIdGenerator<T, TIncrementer> : IIdGenerator<T>
    where TIncrementer : Singleton<TIncrementer>, IIncrementer<T>, new()
    where T : struct
{
    private TIncrementer incrementer;
    protected T Id;

    public T InitValue
    {
        set => Id = value;
    }

    public AbstractIdGenerator(T initValue = default) :
        this(Singleton<TIncrementer>.Instance, initValue)
    {
    }

    public AbstractIdGenerator(TIncrementer incrementer, T initValue = default)
    {
        this.incrementer = incrementer;
        this.Id = initValue;
    }

    public T NewId()
    {
        var res = this.incrementer.Increment(ref this.Id);
        return res;
    }
}