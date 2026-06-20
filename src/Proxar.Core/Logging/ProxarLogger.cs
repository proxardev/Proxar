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
using Microsoft.Extensions.Logging.Abstractions;
using Proxar.Utilities;
using System.Diagnostics;

namespace Proxar.Logging;

/// <summary>
/// Proxar 框架的全局日志门面，提供统一的日志记录和异常报告能力。
/// 业务代码通过静态方法直接使用，无需依赖具体的日志实现。
/// 配置通过 <see cref="LoggerConfigBuilder"/> 注入，在宿主启动时自动生效。
/// </summary>
public static class ProxarLogger
{
    private static ILoggerFactory _loggerFactory = NullLoggerFactory.Instance;

    /// <summary>
    /// 全局默认日志器，类别为 "Server"。
    /// 在注入 <see cref="ILoggerFactory"/> 之前使用 <see cref="NullLogger"/>，不产生任何输出。
    /// </summary>
    public static ILogger Default { get; private set; } = NullLoggerFactory.Instance.CreateLogger("Server");

    /// <summary>
    /// （内部使用）注入日志工厂。由 <see cref="LoggerConfigBuilder.Apply"/> 调用。
    /// </summary>
    /// <param name="factory">日志工厂实例。</param>
    internal static void SetLoggerFactory(ILoggerFactory factory)
    {
        if (factory == null) return;
        _loggerFactory = factory;
        Default = factory.CreateLogger("Server");
    }

    /// <summary>
    /// 创建指定类型的 <see cref="ILogger{T}"/> 实例，用于需要分类日志的场景。
    /// </summary>
    /// <typeparam name="T">日志类别类型。</typeparam>
    public static ILogger<T> CreateLogger<T>() => _loggerFactory.CreateLogger<T>();

    /// <summary>
    /// 全局异常回调，在 <see cref="Error(Exception, string)"/> 或 <see cref="Error(Exception)"/> 被调用时触发。
    /// 通过 <see cref="LoggerConfigBuilder.SetErrorAction"/> 设置。
    /// </summary>
    public static Action<Exception, string>? ErrorAction { get; private set; } = null;

    /// <summary>
    /// （内部使用）设置全局异常回调。
    /// </summary>
    internal static void SetErrorAction(Action<Exception, string> action)
    {
        ErrorAction = action;
    }

    /// <summary>
    /// 输出带毫秒时间戳的消息到控制台，仅在 Debug 编译模式下有效。
    /// </summary>
    [Conditional("Debug")]
    public static void Console(string message)
    {
        System.Console.WriteLine($"{TimeHelper.MilliTimeStr()} {message}");
    }

    /// <summary>
    /// 记录一条信息级别的日志。
    /// </summary>
    public static void Info(string message)
    {
        Default.LogInformation(message);
    }

    /// <summary>
    /// 在 DEBUG 模式下记录一条信息级别的调试日志，Release 版本自动移除调用。
    /// </summary>
    [Conditional("DEBUG")]
    public static void TestDebugLog(string message)
    {
        Default.LogInformation(message);
    }

    /// <summary>
    /// 记录一条错误级别的日志。
    /// </summary>
    public static void Error(string message)
    {
        Default.LogError(message);
    }

    /// <summary>
    /// 记录一条错误日志并附带异常，同时触发 <see cref="ErrorAction"/> 回调。
    /// </summary>
    /// <param name="ex">发生的异常。</param>
    /// <param name="message">附加错误消息。</param>
    public static void Error(Exception ex, string message)
    {
        var oldMsg = message;
        message = $"{message}\n{ex.ToString()}";
        Default.LogError(ex, message);
        ErrorAction?.Invoke(ex, oldMsg);
    }

    /// <summary>
    /// 记录异常的错误日志，同时触发 <see cref="ErrorAction"/> 回调。
    /// </summary>
    public static void Error(Exception ex)
    {
        var message = ex.ToString();
        Default.LogError(message);
        ErrorAction?.Invoke(ex, "");
    }
}