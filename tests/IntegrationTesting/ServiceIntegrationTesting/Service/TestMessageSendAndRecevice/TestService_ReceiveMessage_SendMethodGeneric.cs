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

namespace ServiceIntegrationTesting;

public partial class TestService_ReceiveMessage
{
    [ServiceMethod(170)]
    [ServiceMethodExport]
    private void Send0DataArgs(long serviceId, int msgIdx)
    {
        Service.GetServiceProxy<TestService_MessageProxy>(serviceId)
               .ReceiceMsgIdxResult(msgIdx, 100);
    }

    [ServiceMethod(171)]
    [ServiceMethodExport]
    private void Send1DataArg(long serviceId, int msgIdx, int arg1)
    {
        Service.GetServiceProxy<TestService_MessageProxy>(serviceId)
               .ReceiceMsgIdxResult(msgIdx, arg1 * 3);
    }

    [ServiceMethod(172)]
    [ServiceMethodExport]
    private void Send2DataArgs(long serviceId, int msgIdx, int arg1, int arg2)
    {
        Service.GetServiceProxy<TestService_MessageProxy>(serviceId)
               .ReceiceMsgIdxResult(msgIdx, (arg1 + arg2) * 3);
    }

    [ServiceMethod(173)]
    [ServiceMethodExport]
    private void Send3DataArgs(long serviceId, int msgIdx, int arg1, int arg2, int arg3)
    {
        Service.GetServiceProxy<TestService_MessageProxy>(serviceId)
               .ReceiceMsgIdxResult(msgIdx, arg1 * arg2 * arg3);
    }

    [ServiceMethod(174)]
    [ServiceMethodExport]
    private void Send4DataArgs(long serviceId, int msgIdx, int arg1, int arg2, int arg3, int arg4)
    {
        Service.GetServiceProxy<TestService_MessageProxy>(serviceId)
               .ReceiceMsgIdxResult(msgIdx, arg1 + arg2 + arg3 + arg4 + 50);
    }

    [ServiceMethod(175)]
    [ServiceMethodExport]
    private void Send5DataArgs(long serviceId, int msgIdx, int arg1, int arg2, int arg3, int arg4, int arg5)
    {
        Service.GetServiceProxy<TestService_MessageProxy>(serviceId)
               .ReceiceMsgIdxResult(msgIdx, (arg1 + arg2 + arg3 + arg4 + arg5) * 3);
    }

    [ServiceMethod(176)]
    [ServiceMethodExport]
    private void Send6DataArgs(long serviceId, int msgIdx, int arg1, int arg2, int arg3, int arg4, int arg5, int arg6)
    {
        Service.GetServiceProxy<TestService_MessageProxy>(serviceId)
               .ReceiceMsgIdxResult(msgIdx, arg1 + arg2 + arg3 + arg4 + arg5 + arg6 + 200);
    }

    [ServiceMethod(177)]
    [ServiceMethodExport]
    private void Send7DataArgs(long serviceId, int msgIdx, int arg1, int arg2, int arg3, int arg4, int arg5, int arg6, int arg7)
    {
        Service.GetServiceProxy<TestService_MessageProxy>(serviceId)
               .ReceiceMsgIdxResult(msgIdx, arg1 + arg2 + arg3 + arg4 + arg5 + arg6 + arg7);
    }
}