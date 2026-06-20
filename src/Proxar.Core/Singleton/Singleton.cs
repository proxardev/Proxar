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


namespace Proxar.Core;

/// <summary>
/// 提供泛型单例基类，确保类型 <typeparamref name="T"/> 在整个应用程序域中只有一个实例。
/// 实例的创建和初始化是线程安全的（通过 <see cref="Lazy{T}"/> 实现），但实例内部状态和方法的线程安全性需由派生类自行保证。
/// </summary>
/// <typeparam name="T">要作为单例的类，必须具有无参数构造函数，且继承自 <see cref="Singleton{T}"/>。</typeparam>
public abstract class Singleton<T>
    where T : Singleton<T>, new()
{
    /// <summary>
    /// 延迟初始化的单例实例，使用 <see cref="Lazy{T}"/> 保证实例创建的线程安全。
    /// </summary>
    private static readonly Lazy<T> _lazyInstance = new Lazy<T>(CreateInstance, true);

    /// <summary>
    /// 初始化 <see cref="Singleton{T}"/> 类的新实例（仅由派生类调用）。
    /// </summary>
    protected Singleton() { }

    /// <summary>
    /// 获取该类型的唯一实例。首次访问时会以线程安全的方式创建并初始化实例。
    /// </summary>
    public static T Instance => _lazyInstance.Value;

    private static T CreateInstance()
    {
        var instance = new T();
        InitializeSingleton(instance);
        return instance;
    }

    /// <summary>
    /// 对单例实例执行初始化。如果实例实现了 <see cref="ISingletonInitializer"/>，
    /// 则调用其 <see cref="ISingletonInitializer.Initialize"/> 方法。
    /// </summary>
    /// <param name="instance">要初始化的单例实例。</param>
    private static void InitializeSingleton(T instance)
    {
        (instance as ISingletonInitializer)?.Initialize();
    }
}