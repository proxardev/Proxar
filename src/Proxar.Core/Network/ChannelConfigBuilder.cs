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
/// 用于构建网络通道配置的建造器，允许注入自定义的终结点解析器 <see cref="IEndpointResolver"/> 和通道工厂 <see cref="IChannelFactory"/>。
/// </summary>
public class ChannelConfigBuilder
{
    internal IEndpointResolver? EndpointResolver;
    internal IChannelFactory? ChannelFactory;

    /// <summary>
    /// 设置自定义的 <see cref="IEndpointResolver"/>，用于根据工作节点 ID 解析 IP 和端口。
    /// </summary>
    /// <param name="resolver">要注入的终结点解析器。</param>
    /// <returns>当前建造器实例，支持链式调用。</returns>
    public ChannelConfigBuilder UseEndpointResolver(IEndpointResolver resolver)
    {
        EndpointResolver = resolver;
        return this;
    }

    /// <summary>
    /// 设置自定义的 <see cref="IChannelFactory"/>，用于创建通信通道。
    /// </summary>
    /// <param name="factory">要注入的通道工厂。</param>
    /// <returns>当前建造器实例，支持链式调用。</returns>
    public ChannelConfigBuilder UseChannelFactory(IChannelFactory factory)
    {
        ChannelFactory = factory;
        return this;
    }

    internal void Apply()
    {
        if (EndpointResolver != null)
            ChannelConfig.SetEndpointResolver(EndpointResolver);
        if (ChannelFactory != null)
            ChannelConfig.SetChannelFactory(ChannelFactory);
    }
}