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


/// <summary>
/// 基于 Actor 的泛型单例基类。每个 Actor 拥有属于自己的该类型单例实例，
/// 实例存储于当前 Actor 的 <see cref="Proxar.ServiceSynchronizationContext.ActorSynchronizationContext"/> 中，
/// 通过 <see cref="ActorSingletonGlobalIdGenerator"/> 分配的固定插槽进行索引。
/// 使用 <see cref="ThreadStaticAttribute"/> 提供的线程本地缓存加速同一线程内的重复访问。
/// </summary>
/// <typeparam name="TSelf">派生类自身的类型。</typeparam>
public abstract class ActorSingleton<TSelf> :
    IActorSingleton
    where TSelf : ActorSingleton<TSelf>, new()
{
    /// <summary>
    /// 此单例类型在 Actor 单例列表中的固定插槽位置。
    /// </summary>
    internal static readonly int Slot;

    /// <summary>
    /// Actor 单例列表的最小长度，即 <see cref="Slot"/> + 1。
    /// </summary>
    internal static readonly int NeedSize;

    /// <summary>
    /// 获取拥有此单例实例的服务 ID。
    /// </summary>
    internal long OwnerServiceId { get; private set; }

    [ThreadStatic]
    private static TSelf actorSingletonCache = null!;

    /// <summary>
    /// 获取当前 Actor 的单例实例。此属性不使用线程本地缓存，每次都从 Actor 上下文中获取。
    /// </summary>
    public static TSelf CurrentWithNotCache
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            return GetActorSingletonFromActor();
        }
    }

    /// <summary>
    /// 获取当前 Actor 的单例实例，优先使用线程本地缓存以提高性能。
    /// </summary>
    public static TSelf Current => GetActorSingleton();

    /// <summary>
    /// 初始化 <see cref="ActorSingleton{TSelf}"/> 的新实例（仅由派生类调用）。
    /// </summary>
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

    /// <inheritdoc/>
    public abstract void Dispose();
}