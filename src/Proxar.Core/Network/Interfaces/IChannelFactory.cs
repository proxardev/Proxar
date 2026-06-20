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
/// 定义<see cref="IChannel"/>工厂接口，用于根据指定的 IP 地址和端口创建<see cref="IChannel"/>。
/// </summary>
public interface IChannelFactory
{
    /// <summary>
    /// 异步创建并连接到指定终端的通道。
    /// </summary>
    /// <param name="ip">目标 IP 地址。</param>
    /// <param name="port">目标端口。</param>
    /// <returns>代表异步连接操作的任务，完成后返回 <see cref="IChannel"/> 实例。</returns>
    Task<IChannel> CreateChannel(string ip, int port);
}