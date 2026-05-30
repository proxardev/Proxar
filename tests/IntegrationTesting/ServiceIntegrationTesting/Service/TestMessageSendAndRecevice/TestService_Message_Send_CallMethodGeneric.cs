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

namespace ServiceIntegrationTesting;

public partial class TestService_Message
{
    [ServiceMethod(150)]
    [ServiceMethodExport]
    private async ZFTask Call1Arg_SendReceiveSucc()
    {
        var result = await this.GetTargetServiceProxy().Call1Arg(21);
        result.Should().Be(42);
        TestResultHelper.SetFinishTest();
    }

    [ServiceMethod(151)]
    [ServiceMethodExport]
    private async ZFTask Call2Args_SendReceiveSucc()
    {
        var result = await this.GetTargetServiceProxy().Call2Args(10, 20);
        result.Should().Be(60);
        TestResultHelper.SetFinishTest();
    }

    [ServiceMethod(152)]
    [ServiceMethodExport]
    private async ZFTask Call3Args_SendReceiveSucc()
    {
        var result = await this.GetTargetServiceProxy().Call3Args(5, 10, 15);
        result.Should().Be(40);
        TestResultHelper.SetFinishTest();
    }

    [ServiceMethod(153)]
    [ServiceMethodExport]
    private async ZFTask Call4Args_SendReceiveSucc()
    {
        var result = await this.GetTargetServiceProxy().Call4Args(2, 3, 4, 5);
        result.Should().Be(26);
        TestResultHelper.SetFinishTest();
    }

    [ServiceMethod(154)]
    [ServiceMethodExport]
    private async ZFTask Call5Args_SendReceiveSucc()
    {
        var result = await this.GetTargetServiceProxy().Call5Args(1, 2, 3, 4, 5);
        result.Should().Be(30);
        TestResultHelper.SetFinishTest();
    }

    [ServiceMethod(155)]
    [ServiceMethodExport]
    private async ZFTask Call6Args_SendReceiveSucc()
    {
        var result = await this.GetTargetServiceProxy().Call6Args(1, 2, 3, 4, 5, 6);
        result.Should().Be(121);
        TestResultHelper.SetFinishTest();
    }

    [ServiceMethod(156)]
    [ServiceMethodExport]
    private async ZFTask Call7Args_SendReceiveSucc()
    {
        var result = await this.GetTargetServiceProxy().Call7Args(1, 2, 3, 10, 20, 30, 40);
        result.Should().Be(112);
        TestResultHelper.SetFinishTest();
    }

    [ServiceMethod(157)]
    [ServiceMethodExport]
    private async ZFTask Call8Args_SendReceiveSucc()
    {
        var result = await this.GetTargetServiceProxy().Call8Args(10, 10, 10, 10, 10, 10, 10, 10);
        result.Should().Be(40);
        TestResultHelper.SetFinishTest();
    }

    [ServiceMethod(158)]
    [ServiceMethodExport]
    private async ZFTask Call9Args_SendReceiveSucc()
    {
        var result = await this.GetTargetServiceProxy().Call9Args(1, 2, 3, 4, 5, 6, 7, 8, 9);
        result.Should().Be(90);
        TestResultHelper.SetFinishTest();
    }
}