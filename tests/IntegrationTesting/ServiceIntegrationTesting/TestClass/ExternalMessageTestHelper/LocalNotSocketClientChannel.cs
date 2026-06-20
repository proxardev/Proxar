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
using Proxar.ServiceCore;
using Proxar.ServiceCore.Message;

namespace ServiceIntegrationTesting;



public sealed class LocalNotSocketClientChannel : IChannel
{
    public string StrId { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    public bool IsConnected => true;

    public long Id { get; set; }

    public event Action<IChannel, ReadOnlyMemory<byte>> MessageReceived;
#pragma warning disable CS0067
    public event Action<IChannel, Exception> ErrorOccurred;
    public event Action<IChannel, string> Disconnected;
#pragma warning restore CS0067




#pragma warning disable CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑添加 "required" 修饰符或声明为可为 null。
    public LocalNotSocketClientChannel(long serviceId)
#pragma warning restore CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑添加 "required" 修饰符或声明为可为 null。
    {
        this.MessageReceived += (c, r) => ClientChannel_MessageReceived(serviceId, c, r);
        Id = ProxarHost.Instance.SnowflakeIdGenerator.NewId();
    }


    private static void ClientChannel_MessageReceived(long serviceId, IChannel channel, ReadOnlyMemory<byte> readOnlyMemory)
    {
        var clientNetMessage = new ClientNetMessage(readOnlyMemory.ToArray());
        clientNetMessage.NetMessageDeserialize();
        clientNetMessage.SetToServiceId(serviceId);
        clientNetMessage.SetFromServiceId(0);
        Service.MessageInvoker.Send(0, serviceId, clientNetMessage);
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

    private LocalChannel serverChannel;
    public void SetServerChannal(LocalChannel channel)
    {
        this.serverChannel = channel;
    }

    public void Send(Proxar.Network.INetMessage message)
    {
        var buffer = new PooledBufferWriter(64);
        message.NetMessageSerialize(buffer);
        var writtenMemory = buffer.WrittenMemory;
        serverChannel.RaiseReceivedDataEvent(writtenMemory);

    }

    internal void RaiseReceivedDataEvent(ReadOnlyMemory<byte> data)
    {
        MessageReceived?.Invoke(this, data);
    }
}