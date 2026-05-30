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
/// 用于特定情况下，在服务之间传递引用对象
/// 在当前进程下，A服务调用B服务，请求生成xx对象，在确保调用返回前，A不会使用该对象，在调用开始返回后，B不会使用、处理该对象，则可以将RefResult传入，B设置Value，返回给A使用
/// </summary>
/// <typeparam name="T"></typeparam>
public class RefResult<T>
{
    [AllowNull]
    public T Value { get; set; }
}