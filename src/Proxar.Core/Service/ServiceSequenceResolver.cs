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



using Proxar.Core;
using Proxar.Logging;
using Proxar.ServiceCore.Interfaces;
using System.Collections.Concurrent;
using System.Text.Json;

namespace Proxar.ServiceCore;

/// <summary>
/// 知名服务序列号解析器。框架和业务各自维护独立的配置文件，
/// 启动时一次性加载到字典中
/// </summary>
public class ServiceSequenceResolver : Singleton<ServiceSequenceResolver>
{
    private readonly ConcurrentDictionary<string, int> _serviceSequences = new();
    private readonly ConcurrentDictionary<int, string> sequences2Services = new();


    public void Register<T>(int sequenceId) where T : ServiceBase, IKnownService
    {
        var name = typeof(T).FullName!;
        if (_serviceSequences.ContainsKey(name))
        {
            throw new Exception($"{name} unique service register repeat, now sequenceId {sequenceId}, old:{_serviceSequences[name]}");
        }
        if (sequences2Services.ContainsKey(sequenceId))
        {
            throw new Exception($"{sequenceId} unique service id register repeat, now sequenceId {name}, old:{sequences2Services[sequenceId]}");
        }
        _serviceSequences[name] = sequenceId;
        sequences2Services[sequenceId] = name;
    }

    public ServiceSequenceResolver()
    {
        // 加载框架内置配置
        LoadFromDirectory(Path.Combine(AppContext.BaseDirectory, "Config/ProxarCore/FrameworkServices.json"));

        // 加载业务方配置
        LoadFromDirectory(Path.Combine(AppContext.BaseDirectory, "Config/Business.json"));

        // 3. 支持通过环境变量或配置文件指定额外的目录（可选）
        var extraPath = Environment.GetEnvironmentVariable("PROXAR_EXTRA_CONFIG_PATH");
        if (!string.IsNullOrEmpty(extraPath))
        {
            LoadFromDirectory(extraPath);
        }
    }

    /// <summary>
    /// 从指定目录加载所有 JSON 配置文件，并将服务名和序列号合并到内部字典中。
    /// </summary>
    private void LoadFromDirectory(string jsonFiles)
    {
        if (!Directory.Exists(jsonFiles))
            return;

        try
        {
            var json = File.ReadAllText(jsonFiles);
            var dict = JsonSerializer.Deserialize<Dictionary<string, int>>(json);
            if (dict != null)
            {
                foreach (var kvp in dict)
                {
                    _serviceSequences[kvp.Key] = kvp.Value;
                }
            }
        }
        catch (Exception ex)
        {
            ProxarLogger.Error(ex, $"无法解析配置文件: {jsonFiles}");
        }

    }

    /// <summary>
    /// 根据服务名查找对应的序列号。业务配置优先，框架配置兜底。
    /// </summary>
    /// <param name="serviceName">服务名称，如 "Gateway"、"ChatService"</param>
    /// <returns>序列号，如果未找到则返回 null</returns>
    internal int? ResolveSequence(string serviceName)
    {
        if (_serviceSequences.TryGetValue(serviceName, out var sequence))
            return sequence;
        return null;
    }

    /// <summary>
    /// 判断指定名称的服务是否为知名服务
    /// </summary>
    internal bool IsKnownService(string serviceName)
    {
        return _serviceSequences.ContainsKey(serviceName);
    }
}