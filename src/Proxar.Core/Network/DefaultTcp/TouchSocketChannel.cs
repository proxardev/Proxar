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


using Proxar.Tasks;
using System.Buffers.Binary;
using System.Net;
using TouchSocket.Core;
using TouchSocket.Sockets;

namespace Proxar.Network;

public class TouchSocketChannel : IChannel, IDisposable
{
    private readonly ITcpClient tcpClient; // 用于主动发起的连接
    private readonly ITcpSessionClient sessionClient; // 用于被动接受的连接
    private bool connected = false;

    public string StrId { get; set; }
    private EndPoint RemoteEndPoint { get; set; } // 记录远程端点，方便调试


    private TouchSocketChannel(ITcpClient client)
    {
        tcpClient = client;
        StrId = string.Empty;
        sessionClient = null!;
        RemoteEndPoint = client.RemoteEndPoint;
        RegisterClientEvents(client);
    }

    private TouchSocketChannel(ITcpSessionClient session)
    {
        sessionClient = session;
        tcpClient = null!;
        StrId = session.Id;
        session.SetChannel(this);
        RemoteEndPoint = session.RemoteEndPoint;
        RegisterSessionEvents(sessionClient);
    }

    // 用于由客户端发起的连接
    public static async Task<TouchSocketChannel> ConnectAsync(IPHost remoteEndPoint)
    {
        var client = new TcpClient();
        await client.SetupAsync(new TouchSocketConfig()
            .SetRemoteIPHost(remoteEndPoint)
            .ConfigurePlugins(a =>
                {
                    a.Add<TcpServiceReceivedPlugin>();
                }
            )
            );
        await client.ConnectAsync();
        var channel = new TouchSocketChannel(client);
        client.SetChannel(channel);
        channel.connected = true;
        return channel;
    }

    // 用于由服务端接受的连接
    public static TouchSocketChannel FromSession(ITcpSessionClient session)
    {
        var channel = new TouchSocketChannel(session);
        channel.connected = true;
        return channel;
    }

    public bool IsConnected => ValidConnected();

    public long Id { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    private bool ValidConnected()
    {
        if (tcpClient == null && sessionClient == null)
        {
            return false;
        }
        return connected;
    }

    // === 事件定义 ===
    public event Action<IChannel, ReadOnlyMemory<byte>> MessageReceived = null!;
    public event Action<IChannel, Exception> ErrorOccurred = null!;
    public event Action<IChannel, string> Disconnected = null!;

    #region 事件注册
    private static void RegisterClientEvents(ITcpClient tcpClient)
    {
        tcpClient.Closed = (client, e) =>
        {
            var channel = tcpClient.GetChannel<TouchSocketChannel>();
            if (channel.connected)
            {
                channel.RaiseDisconnectedEvent(e.Message);
                channel.connected = false;
            }
            return Task.CompletedTask;
        };
    }

    private static void RegisterSessionEvents(ITcpSessionClient tcpSessionClient)
    {
        tcpSessionClient.Closed = (session, e) =>
        {
            var channel = tcpSessionClient.GetChannel<TouchSocketChannel>();
            if (channel.connected)
            {
                channel.RaiseDisconnectedEvent(e.Message);
                channel.connected = false;
            }
            return Task.CompletedTask;
        };
    }
    #endregion

    #region 发送与关闭
    public void Send(INetMessage message)
    {
        if (!IsConnected)
        {
            return;
        }
        SendAsync(message).Coroutine();
    }

    public async ZFTask SendAsync(INetMessage message)
    {
        try
        {
            var headSize = 4;
            var buffer = new PooledBufferWriter(64);
            var span = buffer.GetSpan(headSize);
            buffer.Advance(headSize);
            message.NetMessageSerialize(buffer);
            var writtenMemory = buffer.WrittenMemory;
            BinaryPrimitives.WriteInt32LittleEndian(writtenMemory.Span[..headSize], writtenMemory.Length - headSize);
            if (tcpClient != null)
            {
                await tcpClient.SendAsync(writtenMemory);
            }
            else
            {
                await sessionClient.SendAsync(writtenMemory);
            }
            message.Dispose();
            buffer.Dispose();
        }
        catch (Exception e)
        {
            RaiseErrorOccurredEvent(e);
        }
    }

    public void Close(string reason = "Normal")
    {
        if (!connected) return;
        connected = false;
        CloseAsync().Coroutine();
    }

    public async ZFTask CloseAsync(string reason = "Normal")
    {

        if (tcpClient != null)
        {
            await tcpClient.CloseAsync(reason);
            tcpClient.SafeDispose();
        }
        else
        {
            await sessionClient.CloseAsync(reason);
            sessionClient.SafeDispose();
        }
    }
    #endregion

    // 实现 IDisposable 以便清理资源
    public void Dispose()
    {
        Close("Disposed");
    }

    internal void RaiseReceivedDataEvent(ReadOnlyMemory<byte> data)
    {
        MessageReceived?.Invoke(this, data);
    }

    internal void RaiseDisconnectedEvent(string reason)
    {
        Disconnected?.Invoke(this, reason);
    }

    internal void RaiseErrorOccurredEvent(Exception exception)
    {
        ErrorOccurred?.Invoke(this, exception);
    }

    public string GetRemoteIp()
    {
        if (RemoteEndPoint == null)
        {
            return string.Empty;
        }
        return RemoteEndPoint.GetIP();
    }

    public int GetRemotePort()
    {
        if (RemoteEndPoint == null)
        {
            return 0;
        }
        return RemoteEndPoint.GetPort();
    }
}