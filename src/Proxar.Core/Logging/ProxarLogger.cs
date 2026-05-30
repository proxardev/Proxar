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

public static class ProxarLogger
{
    private static ILoggerFactory _loggerFactory = NullLoggerFactory.Instance;
    public static ILogger Default { get; private set; } = NullLoggerFactory.Instance.CreateLogger("Server");

    internal static void SetLoggerFactory(ILoggerFactory factory)
    {
        if (factory == null) return;
        _loggerFactory = factory;
        Default = factory.CreateLogger("Server");
    }

    public static ILogger<T> CreateLogger<T>() => _loggerFactory.CreateLogger<T>();


    public static Action<Exception, string>? ErrorAction { get; private set; } = null;

    internal static void SetErrorAction(Action<Exception, string> action)
    {
        ErrorAction = action;
    }

    public static void Console(string message)
    {
        System.Console.WriteLine($"{TimeHelper.MilliTimeStr()} {message}");
    }

    public static void Info(string message)
    {
        Default.LogInformation(message);
    }

    [Conditional("DEBUG")]
    public static void TestDebugLog(string message)
    {
        Default.LogInformation(message);
    }

    public static void Error(string message)
    {
        Default.LogError(message);
    }

    public static void Error(Exception ex, string message)
    {
        var oldMsg = message;
        message = $"{message}\n{ex.ToString()}";
        Default.LogError(ex, message);
        ErrorAction?.Invoke(ex, oldMsg);
    }

    public static void Error(Exception ex)
    {
        var message = ex.ToString();
        Default.LogError(message);
        ErrorAction?.Invoke(ex, "");
    }
}