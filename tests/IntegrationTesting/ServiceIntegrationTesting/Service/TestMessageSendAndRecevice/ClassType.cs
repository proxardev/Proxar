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


using MessagePack;
using ServiceIntegrationTesting.Models;

namespace ServiceIntegrationTesting;

[MessagePackObject]
public class PlayerScore
{
    [Key(0)] public int PlayerId { get; set; }
    [Key(1)] public int Score { get; set; }

    public int TotalScore() => PlayerId * 1000 + Score;
}

[MessagePackObject]
public class PlayerInfo
{
    [Key(0)] public int Id { get; set; }
    [Key(1)] public string Name { get; set; } = string.Empty;
    [Key(2)] public int Level { get; set; }

    public int PowerLevel() => Id + Name.Length * 10 + Level * 100;
}

[MessagePackObject]
public class TeamStats
{
    [Key(0)] public string TeamName { get; set; } = string.Empty;
    [Key(1)] public List<int> Scores { get; set; } = new();

    public int TotalScore()
    {
        int sum = 0;
        foreach (var s in Scores) sum += s;
        return sum + TeamName.Length;
    }
}

[MessagePackObject]
public class PlayerWithPosition
{
    [Key(0)] public PlayerInfo Player { get; set; } = new();
    [Key(1)] public Point3D Position { get; set; }

    public int CombinedScore() => Player.PowerLevel() + Position.Sum();
}