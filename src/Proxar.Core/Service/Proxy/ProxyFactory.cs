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

using Proxar.ServiceCore.Interfaces;
using System.Diagnostics.CodeAnalysis;

namespace Proxar.ServiceCore;






public static class ProxyFactory<T> where T : class, IServiceProxyBase
{
    [AllowNull]
    internal static Func<long, T> Create { get; private set; }
    [AllowNull]
    internal static Func<long, IMessageInvoker, T> CreateWithInvoker { get; private set; }

    public static void Register(Func<long, T> factory)
    {
        Create = factory;
    }

    public static void RegisterCreateWithInvoker(Func<long, IMessageInvoker, T> factory)
    {
        CreateWithInvoker = factory;
    }

}
