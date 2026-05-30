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
    // ========== int? ==========
    [ServiceMethod(60)]
    [ServiceMethodExport]
    private async ZFTask NullableInt_SendReceiveSucc()
    {
        int? input = 42;
        var result = await this.GetTargetServiceProxy().NullableIntArgsReceive(input);
        result.Should().Be(input * 2);
        TestResultHelper.SetFinishTest();
    }

    [ServiceMethod(61)]
    [ServiceMethodExport]
    private async ZFTask NullableIntNull_SendReceiveSucc()
    {
        int? input = null;
        var result = await this.GetTargetServiceProxy().NullableIntNullArgsReceive(input);
        result.Should().Be(-1);
        TestResultHelper.SetFinishTest();
    }

    // ========== long? ==========
    [ServiceMethod(62)]
    [ServiceMethodExport]
    private async ZFTask NullableLong_SendReceiveSucc()
    {
        long? input = 100L;
        var result = await this.GetTargetServiceProxy().NullableLongArgsReceive(input);
        result.Should().Be(input + 50);
        TestResultHelper.SetFinishTest();
    }

    [ServiceMethod(63)]
    [ServiceMethodExport]
    private async ZFTask NullableLongNull_SendReceiveSucc()
    {
        long? input = null;
        var result = await this.GetTargetServiceProxy().NullableLongNullArgsReceive(input);
        result.Should().Be(0L);
        TestResultHelper.SetFinishTest();
    }

    // ========== float? ==========
    [ServiceMethod(64)]
    [ServiceMethodExport]
    private async ZFTask NullableFloat_SendReceiveSucc()
    {
        float? input = 3.5f;
        var result = await this.GetTargetServiceProxy().NullableFloatArgsReceive(input);
        result.Should().BeApproximately(input.Value * 2, 0.001f);
        TestResultHelper.SetFinishTest();
    }

    [ServiceMethod(65)]
    [ServiceMethodExport]
    private async ZFTask NullableFloatNull_SendReceiveSucc()
    {
        float? input = null;
        var result = await this.GetTargetServiceProxy().NullableFloatNullArgsReceive(input);
        result.Should().Be(-1.0f);
        TestResultHelper.SetFinishTest();
    }

    // ========== double? ==========
    [ServiceMethod(66)]
    [ServiceMethodExport]
    private async ZFTask NullableDouble_SendReceiveSucc()
    {
        double? input = 2.5;
        var result = await this.GetTargetServiceProxy().NullableDoubleArgsReceive(input);
        result.Should().BeApproximately(input.Value * 3, 0.001);
        TestResultHelper.SetFinishTest();
    }

    [ServiceMethod(67)]
    [ServiceMethodExport]
    private async ZFTask NullableDoubleNull_SendReceiveSucc()
    {
        double? input = null;
        var result = await this.GetTargetServiceProxy().NullableDoubleNullArgsReceive(input);
        result.Should().Be(-1.0);
        TestResultHelper.SetFinishTest();
    }

    // ========== decimal? ==========
    [ServiceMethod(68)]
    [ServiceMethodExport]
    private async ZFTask NullableDecimal_SendReceiveSucc()
    {
        decimal? input = 10.5m;
        var result = await this.GetTargetServiceProxy().NullableDecimalArgsReceive(input);
        result.Should().Be(input * 1.5m);
        TestResultHelper.SetFinishTest();
    }

    [ServiceMethod(69)]
    [ServiceMethodExport]
    private async ZFTask NullableDecimalNull_SendReceiveSucc()
    {
        decimal? input = null;
        var result = await this.GetTargetServiceProxy().NullableDecimalNullArgsReceive(input);
        result.Should().Be(-1m);
        TestResultHelper.SetFinishTest();
    }

    // ========== bool? ==========
    [ServiceMethod(70)]
    [ServiceMethodExport]
    private async ZFTask NullableBool_SendReceiveSucc()
    {
        bool? input = true;
        var result = await this.GetTargetServiceProxy().NullableBoolArgsReceive(input);
        result.Should().Be(!input);
        TestResultHelper.SetFinishTest();
    }

    [ServiceMethod(71)]
    [ServiceMethodExport]
    private async ZFTask NullableBoolNull_SendReceiveSucc()
    {
        bool? input = null;
        var result = await this.GetTargetServiceProxy().NullableBoolNullArgsReceive(input);
        result.Should().Be(1);
        TestResultHelper.SetFinishTest();
    }

    // ========== byte? ==========
    [ServiceMethod(72)]
    [ServiceMethodExport]
    private async ZFTask NullableByte_SendReceiveSucc()
    {
        byte? input = 200;
        var result = await this.GetTargetServiceProxy().NullableByteArgsReceive(input);
        result.Should().Be((byte?)(input + 10));
        TestResultHelper.SetFinishTest();
    }

    [ServiceMethod(73)]
    [ServiceMethodExport]
    private async ZFTask NullableByteNull_SendReceiveSucc()
    {
        byte? input = null;
        var result = await this.GetTargetServiceProxy().NullableByteNullArgsReceive(input);
        result.Should().Be(1);
        TestResultHelper.SetFinishTest();
    }

    // ========== sbyte? ==========
    [ServiceMethod(74)]
    [ServiceMethodExport]
    private async ZFTask NullableSbyte_SendReceiveSucc()
    {
        sbyte? input = -50;
        var result = await this.GetTargetServiceProxy().NullableSbyteArgsReceive(input);
        result.Should().Be((sbyte?)(-input));
        TestResultHelper.SetFinishTest();
    }

    [ServiceMethod(75)]
    [ServiceMethodExport]
    private async ZFTask NullableSbyteNull_SendReceiveSucc()
    {
        sbyte? input = null;
        var result = await this.GetTargetServiceProxy().NullableSbyteNullArgsReceive(input);
        result.Should().Be(1);
        TestResultHelper.SetFinishTest();
    }

    // ========== short? ==========
    [ServiceMethod(76)]
    [ServiceMethodExport]
    private async ZFTask NullableShort_SendReceiveSucc()
    {
        short? input = 1000;
        var result = await this.GetTargetServiceProxy().NullableShortArgsReceive(input);
        result.Should().Be((short?)(input + 100));
        TestResultHelper.SetFinishTest();
    }

    [ServiceMethod(77)]
    [ServiceMethodExport]
    private async ZFTask NullableShortNull_SendReceiveSucc()
    {
        short? input = null;
        var result = await this.GetTargetServiceProxy().NullableShortNullArgsReceive(input);
        result.Should().Be(1);
        TestResultHelper.SetFinishTest();
    }

    // ========== ushort? ==========
    [ServiceMethod(78)]
    [ServiceMethodExport]
    private async ZFTask NullableUshort_SendReceiveSucc()
    {
        ushort? input = 50000;
        var result = await this.GetTargetServiceProxy().NullableUshortArgsReceive(input);
        result.Should().Be((ushort?)(input * 2));
        TestResultHelper.SetFinishTest();
    }

    [ServiceMethod(79)]
    [ServiceMethodExport]
    private async ZFTask NullableUshortNull_SendReceiveSucc()
    {
        ushort? input = null;
        var result = await this.GetTargetServiceProxy().NullableUshortNullArgsReceive(input);
        result.Should().Be(1);
        TestResultHelper.SetFinishTest();
    }

    // ========== uint? ==========
    [ServiceMethod(80)]
    [ServiceMethodExport]
    private async ZFTask NullableUint_SendReceiveSucc()
    {
        uint? input = 1000000U;
        var result = await this.GetTargetServiceProxy().NullableUintArgsReceive(input);
        result.Should().Be(input * 3);
        TestResultHelper.SetFinishTest();
    }

    [ServiceMethod(81)]
    [ServiceMethodExport]
    private async ZFTask NullableUintNull_SendReceiveSucc()
    {
        uint? input = null;
        var result = await this.GetTargetServiceProxy().NullableUintNullArgsReceive(input);
        result.Should().Be(1);
        TestResultHelper.SetFinishTest();
    }

    // ========== ulong? ==========
    [ServiceMethod(82)]
    [ServiceMethodExport]
    private async ZFTask NullableUlong_SendReceiveSucc()
    {
        ulong? input = 999999UL;
        var result = await this.GetTargetServiceProxy().NullableUlongArgsReceive(input);
        result.Should().Be(input + 1000);
        TestResultHelper.SetFinishTest();
    }

    [ServiceMethod(83)]
    [ServiceMethodExport]
    private async ZFTask NullableUlongNull_SendReceiveSucc()
    {
        ulong? input = null;
        var result = await this.GetTargetServiceProxy().NullableUlongNullArgsReceive(input);
        result.Should().Be(1);
        TestResultHelper.SetFinishTest();
    }

    // ========== char? ==========
    [ServiceMethod(84)]
    [ServiceMethodExport]
    private async ZFTask NullableChar_SendReceiveSucc()
    {
        char? input = 'a';
        var result = await this.GetTargetServiceProxy().NullableCharArgsReceive(input);
        result.Should().Be(char.ToUpper(input.Value));
        TestResultHelper.SetFinishTest();
    }

    [ServiceMethod(85)]
    [ServiceMethodExport]
    private async ZFTask NullableCharNull_SendReceiveSucc()
    {
        char? input = null;
        var result = await this.GetTargetServiceProxy().NullableCharNullArgsReceive(input);
        result.Should().Be('?');
        TestResultHelper.SetFinishTest();
    }
}