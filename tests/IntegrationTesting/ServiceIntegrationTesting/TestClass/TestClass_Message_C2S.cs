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
using Proxar.ServiceCore;
using ServiceIntegrationTesting.TestMessageSendAndRecevice;
using TestShared;
namespace ServiceIntegrationTesting.TestClass;


[Collection("TestExternalMessageCollection")]
public class TestClass_Message_C2S : TestClass_MessageBase<TestService_Message, TestService_Message_ExternalProxy>
{

    protected override async Task<long> InvokeCreateService(Func<Task<long>> func)
    {
        Task<long> task = null!;
        var action = () =>
        {
            task = func();
        };
        TestInitializer.ClientServiceGroup.ServiceGroupExecute(action);
        return await task;
    }

    private static void CreateChannel(long clientServiceId, long serverServiceId)
    {
        var targetId = TestExternalMessageChannelHelper.RoleId;
        var clientChannel = new LocalChannel();
        var serverChannel = new LocalChannel();
        clientChannel.SetCommunicationChannel(serverChannel);
        serverChannel.SetCommunicationChannel(clientChannel);

        var x = ActorThreadScope.ServiceGroup;


        clientChannel.MessageReceived += (channel, readOnlyMemory) =>
        {
            TestInitializer.ClientServiceGroup.ServiceGroupExecute(() =>
                Channel_MessageReceived(channel, readOnlyMemory)
                );
        };
        serverChannel.MessageReceived += (channel, readOnlyMemory) =>
        {
            TestInitializer.ServerServiceGroup.ServiceGroupExecute(() =>
                Channel_MessageReceived(channel, readOnlyMemory)
                );
        };

        TestInitializer.ClientServiceGroup.ServiceGroupExecute(
            () =>
            {
                Game.Instance.GateMessageInvoker
                    .AddChannel(clientChannel);
                Game.Instance.GateMessageInvoker
                    .SetTarget2Channel(targetId, clientChannel.Id);
                Game.Instance.GateMessageInvoker
                    .SetTargetServiceMapping(targetId, TestService_CustomReceiveMessageExternalProxy.ProxyId, clientServiceId);
            }
        );

        TestInitializer.ServerServiceGroup.ServiceGroupExecute(
            () =>
            {
                Game.Instance.GateMessageInvoker
                    .AddChannel(serverChannel);
                Game.Instance.GateMessageInvoker
                    .SetTarget2Channel(targetId, serverChannel.Id);
                Game.Instance.GateMessageInvoker
                    .SetTargetServiceMapping(targetId, TestService_CustomReceiveMessageExternalProxy.ProxyId, serverServiceId);
            }
        );
    }

    protected override void CreateServiceCallBack(long serviceId)
    {
        var proxy = Service.GetServiceProxy<TestService_MessageProxy>(serviceId);
        var receiveServiceId = Service.CreateService<TestService_ReceiveMessage>();

        var targetId = TestExternalMessageChannelHelper.RoleId;
        var receivedProxy = new TestService_CustomReceiveMessageExternalProxy(targetId);

        proxy.Raw.SetTargetProxy2(receivedProxy);

        CreateChannel(serviceId, receiveServiceId);
    }


    private static void Channel_MessageReceived(IChannel channel, ReadOnlyMemory<byte> readOnlyMemory)
    {
        Game.Instance.GateMessageInvoker
            .ReceiveChannelData(channel, readOnlyMemory.ToArray());
    }

    protected override object CreateProxy()
    {
        return Service.GetServiceProxy<TestService_MessageProxy>(this.serviceId);
    }

}