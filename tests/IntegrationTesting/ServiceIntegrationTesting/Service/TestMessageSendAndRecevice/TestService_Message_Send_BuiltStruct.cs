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
    // 90: DateTime
    [ServiceMethod(90)]
    [ServiceMethodExport]
    private async ZFTask DateTime_SendReceiveSucc()
    {
        var input = new DateTime(2024, 1, 15, 10, 30, 0);
        var result = await this.GetTargetServiceProxy().DateTimeArgsReceive(input);
        result.Should().Be(input.AddDays(1));
        TestResultHelper.SetFinishTest();
    }

    // 91: Guid
    [ServiceMethod(91)]
    [ServiceMethodExport]
    private async ZFTask Guid_SendReceiveSucc()
    {
        var input = Guid.Parse("a1b2c3d4-e5f6-7890-abcd-ef1234567890");
        var result = await this.GetTargetServiceProxy().GuidArgsReceive(input);
        result.Should().Be(input);
        TestResultHelper.SetFinishTest();
    }

    // 92: ValueTuple (int, string, Point3D)
    [ServiceMethod(92)]
    [ServiceMethodExport]
    private async ZFTask ValueTuple_SendReceiveSucc()
    {
        var input = (42, "hello", new Point3D(1, 2, 3));
        var result = await this.GetTargetServiceProxy().ValueTupleArgsReceive(input);
        result.Item1.Should().Be(input.Item1 * 2);
        result.Item2.Should().Be(input.Item2.ToUpper());
        result.Item3.Sum().Should().Be(input.Item3.Sum() * 2);
        TestResultHelper.SetFinishTest();
    }
}