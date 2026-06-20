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


using Proxar.ServiceSynchronizationContext;


/// <summary>
/// 创建对象时自动捕获当前同步上下文的基类，提供对 <see cref="AbstractSynchronizationContext"/> 的便捷访问。
/// </summary>
public abstract class BaseObject : IDisposable
{
    private AbstractSynchronizationContext? synchronizationContext;

    /// <summary>
    /// 初始化 <see cref="BaseObject"/> 类的新实例，并捕获当前同步上下文。
    /// </summary>
    public BaseObject()
    {
        synchronizationContext = GetCurrentSynchronizationContext();
    }

    private static AbstractSynchronizationContext GetCurrentSynchronizationContext()
    {
        var syncContext = SynchronizationContext.Current as AbstractSynchronizationContext;
        return syncContext!;
    }

    /// <summary>
    /// 获取创建此对象时捕获的同步上下文。
    /// </summary>
    /// <returns>一个 <see cref="AbstractSynchronizationContext"/> 实例，或 <c>null</c>。</returns>
    public AbstractSynchronizationContext? GetSynchronization()
    {
        return synchronizationContext;
    }

    /// <summary>
    /// 获取指定类型的同步上下文。
    /// </summary>
    /// <typeparam name="T">期望的上下文类型，必须为 <see cref="AbstractSynchronizationContext"/> 的子类。</typeparam>
    /// <returns>指定类型的上下文实例，如果类型不匹配则返回 <c>null</c>。</returns>
    public T? GetSynchronization<T>()
        where T : class
    {
        return synchronizationContext as T;
    }

    /// <summary>
    /// 释放此对象占用的资源，并清除对同步上下文的引用。
    /// </summary>
    public void Dispose()
    {
        this.synchronizationContext = null;
        this.DisposeResources();
    }

    /// <summary>
    /// 在派生类中重写时，释放该对象持有的其他资源。
    /// </summary>
    protected virtual void DisposeResources()
    {

    }
}