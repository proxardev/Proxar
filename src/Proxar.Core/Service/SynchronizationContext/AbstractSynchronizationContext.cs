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


namespace Proxar.ServiceSynchronizationContext;

/// <summary>
/// 抽象同步上下文，提供在关联的调度器上投递操作的基类。
/// </summary>
public abstract class AbstractSynchronizationContext : SynchronizationContext
{
    /// <summary>
    /// 将指定的操作投递到关联的调度器，操作将在调度器的线程上异步执行。
    /// </summary>
    /// <param name="action">要异步执行的操作。</param>
    public abstract void Post(Action action);
}