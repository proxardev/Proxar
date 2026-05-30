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


using Microsoft.Extensions.Configuration;
namespace Proxar.AppHost;

public static class AppOptionsLoader
{
    internal static T Load<T>(string[] args)
        where T : AppOptions, new()
    {
        var configArgs = new ConfigurationBuilder().AddCommandLine(args).Build();
        var configFile = configArgs["ConfigFile"];

        var builder = new ConfigurationBuilder();

        // 仅当 --config 提供时才加载自定义文件
        if (!string.IsNullOrEmpty(configFile))
        {
            builder.AddJsonFile(configFile, optional: false);
        }
        builder.AddCommandLine(args);

        var config = builder.Build();
        var result = new T();
        config.Bind(result);
        return result;
    }
}