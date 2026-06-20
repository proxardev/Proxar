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


using Microsoft.Extensions.Logging;
using Proxar.Logging;
using Proxar.Network;
using Proxar.ServiceCore;

namespace Proxar.AppHost;

public partial class AppHostBuilder
{
    /// <summary>
    /// 配置网络通道，允许注入自定义的 <see cref="Proxar.Network.Interfaces.IEndpointResolver"/> 和 <see cref="Proxar.Network.Interfaces.IChannelFactory"/> 实现。
    /// </summary>
    /// <param name="action">用于配置 <see cref="ChannelConfigBuilder"/> 的回调。</param>
    /// <returns>当前构建器实例，支持链式调用。</returns>
    public AppHostBuilder ConfigureChannel(Action<ChannelConfigBuilder> action)
    {
        var builder = new ChannelConfigBuilder();
        action.Invoke(builder);
        builder.Apply();
        return this;
    }

    /// <summary>
    /// 配置服务相关选项，例如服务 ID 生成策略或全局超时设置。
    /// </summary>
    /// <param name="action">用于配置 <see cref="ServiceConfigBuilder"/> 的回调。</param>
    /// <returns>当前构建器实例，支持链式调用。</returns>
    public AppHostBuilder ConfigureService(Action<ServiceConfigBuilder> action)
    {
        var builder = new ServiceConfigBuilder();
        action.Invoke(builder);
        return this;
    }

    /// <summary>
    /// 配置日志系统，注入 <see cref="ILoggerFactory"/> 实例并设置全局异常回调。
    /// </summary>
    /// <param name="action">用于配置 <see cref="LoggerConfigBuilder"/> 的回调。</param>
    /// <returns>当前构建器实例，支持链式调用。</returns>
    public AppHostBuilder ConfigureLogger(Action<LoggerConfigBuilder> action)
    {
        var builder = new LoggerConfigBuilder();
        action.Invoke(builder);
        builder.Apply();
        return this;
    }
}