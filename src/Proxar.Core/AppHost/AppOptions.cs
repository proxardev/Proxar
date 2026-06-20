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


namespace Proxar.AppHost;

// TODO-接口抽象
/// <summary>
/// 程序全局配置
/// </summary>
public class AppOptions
{
    /// <summary>
    /// 工作机器Id
    /// </summary>
    public int WorkerId { get; init; }

    /// <summary>
    /// 集群内部通讯监听IP
    /// </summary>
    public string Ip { get; init; } = string.Empty;

    /// <summary>
    /// 集群内部通讯监听端口
    /// </summary>
    public int Port { get; init; }

    /// <summary>
    /// 对外客户端通讯监听IP（玩家连接入口）
    /// </summary>
    public string ClientIp { get; init; } = string.Empty;

    /// <summary>
    /// 对外客户端通讯监听端口
    /// </summary>
    public int ClientPort { get; init; }

    /// <summary>
    /// 集群配置文件
    /// </summary>
    public string ClusterConfigFile { get; init; } = "ClusterConfig.Json";
}