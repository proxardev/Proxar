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


namespace Proxar.Network.Interfaces;

/// <summary>
/// 定义Ip端口解析器，用于根据工作节点 ID 解析出对应的 IP 地址和端口号。
/// </summary>
public interface IEndpointResolver
{
    /// <summary>
    /// 根据指定的工作节点 ID 解析网络ip和端口。
    /// </summary>
    /// <param name="workerId">工作节点的唯一标识符。</param>
    /// <returns>包含 IP 地址和端口号的元组。</returns>
    (string Ip, int Port) Resolve(long workerId);
}