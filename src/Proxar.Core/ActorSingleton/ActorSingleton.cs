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


using Proxar.ActorSingletonCore.Interfaces;
using Proxar.Core;
using Proxar.ServiceCore;
using System.Runtime.CompilerServices;
namespace Proxar.ActorSingletonCore;

public abstract class ActorSingleton<TSelf> :
    IActorSingleton
    where TSelf : ActorSingleton<TSelf>, new()
{
    internal static readonly int Slot;
    internal static readonly int NeedSize;
    internal long OwnerServiceId { get; private set; }

    [ThreadStatic]
    private static TSelf actorSingletonCache = null!;

    public static TSelf CurrentWithNotCache
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            return GetActorSingletonFromActor();
        }
    }

    public static TSelf Current => GetActorSingleton();

    protected ActorSingleton() { }
    static ActorSingleton()
    {
        Slot = ActorSingletonGlobalIdGenerator
            .Instance
            .NewId();
        NeedSize = Slot + 1;
    }

    private static TSelf CreateActorSingleton()
    {
        var actorSingletonList = ActorThreadScope.ActorSingletonList!;

        actorSingletonList.EnsureCapacity(NeedSize);

        var addCount = NeedSize - actorSingletonList.Count;
        for (int i = 0; i < addCount; i++)
        {
            actorSingletonList.Add(null);
        }
        var instance = new TSelf();
        (instance as ISingletonInitializer)?.Initialize();
        actorSingletonList[Slot] = instance;
        instance.OwnerServiceId = ActorThreadScope.OperateServiceId;
        return instance;
    }


    private static TSelf GetActorSingleton()
    {
        if (actorSingletonCache != null
            && actorSingletonCache.OwnerServiceId == ActorThreadScope.OperateServiceId)
        {
            return actorSingletonCache;
        }
        return GetActorSingletonFromActor();
    }


    private static TSelf GetActorSingletonFromActor()
    {
        var actorSingletonList = ActorThreadScope.ActorSingletonList;
        if (actorSingletonList.Count < NeedSize
            || actorSingletonList[Slot] == null)
        {
            actorSingletonCache = CreateActorSingleton();
        }
        actorSingletonCache = (TSelf)actorSingletonList[Slot]!;
        return actorSingletonCache;
    }

    public abstract void Dispose();
}