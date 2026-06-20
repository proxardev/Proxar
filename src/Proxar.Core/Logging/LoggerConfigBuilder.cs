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

namespace Proxar.Logging;


/// <summary>
/// 用于构建并配置 <see cref="ProxarLogger"/> 的建造器。
/// 业务启动代码通过此类设置日志工厂和全局异常回调，配置会在宿主启动时自动应用。
/// </summary>
public class LoggerConfigBuilder
{
    private ILoggerFactory loggerFactory = null!;
    private Action<Exception, string> errorAction = null!;

    /// <summary>
    /// 设置要注入的 <see cref="ILoggerFactory"/> 实例。
    /// </summary>
    /// <param name="loggerFactory">日志工厂，例如 NLog、Serilog 的适配工厂。</param>
    /// <returns>当前建造器实例，支持链式调用。</returns>
    public LoggerConfigBuilder SetLoggerFactory(ILoggerFactory loggerFactory)
    {
        this.loggerFactory = loggerFactory;
        return this;
    }

    /// <summary>
    /// 设置全局异常回调，当 <see cref="ProxarLogger.Error(Exception, string)"/> 被调用时触发。
    /// </summary>
    /// <param name="action">异常回调，参数为异常对象和附加消息。</param>
    /// <returns>当前建造器实例，支持链式调用。</returns>
    public LoggerConfigBuilder SetErrorAction(Action<Exception, string> action)
    {
        this.errorAction = action;
        return this;
    }

    internal void Apply()
    {
        ProxarLogger.SetLoggerFactory(loggerFactory);
        ProxarLogger.SetErrorAction(errorAction);
    }
}