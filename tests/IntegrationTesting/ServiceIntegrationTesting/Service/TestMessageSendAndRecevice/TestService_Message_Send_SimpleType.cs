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
    [ServiceMethod(3)]
    [ServiceMethodExport]
    private async ZFTask Int_SendReceiveSucc()
    {
        const int input = 42;
        var result = await this.GetTargetServiceProxy().IntArgsReceive(input);
        result.Should().Be(SimpleTypeOps.Transform(input));
        TestResultHelper.SetFinishTest();
    }

    [ServiceMethod(4)]
    [ServiceMethodExport]
    private async ZFTask Long_SendReceiveSucc()
    {
        const long input = 100L;
        var result = await this.GetTargetServiceProxy().LongArgsReceive(input);
        result.Should().Be(SimpleTypeOps.Transform(input));
        TestResultHelper.SetFinishTest();
    }

    [ServiceMethod(5)]
    [ServiceMethodExport]
    private async ZFTask Short_SendReceiveSucc()
    {
        const short input = 50;
        var result = await this.GetTargetServiceProxy().ShortArgsReceive(input);
        result.Should().Be(SimpleTypeOps.Transform(input));
        TestResultHelper.SetFinishTest();
    }

    [ServiceMethod(6)]
    [ServiceMethodExport]
    private async ZFTask Byte_SendReceiveSucc()
    {
        const byte input = 200;
        var result = await this.GetTargetServiceProxy().ByteArgsReceive(input);
        result.Should().Be(SimpleTypeOps.Transform(input));
        TestResultHelper.SetFinishTest();
    }

    [ServiceMethod(7)]
    [ServiceMethodExport]
    private async ZFTask Sbyte_SendReceiveSucc()
    {
        const sbyte input = -50;
        var result = await this.GetTargetServiceProxy().SbyteArgsReceive(input);
        result.Should().Be(SimpleTypeOps.Transform(input));
        TestResultHelper.SetFinishTest();
    }

    [ServiceMethod(8)]
    [ServiceMethodExport]
    private async ZFTask Float_SendReceiveSucc()
    {
        const float input = 3.5f;
        var result = await this.GetTargetServiceProxy().FloatArgsReceive(input);
        result.Should().BeApproximately(SimpleTypeOps.Transform(input), 0.001f);
        TestResultHelper.SetFinishTest();
    }

    [ServiceMethod(9)]
    [ServiceMethodExport]
    private async ZFTask Double_SendReceiveSucc()
    {
        const double input = 2.5;
        var result = await this.GetTargetServiceProxy().DoubleArgsReceive(input);
        result.Should().BeApproximately(SimpleTypeOps.Transform(input), 0.001);
        TestResultHelper.SetFinishTest();
    }

    [ServiceMethod(10)]
    [ServiceMethodExport]
    private async ZFTask Decimal_SendReceiveSucc()
    {
        const decimal input = 10.5m;
        var result = await this.GetTargetServiceProxy().DecimalArgsReceive(input);
        result.Should().Be(SimpleTypeOps.Transform(input));
        TestResultHelper.SetFinishTest();
    }

    [ServiceMethod(11)]
    [ServiceMethodExport]
    private async ZFTask Bool_SendReceiveSucc()
    {
        const bool input = true;
        var result = await this.GetTargetServiceProxy().BoolArgsReceive(input);
        result.Should().Be(SimpleTypeOps.Transform(input));
        TestResultHelper.SetFinishTest();
    }

    [ServiceMethod(12)]
    [ServiceMethodExport]
    private async ZFTask Char_SendReceiveSucc()
    {
        const char input = 'a';
        var result = await this.GetTargetServiceProxy().CharArgsReceive(input);
        result.Should().Be(SimpleTypeOps.Transform(input));
        TestResultHelper.SetFinishTest();
    }

    [ServiceMethod(13)]
    [ServiceMethodExport]
    private async ZFTask Uint_SendReceiveSucc()
    {
        const uint input = 1000U;
        var result = await this.GetTargetServiceProxy().UintArgsReceive(input);
        result.Should().Be(SimpleTypeOps.Transform(input));
        TestResultHelper.SetFinishTest();
    }

    [ServiceMethod(14)]
    [ServiceMethodExport]
    private async ZFTask Ulong_SendReceiveSucc()
    {
        const ulong input = 999999UL;
        var result = await this.GetTargetServiceProxy().UlongArgsReceive(input);
        result.Should().Be(SimpleTypeOps.Transform(input));
        TestResultHelper.SetFinishTest();
    }

    [ServiceMethod(15)]
    [ServiceMethodExport]
    private async ZFTask Ushort_SendReceiveSucc()
    {
        const ushort input = 60000;
        var result = await this.GetTargetServiceProxy().UshortArgsReceive(input);
        result.Should().Be(SimpleTypeOps.Transform(input));
        TestResultHelper.SetFinishTest();
    }

    //    // 16-19: 预留


}