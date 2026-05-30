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
using ServiceIntegrationTesting.Models;

namespace ServiceIntegrationTesting;

public partial class TestService_ReceiveMessage
{
    [ServiceMethod(20)]
    [ServiceMethodExport]
    private string StringArgsReceive(string args1) => args1.ToUpper();

    [ServiceMethod(21)]
    [ServiceMethodExport]
    private int EnumStatusCodeArgsReceive(StatusCode args1) => (int)args1 * 5 + 2;

    [ServiceMethod(22)]
    [ServiceMethodExport]
    private int EnumPriorityArgsReceive(Priority args1) => (int)args1 * 10;

    [ServiceMethod(23)]
    [ServiceMethodExport]
    private int EnumFlagsArgsReceive(PermissionFlags args1) => (int)args1 + 100;

    [ServiceMethod(24)]
    [ServiceMethodExport]
    private byte EnumByteArgsReceive(ByteEnum args1) => (byte)((byte)args1 + 1);

    // 425-429: 预留
}