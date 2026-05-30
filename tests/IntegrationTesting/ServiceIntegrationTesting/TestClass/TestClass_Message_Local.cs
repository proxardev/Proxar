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
namespace ServiceIntegrationTesting.TestClass;

public class TestClass_Message_Local : TestClass_MessageBase<TestService_Message, TestService_MessageProxy>
{

    protected override void CreateServiceCallBack(long serviceId)
    {
        var receiveServiceId = Service.CreateService<TestService_ReceiveMessage>();
        var proxy = Service.GetServiceProxy<TestService_MessageProxy>(serviceId);
        proxy.SetTargetTestService(receiveServiceId);
    }

    protected override object CreateProxy()
    {
        return Service.GetServiceProxy<TestService_MessageProxy>(this.serviceId);
    }
}