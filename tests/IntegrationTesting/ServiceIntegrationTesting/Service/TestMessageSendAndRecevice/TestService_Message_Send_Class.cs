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
    [ServiceMethod(40)]
    [ServiceMethodExport]
    private async ZFTask PlayerScore_SendReceiveSucc()
    {
        var input = new PlayerScore { PlayerId = 42, Score = 100 };
        var result = await this.GetTargetServiceProxy().PlayerScoreArgsReceive(input);
        result.Should().Be(input.TotalScore());
        TestResultHelper.SetFinishTest();
    }

    [ServiceMethod(41)]
    [ServiceMethodExport]
    private async ZFTask PlayerInfo_SendReceiveSucc()
    {
        var input = new PlayerInfo { Id = 1, Name = "Alice", Level = 5 };
        var input2 = new PlayerInfo { Id = 1, Name = "Alice", Level = 5 };
        var powerLevel = input.PowerLevel();
        var result = await this.GetTargetServiceProxy().PlayerInfoArgsReceive_41(input);
        input.Level++;
        result.Should().Be(powerLevel);
        TestResultHelper.SetFinishTest();
    }

    [ServiceMethod(42)]
    [ServiceMethodExport]
    private async ZFTask TeamStats_SendReceiveSucc()
    {
        var input = new TeamStats
        {
            TeamName = "RedTeam",
            Scores = new List<int> { 10, 20, 30 }
        };
        var result = await this.GetTargetServiceProxy().TeamStatsArgsReceive(input);
        result.Should().Be(input.TotalScore());
        TestResultHelper.SetFinishTest();
    }

    [ServiceMethod(43)]
    [ServiceMethodExport]
    private async ZFTask PlayerWithPosition_SendReceiveSucc()
    {
        var input = new PlayerWithPosition
        {
            Player = new PlayerInfo { Id = 2, Name = "Bob", Level = 3 },
            Position = new Point3D(1, 2, 3)
        };
        var result = await this.GetTargetServiceProxy().PlayerWithPositionArgsReceive(input);
        result.Should().Be(input.CombinedScore());
        TestResultHelper.SetFinishTest();
    }

    [ServiceMethod(44)]
    [ServiceMethodExport]
    private async ZFTask SendPlayerInfoAndChannge_SendReceiveRightSucc()
    {
        var input = new PlayerInfo { Id = 1, Name = "Alice", Level = 5 };
        var input2 = new PlayerInfo { Id = 1, Name = "Alice", Level = 5 };
        var powerLevel = input.PowerLevel();
        var resultTask = GetTargetServiceProxy().PlayerInfoArgsReceive(input);
        input.Level++;
        var result = await resultTask;
        result.Should().Be(powerLevel);
        TestResultHelper.SetFinishTest();
    }
}