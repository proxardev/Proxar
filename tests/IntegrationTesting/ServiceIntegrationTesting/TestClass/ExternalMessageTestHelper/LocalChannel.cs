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
using Proxar.Network;
using Proxar.Network.Interfaces;

namespace ServiceIntegrationTesting;



public sealed class LocalChannel : IChannel
{
    public string StrId { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    public bool IsConnected => true;

    public long Id { get; set; }

    public event Action<IChannel, ReadOnlyMemory<byte>> MessageReceived;
#pragma warning disable CS0067
    public event Action<IChannel, Exception> ErrorOccurred;
    public event Action<IChannel, string> Disconnected;
#pragma warning restore CS0067

    private LocalChannel channel = null!;

#pragma warning disable CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑添加 "required" 修饰符或声明为可为 null。
    public LocalChannel()
#pragma warning restore CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑添加 "required" 修饰符或声明为可为 null。
    {
        Id = ProxarHost.Instance.SnowflakeIdGenerator.NewId();
    }

    public void SetCommunicationChannel(LocalChannel channel)
    {
        this.channel = channel;
    }

    public void Close(string reason)
    {
        throw new NotImplementedException();
    }

    public string GetRemoteIp()
    {
        throw new NotImplementedException();
    }

    public int GetRemotePort()
    {
        throw new NotImplementedException();
    }

    public void Send(Proxar.Network.INetMessage message)
    {
        var buffer = new PooledBufferWriter(64);
        message.NetMessageSerialize(buffer);
        var writtenMemory = buffer.WrittenMemory;
        this.channel.RaiseReceivedDataEvent(writtenMemory);
        message.Dispose();
        buffer.Dispose();
    }

    internal void RaiseReceivedDataEvent(ReadOnlyMemory<byte> data)
    {
        MessageReceived?.Invoke(this, data);
    }
}