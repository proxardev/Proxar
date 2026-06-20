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

/// <summary>
/// 定义可重置状态的对象，支持在对象池中回收时进行清理操作。
/// </summary>
public interface IResettablePooled
{
    /// <summary>
    /// 重置对象的状态到初始值，以便在对象池中重用。
    /// </summary>
    void Reset();
}