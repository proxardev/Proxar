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
using TestShared;

namespace ServiceIntegrationTesting;


[ServiceExport(1)]
public partial class TestService_ReceiveMessage : ServiceBase
{
    [ServiceMethod(1)]
    [ServiceMethodExport]
    private (byte, short, ushort, int, uint, long, ulong, float, double) SimpleArgsReceive(
        byte args1, short args2, ushort args3, int args4,
        uint args5, long args6, ulong args7, float args8, double args9)
    {
        return (args1, args2, args3, args4, args5, args6, args7, args8, args9);
    }

    [ServiceMethod(2)]
    [ServiceMethodExport]
    private (string, decimal) SimpleArgsReceive2(string args1, decimal args2)
    {
        return (args1, args2);
    }

    [ServiceMethod(897)]
    [ServiceMethodExport]
    private bool ThrowException()
    {
        IntergrationTestResultActorSingleton.Current.FailAfterExecGlobalResultAction = false;
        var proxy = Service.GetServiceProxy<TestService_ReceiveMessageProxy>(this.GetServiceId());
        proxy.Queue0.SetFailAfterExecGlobalResultAction();

        throw new Exception("Dispatch_897_ThrowException");
    }

    [ServiceMethod(898)]
    [ServiceMethodAction(false, true)]
    [ServiceMethodExport]
    private void SetFailAfterExecGlobalResultAction()
    {
        IntergrationTestResultActorSingleton.Current.FailAfterExecGlobalResultAction = true;
    }
}