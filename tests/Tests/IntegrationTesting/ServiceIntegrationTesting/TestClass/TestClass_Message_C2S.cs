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
using ServiceIntegrationTesting.TestMessageSendAndRecevice;
using System.Net;
using TestShared;
using TouchSocket.Sockets;
namespace ServiceIntegrationTesting.TestClass;


[Collection("TestExternalMessageCollection")]
public class TestClass_Message_C2S : TestClass_MessageBase<TestService_Message, TestService_Message_ExternalProxy>
{
    protected override void OnBeforeCallMethod()
    {
        TestConfig.onClientChannelConnectedCallback = onClientChannelConnectedCallback;
    }


    private static async Task CreateAndRegisterClientChannelAsync(long clientServiceId)
    {
        var ip = ProxarHost.Instance.AppOptions.ClientIp;
        var port = ProxarHost.Instance.AppOptions.ClientPort;
        var ipHost = new IPHost(IPAddress.Parse(ip), port);
        var clientChannel = await TouchSocketChannel.ConnectAsync(ipHost);
        clientChannel.Id = 1;
        clientChannel.MessageReceived += (channel, readOnlyMemory) =>
        {
            TestInitializer.ClientServiceGroup.ServiceGroupExecute(() =>
                Channel_MessageReceived(channel, readOnlyMemory)
                );
        };

        var targetId = TestExternalMessageChannelHelper.RoleId;
        TestInitializer.ClientServiceGroup.ServiceGroupExecute(
            () =>
            {
                ProxarHost.Instance.GateMessageInvoker
                    .AddChannel(clientChannel);
                ProxarHost.Instance.GateMessageInvoker
                    .SetTarget2Channel(targetId, clientChannel.Id);
                ProxarHost.Instance.GateMessageInvoker
                    .SetTargetServiceMapping(targetId, TestService_CustomReceiveMessageExternalProxy.ProxyId, clientServiceId);
            }
        );
    }

    protected override async Task<long> InvokeCreateService(Func<Task<long>> func)
    {
        Task<long> task = null!;
        var asyncAction = async () =>
        {
            var clientServiceId = await func();
            await CreateAndRegisterClientChannelAsync(clientServiceId);
            return clientServiceId;
        };
        var action = () =>
        {
            task = asyncAction.Invoke();
        };
        TestInitializer.ClientServiceGroup.ServiceGroupExecute(action);
        return await task;
    }

    private static void onClientChannelConnectedCallback(IChannel channel)
    {
        var targetId = TestExternalMessageChannelHelper.RoleId;
        channel.Id = 0;
        channel.MessageReceived += (channel, readOnlyMemory) =>
        {
            TestInitializer.ServerServiceGroup.ServiceGroupExecute(() =>
                Channel_MessageReceived(channel, readOnlyMemory)
                );
        };

        TestInitializer.ServerServiceGroup.ServiceGroupExecute(
            () =>
            {
                var receiveServiceId = Service.CreateService<TestService_ReceiveMessage>();
                ProxarHost.Instance.GateMessageInvoker
                    .AddChannel(channel);
                ProxarHost.Instance.GateMessageInvoker
                    .SetTarget2Channel(targetId, channel.Id);
                ProxarHost.Instance.GateMessageInvoker
                    .SetTargetServiceMapping(targetId, TestService_CustomReceiveMessageExternalProxy.ProxyId, receiveServiceId);
            }
        );
    }

    protected override void CreateServiceCallBack(long serviceId)
    {
        var proxy = Service.GetServiceProxy<TestService_MessageProxy>(serviceId);

        var targetId = TestExternalMessageChannelHelper.RoleId;
        var receivedProxy = new TestService_CustomReceiveMessageExternalProxy(targetId);
        proxy.Raw.SetTargetProxy2(receivedProxy);

        TestInitializer.ServerServiceGroup.ServiceGroupExecute(() =>
        {
            var receiveServiceId = Service.CreateService<TestService_ReceiveMessage>();
        });
    }


    private static void Channel_MessageReceived(IChannel channel, ReadOnlyMemory<byte> readOnlyMemory)
    {
        ProxarHost.Instance.GateMessageInvoker
            .ReceiveChannelData(channel, readOnlyMemory.ToArray());
    }

    protected override object CreateProxy()
    {
        return Service.GetServiceProxy<TestService_MessageProxy>(this.serviceId);
    }

}