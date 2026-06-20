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


using Proxar.Tasks;
namespace Proxar.Core;

/// <summary>
/// 定义需要同步初始化的单例组件。
/// 实现此接口的单例将在初始化时调用 <see cref="Initialize"/> 方法。
/// </summary>
public interface ISingletonInitializer
{
    /// <summary>
    /// 执行同步初始化逻辑。
    /// </summary>
    public void Initialize();
}

/// <summary>
/// 定义需要异步初始化的单例组件。
/// 实现此接口的单例将在初始化时调用 <see cref="InitializeAsync"/> 方法，
/// 返回的 <see cref="ZFTask"/> 会被等待直到初始化完成。
/// </summary>
internal interface ISingletonInitializerAsync
{
    /// <summary>
    /// 执行异步初始化逻辑，返回一个可等待的 <see cref="ZFTask"/>。
    /// </summary>
    /// <returns>表示异步初始化操作的 <see cref="ZFTask"/>。</returns>
    public ZFTask InitializeAsync();
}