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


using Proxar.Network.Interfaces;

namespace Proxar.Network;

/// <summary>
/// 全局通道配置，提供默认的终结点解析器 <see cref="IEndpointResolver"/> 和通道工厂 <see cref="IChannelFactory"/>。
/// </summary>
public static class ChannelConfig
{
    /// <summary>
    /// 获取当前的终结点解析器。默认使用 <see cref="DefaultEndpointResolver"/>。
    /// </summary>
    public static IEndpointResolver EndpointResolver { get; private set; } = new DefaultEndpointResolver();

    /// <summary>
    /// 获取当前的通道工厂。默认使用 <see cref="DefaultChannelFactory"/>。
    /// </summary>
    public static IChannelFactory ChannelFactory { get; private set; } = new DefaultChannelFactory();

    internal static void SetEndpointResolver(IEndpointResolver resolver)
    {
        EndpointResolver = resolver ?? throw new ArgumentNullException(nameof(resolver));
    }

    internal static void SetChannelFactory(IChannelFactory factory)
    {
        ChannelFactory = factory ?? throw new ArgumentNullException(nameof(factory));
    }
}