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
/// 定义网络通信通道的抽象，封装底层连接，提供消息收发和连接状态管理。
/// </summary>
public interface IChannel
{
    /// <summary>
    /// 获取或设置通道的字符串标识，通常用于日志和调试。
    /// </summary>
    string StrId { get; set; }

    /// <summary>
    /// 获取或设置通道的整数标识，用于内部快速索引。
    /// </summary>
    long Id { get; set; }

    /// <summary>
    /// 获取一个值，指示通道是否已连接且可用。
    /// </summary>
    bool IsConnected { get; }

    /// <summary>
    /// 获取远程终端的 IP 地址字符串。
    /// </summary>
    /// <returns>远程 IP 地址。</returns>
    string GetRemoteIp();

    /// <summary>
    /// 获取远程终端的端口号。
    /// </summary>
    /// <returns>远程端口。</returns>
    int GetRemotePort();

    /// <summary>
    /// 发送一个网络消息。
    /// </summary>
    /// <param name="message">要发送的消息，实现 <see cref="INetMessage"/> 接口。</param>
    void Send(INetMessage message);

    /// <summary>
    /// 关闭通道，释放连接资源。
    /// </summary>
    /// <param name="reason">关闭原因描述。</param>
    void Close(string reason);

    /// <summary>
    /// 当通道接收到完整消息时触发。
    /// </summary>
    event Action<IChannel, ReadOnlyMemory<byte>> MessageReceived;

    /// <summary>
    /// 当通道发生通信异常时触发。
    /// </summary>
    event Action<IChannel, Exception> ErrorOccurred;

    /// <summary>
    /// 当通道断开连接时触发。
    /// </summary>
    event Action<IChannel, string> Disconnected;
}