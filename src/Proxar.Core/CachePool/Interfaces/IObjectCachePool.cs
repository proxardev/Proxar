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
/// 定义对象缓存池的基本契约，提供定期释放过期对象的方法。
/// </summary>
public interface IObjectCachePool
{
    /// <summary>
    /// 检查并释放池中已过期的对象。
    /// </summary>
    public void CheckRelease();
}