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


using Proxar.ServiceCore;
using Proxar.ServiceCore.Interfaces;

namespace ServiceIntegrationTesting.TestMessageSendAndRecevice;

public class TestService_CustomReceiveMessageExternalProxy : TestService_ReceiveMessage_ExternalProxy, ICustomTestService_ReceiveMessageProxy
{
    public TestService_CustomReceiveMessageExternalProxy(long serviceId) : base(serviceId)
    {
    }

    public TestService_CustomReceiveMessageExternalProxy(long serviceId, IMessageInvoker messageInvoker) : base(serviceId, messageInvoker)
    {
    }

    new static IServiceProxy Create(long serviceId)
    {
        throw new NotImplementedException();
    }

    new static IServiceProxy Create(long serviceId, IMessageInvoker messageInvoker)
    {
        throw new NotImplementedException();
    }


    static TestService_CustomReceiveMessageExternalProxy()
    {
        ProxyFactory<TestService_CustomReceiveMessageExternalProxy>.Register(CreateProxy);
        ProxyFactory<TestService_CustomReceiveMessageExternalProxy>.RegisterCreateWithInvoker(CreateProxy);
    }

    private static TestService_CustomReceiveMessageExternalProxy CreateProxy(long serviceId)
    {
        return new TestService_CustomReceiveMessageExternalProxy(serviceId);
    }

    private static TestService_CustomReceiveMessageExternalProxy CreateProxy(long serviceId, IMessageInvoker messageInvoker)
    {
        return new TestService_CustomReceiveMessageExternalProxy(serviceId, messageInvoker);
    }
}