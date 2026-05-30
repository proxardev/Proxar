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
using System.Collections.Concurrent;

namespace ServiceIntegrationTesting;

public partial class TestService_Message
{
    [ServiceMethod(50)]
    [ServiceMethodExport]
    private async ZFTask ListInt_SendReceiveSucc()
    {
        var input = new List<int> { 1, 2, 3, 4, 5 };
        var result = await this.GetTargetServiceProxy().ListIntArgsReceive(input);
        result.Should().Be(input.Sum());
        TestResultHelper.SetFinishTest();
    }

    [ServiceMethod(51)]
    [ServiceMethodExport]
    private async ZFTask ListString_SendReceiveSucc()
    {
        var input = new List<string> { "a", "bb", "ccc" };
        var result = await this.GetTargetServiceProxy().ListStringArgsReceive(input);
        result.Should().Be(input.Sum(s => s.Length));
        TestResultHelper.SetFinishTest();
    }

    [ServiceMethod(52)]
    [ServiceMethodExport]
    private async ZFTask ListPoint3D_SendReceiveSucc()
    {
        var input = new List<Point3D>
        {
            new Point3D(1, 2, 3),
            new Point3D(4, 5, 6)
        };
        var result = await this.GetTargetServiceProxy().ListPoint3DArgsReceive(input);
        result.Should().Be(input.Sum(p => p.Sum()));
        TestResultHelper.SetFinishTest();
    }

    [ServiceMethod(53)]
    [ServiceMethodExport]
    private async ZFTask ListPlayerScore_SendReceiveSucc()
    {
        var input = new List<PlayerScore>
        {
            new PlayerScore { PlayerId = 1, Score = 100 },
            new PlayerScore { PlayerId = 2, Score = 200 }
        };
        var result = await this.GetTargetServiceProxy().ListPlayerScoreArgsReceive(input);
        result.Should().Be(input.Sum(p => p.TotalScore()));
        TestResultHelper.SetFinishTest();
    }

    [ServiceMethod(54)]
    [ServiceMethodExport]
    private async ZFTask IntArray_SendReceiveSucc()
    {
        var input = new int[] { 10, 20, 30 };
        var result = await this.GetTargetServiceProxy().IntArrayArgsReceive(input);
        result.Should().Be(input.Sum() * 2);
        TestResultHelper.SetFinishTest();
    }

    [ServiceMethod(55)]
    [ServiceMethodExport]
    private async ZFTask Dictionary_SendReceiveSucc()
    {
        var input = new Dictionary<string, int> { { "a", 1 }, { "b", 2 }, { "c", 3 } };
        var result = await this.GetTargetServiceProxy().DictionaryArgsReceive(input);
        result.Should().Be(input.Values.Sum());
        TestResultHelper.SetFinishTest();
    }

    [ServiceMethod(56)]
    [ServiceMethodExport]
    private async ZFTask HashSet_SendReceiveSucc()
    {
        var input = new HashSet<int> { 1, 2, 3, 4, 5 };
        var result = await this.GetTargetServiceProxy().HashSetArgsReceive(input);
        result.Should().Be(input.Sum());
        TestResultHelper.SetFinishTest();
    }

    [ServiceMethod(57)]
    [ServiceMethodExport]
    private async ZFTask ConcurrentDict_SendReceiveSucc()
    {
        var input = new ConcurrentDictionary<string, int>();
        input["x"] = 10;
        input["y"] = 20;
        input["z"] = 30;
        var result = await this.GetTargetServiceProxy().ConcurrentDictArgsReceive(input);
        result.Should().Be(input.Values.Sum());
        TestResultHelper.SetFinishTest();
    }
}