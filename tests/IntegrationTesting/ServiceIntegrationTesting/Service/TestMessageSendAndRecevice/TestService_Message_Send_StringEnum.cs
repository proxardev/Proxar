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


using FluentAssertions;
using Proxar.ServiceCore;
using Proxar.Tasks;
using ServiceIntegrationTesting.Models;

namespace ServiceIntegrationTesting;

public partial class TestService_Message
{
    // 20: string
    [ServiceMethod(20)]
    [ServiceMethodExport]
    private async ZFTask String_SendReceiveSucc()
    {
        const string input = "hello";
        var result = await this.GetTargetServiceProxy().StringArgsReceive(input);
        result.Should().Be(input.ToUpper());
        TestResultHelper.SetFinishTest();
    }

    // 21: enum StatusCode
    [ServiceMethod(21)]
    [ServiceMethodExport]
    private async ZFTask EnumStatusCode_SendReceiveSucc()
    {
        const StatusCode input = StatusCode.Pending;
        var result = await this.GetTargetServiceProxy().EnumStatusCodeArgsReceive(input);
        result.Should().Be((int)input * 5 + 2);
        TestResultHelper.SetFinishTest();
    }

    // 22: enum Priority
    [ServiceMethod(22)]
    [ServiceMethodExport]
    private async ZFTask EnumPriority_SendReceiveSucc()
    {
        const Priority input = Priority.High;
        var result = await this.GetTargetServiceProxy().EnumPriorityArgsReceive(input);
        result.Should().Be((int)input * 10);
        TestResultHelper.SetFinishTest();
    }

    // 23: [Flags] enum
    [ServiceMethod(23)]
    [ServiceMethodExport]
    private async ZFTask EnumFlags_SendReceiveSucc()
    {
        const PermissionFlags input = PermissionFlags.Read | PermissionFlags.Write;
        var result = await this.GetTargetServiceProxy().EnumFlagsArgsReceive(input);
        result.Should().Be((int)input + 100);
        TestResultHelper.SetFinishTest();
    }

    // 24: byte底层enum
    [ServiceMethod(24)]
    [ServiceMethodExport]
    private async ZFTask EnumByte_SendReceiveSucc()
    {
        const ByteEnum input = ByteEnum.C;
        var result = await this.GetTargetServiceProxy().EnumByteArgsReceive(input);
        result.Should().Be((byte)((byte)input + 1));
        TestResultHelper.SetFinishTest();
    }

    // 25-29: 预留
}