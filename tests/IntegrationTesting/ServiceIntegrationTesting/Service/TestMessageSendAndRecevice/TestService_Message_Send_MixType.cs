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
    [ServiceMethod(110)]
    [ServiceMethodExport]
    private async ZFTask IntString_SendReceiveSucc()
    {
        var result = await this.GetTargetServiceProxy().IntStringArgsReceive(10, "hello");
        result.Should().Be(10 + 5);
        TestResultHelper.SetFinishTest();
    }

    [ServiceMethod(111)]
    [ServiceMethodExport]
    private async ZFTask IntPoint3D_SendReceiveSucc()
    {
        var result = await this.GetTargetServiceProxy().IntPoint3DArgsReceive(5, new Point3D(1, 2, 3));
        result.Should().Be(5 + 6 + 10);
        TestResultHelper.SetFinishTest();
    }

    [ServiceMethod(112)]
    [ServiceMethodExport]
    private async ZFTask IntPlayerScore_SendReceiveSucc()
    {
        var result = await this.GetTargetServiceProxy().IntPlayerScoreArgsReceive(7, new PlayerScore { PlayerId = 2, Score = 50 });
        result.Should().Be(7 + 2050);
        TestResultHelper.SetFinishTest();
    }

    [ServiceMethod(113)]
    [ServiceMethodExport]
    private async ZFTask LongString_SendReceiveSucc()
    {
        var result = await this.GetTargetServiceProxy().LongStringArgsReceive(100L, "world");
        result.Should().Be(105L);
        TestResultHelper.SetFinishTest();
    }

    [ServiceMethod(114)]
    [ServiceMethodExport]
    private async ZFTask StringListInt_SendReceiveSucc()
    {
        var result = await this.GetTargetServiceProxy().StringListIntArgsReceive("test", new List<int> { 1, 2, 3 });
        result.Should().Be(4 + 6);
        TestResultHelper.SetFinishTest();
    }

    [ServiceMethod(115)]
    [ServiceMethodExport]
    private async ZFTask StringIntArray_SendReceiveSucc()
    {
        var result = await this.GetTargetServiceProxy().StringIntArrayArgsReceive("abc", new int[] { 10, 20 });
        result.Should().Be(3 + 60);
        TestResultHelper.SetFinishTest();
    }

    [ServiceMethod(116)]
    [ServiceMethodExport]
    private async ZFTask EnumInt_SendReceiveSucc()
    {
        var result = await this.GetTargetServiceProxy().EnumIntArgsReceive(StatusCode.Pending, 5);
        result.Should().Be(2 * 10 + 5);
        TestResultHelper.SetFinishTest();
    }

    [ServiceMethod(117)]
    [ServiceMethodExport]
    private async ZFTask EnumString_SendReceiveSucc()
    {
        var result = await this.GetTargetServiceProxy().EnumStringArgsReceive(Priority.High, "msg");
        result.Should().Be(10 + 3);
        TestResultHelper.SetFinishTest();
    }

    [ServiceMethod(118)]
    [ServiceMethodExport]
    private async ZFTask Point2DPoint3D_SendReceiveSucc()
    {
        var result = await this.GetTargetServiceProxy().Point2DPoint3DArgsReceive(new Point2D(3, 4), new Point3D(1, 2, 3));
        result.Should().Be(7 + 6 + 5);
        TestResultHelper.SetFinishTest();
    }

    [ServiceMethod(119)]
    [ServiceMethodExport]
    private async ZFTask Point3DPlayerScore_SendReceiveSucc()
    {
        var result = await this.GetTargetServiceProxy().Point3DPlayerScoreArgsReceive(new Point3D(2, 3, 4), new PlayerScore { PlayerId = 1, Score = 100 });
        result.Should().Be(9 + 1100 + 1);
        TestResultHelper.SetFinishTest();
    }

    [ServiceMethod(120)]
    [ServiceMethodExport]
    private async ZFTask IntEnum_SendReceiveSucc()
    {
        var result = await this.GetTargetServiceProxy().IntEnumArgsReceive(8, StatusCode.Failed);
        result.Should().Be(8 + 1 + 100);
        TestResultHelper.SetFinishTest();
    }

    [ServiceMethod(121)]
    [ServiceMethodExport]
    private async ZFTask StringEnum_SendReceiveSucc()
    {
        var result = await this.GetTargetServiceProxy().StringEnumArgsReceive("test", Priority.Medium);
        result.Should().Be(4 + 5 + 10);
        TestResultHelper.SetFinishTest();
    }

    [ServiceMethod(122)]
    [ServiceMethodExport]
    private async ZFTask ListIntListString_SendReceiveSucc()
    {
        var result = await this.GetTargetServiceProxy().ListIntListStringArgsReceive(new List<int> { 1, 2 }, new List<string> { "a", "bb" });
        result.Should().Be(3 + 3);
        TestResultHelper.SetFinishTest();
    }

    [ServiceMethod(123)]
    [ServiceMethodExport]
    private async ZFTask IntArrayStringArray_SendReceiveSucc()
    {
        var result = await this.GetTargetServiceProxy().IntArrayStringArrayArgsReceive(new int[] { 5, 10 }, new string[] { "x", "yy", "zzz" });
        result.Should().Be(30 + 6);
        TestResultHelper.SetFinishTest();
    }

    [ServiceMethod(124)]
    [ServiceMethodExport]
    private async ZFTask IntDateTime_SendReceiveSucc()
    {
        var result = await this.GetTargetServiceProxy().IntDateTimeArgsReceive(100, new DateTime(2024, 1, 15));
        result.Should().Be(100 + 15);
        TestResultHelper.SetFinishTest();
    }

    [ServiceMethod(125)]
    [ServiceMethodExport]
    private async ZFTask StringGuid_SendReceiveSucc()
    {
        var guid = Guid.Parse("11111111-1111-1111-1111-111111111111");
        var result = await this.GetTargetServiceProxy().StringGuidArgsReceive("guid", guid);
        result.Should().Be(4 + 16);
        TestResultHelper.SetFinishTest();
    }

    [ServiceMethod(126)]
    [ServiceMethodExport]
    private async ZFTask Point3DListInt_SendReceiveSucc()
    {
        var result = await this.GetTargetServiceProxy().Point3DListIntArgsReceive(new Point3D(1, 2, 3), new List<int> { 5, 10 });
        result.Should().Be(6 + 30);
        TestResultHelper.SetFinishTest();
    }

    [ServiceMethod(127)]
    [ServiceMethodExport]
    private async ZFTask PlayerScoreString_SendReceiveSucc()
    {
        var result = await this.GetTargetServiceProxy().PlayerScoreStringArgsReceive(new PlayerScore { PlayerId = 3, Score = 200 }, "player");
        result.Should().Be(3200 + 6);
        TestResultHelper.SetFinishTest();
    }

    [ServiceMethod(128)]
    [ServiceMethodExport]
    private async ZFTask IntDecimal_SendReceiveSucc()
    {
        var result = await this.GetTargetServiceProxy().IntDecimalArgsReceive(50, 10.5m);
        result.Should().Be(60.5m);
        TestResultHelper.SetFinishTest();
    }

    [ServiceMethod(129)]
    [ServiceMethodExport]
    private async ZFTask LongPoint3D_SendReceiveSucc()
    {
        var result = await this.GetTargetServiceProxy().LongPoint3DArgsReceive(1000L, new Point3D(2, 4, 6));
        result.Should().Be(1000L + 12);
        TestResultHelper.SetFinishTest();
    }

    [ServiceMethod(130)]
    [ServiceMethodExport]
    private async ZFTask StringTimeSpan_SendReceiveSucc()
    {
        var result = await this.GetTargetServiceProxy().StringTimeSpanArgsReceive("time", TimeSpan.FromHours(5));
        result.Should().Be(4 + 5);
        TestResultHelper.SetFinishTest();
    }
}