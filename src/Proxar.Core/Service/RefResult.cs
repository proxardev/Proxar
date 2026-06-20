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


using System.Diagnostics.CodeAnalysis;

namespace Proxar.ServiceCore;

/// <summary>
/// 用于在当前进程的服务之间传递引用对象。
/// </summary>
/// <remarks>
/// A 服务调用 B 服务，请求生成 <typeparamref name="T"/> 对象并返回给 A 使用。需确保：
/// 1. 调用返回前，A 不会使用该对象。
/// 2. 调用返回后，B 不再使用或修改该对象。
/// </remarks>
/// <typeparam name="T">传递的引用对象的类型。</typeparam>
public class RefResult<T>
{
    /// <summary>
    /// 结果值。
    /// </summary>
    [AllowNull]
    public T Value { get; set; }
}