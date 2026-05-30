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


using Proxar.AppHost;
using System.Text.Json;

namespace Proxar.Network;

using ClusterConfigType = Dictionary<long, Dictionary<string, string>>;

internal class DefaultEndpointResolver : IEndpointResolver
{



    private static ClusterConfigType ClusterConfigInfos = null!;

    public delegate (string Ip, int Port) IpPortResolver(long workerId);

    public static IpPortResolver GetIpPort { get; private set; } = GetIpPortDefault;

    public static (string Ip, int Port) GetIpPortDefault(long workerId)
    {
        TryLoadClusterConfig(out var clusterConfigs);
        var infos = clusterConfigs[workerId];

        return (infos["Ip"], int.Parse(infos["Port"]));
    }

    private static bool TryLoadClusterConfig(out ClusterConfigType keyValuePairs)
    {
        if (ClusterConfigInfos != null)
        {
            keyValuePairs = ClusterConfigInfos;
            return true;
        }
        string filePath = Path.Combine(Environment.CurrentDirectory, Game.Instance.AppOptions.ClusterConfigFile);
        if (!File.Exists(filePath))
            throw new FileNotFoundException($"文件不存在: {filePath}");

        string jsonContent = File.ReadAllText(filePath);
        var dict = JsonSerializer.Deserialize<ClusterConfigType>(jsonContent)
                   ?? throw new InvalidOperationException("JSON 根对象不是有效的字典");
        keyValuePairs = dict;
        ClusterConfigInfos = dict;
        return true;
    }

    public (string Ip, int Port) Resolve(long workerId)
    {
        return GetIpPortDefault(workerId);
    }
}

