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
    [ServiceMethod(110)]
    [ServiceMethodExport]
    private int IntStringArgsReceive(int args1, string args2) =>
        args1 + args2.Length;

    [ServiceMethod(111)]
    [ServiceMethodExport]
    private int IntPoint3DArgsReceive(int args1, Point3D args2) =>
        args1 + args2.Sum() + 10;

    [ServiceMethod(112)]
    [ServiceMethodExport]
    private int IntPlayerScoreArgsReceive(int args1, PlayerScore args2) =>
        args1 + args2.TotalScore();

    [ServiceMethod(113)]
    [ServiceMethodExport]
    private long LongStringArgsReceive(long args1, string args2) =>
        args1 + args2.Length;

    [ServiceMethod(114)]
    [ServiceMethodExport]
    private int StringListIntArgsReceive(string args1, List<int> args2) =>
        args1.Length + args2.Sum();

    [ServiceMethod(115)]
    [ServiceMethodExport]
    private int StringIntArrayArgsReceive(string args1, int[] args2) =>
        args1.Length + args2.Sum() * 2;

    [ServiceMethod(116)]
    [ServiceMethodExport]
    private int EnumIntArgsReceive(StatusCode args1, int args2) =>
        (int)args1 * 10 + args2;

    [ServiceMethod(117)]
    [ServiceMethodExport]
    private int EnumStringArgsReceive(Priority args1, string args2) =>
        (int)args1 + args2.Length;

    [ServiceMethod(118)]
    [ServiceMethodExport]
    private int Point2DPoint3DArgsReceive(Point2D args1, Point3D args2) =>
        args1.Sum() + args2.Sum() + 5;

    [ServiceMethod(119)]
    [ServiceMethodExport]
    private int Point3DPlayerScoreArgsReceive(Point3D args1, PlayerScore args2) =>
        args1.Sum() + args2.TotalScore() + 1;

    [ServiceMethod(120)]
    [ServiceMethodExport]
    private int IntEnumArgsReceive(int args1, StatusCode args2) =>
        args1 + (int)args2 + 100;

    [ServiceMethod(121)]
    [ServiceMethodExport]
    private int StringEnumArgsReceive(string args1, Priority args2) =>
        args1.Length + (int)args2 + 10;

    [ServiceMethod(122)]
    [ServiceMethodExport]
    private int ListIntListStringArgsReceive(List<int> args1, List<string> args2) =>
        args1.Sum() + args2.Sum(s => s.Length);

    [ServiceMethod(123)]
    [ServiceMethodExport]
    private int IntArrayStringArrayArgsReceive(int[] args1, string[] args2) =>
        args1.Sum() * 2 + args2.Sum(s => s.Length);

    [ServiceMethod(124)]
    [ServiceMethodExport]
    private int IntDateTimeArgsReceive(int args1, DateTime args2) =>
        args1 + args2.Day;

    [ServiceMethod(125)]
    [ServiceMethodExport]
    private int StringGuidArgsReceive(string args1, Guid args2) =>
        args1.Length + 16;

    [ServiceMethod(126)]
    [ServiceMethodExport]
    private int Point3DListIntArgsReceive(Point3D args1, List<int> args2) =>
        args1.Sum() + args2.Sum() * 2;

    [ServiceMethod(127)]
    [ServiceMethodExport]
    private int PlayerScoreStringArgsReceive(PlayerScore args1, string args2) =>
        args1.TotalScore() + args2.Length;

    [ServiceMethod(128)]
    [ServiceMethodExport]
    private decimal IntDecimalArgsReceive(int args1, decimal args2) =>
        args1 + args2;

    [ServiceMethod(129)]
    [ServiceMethodExport]
    private long LongPoint3DArgsReceive(long args1, Point3D args2) =>
        args1 + args2.Sum();

    [ServiceMethod(130)]
    [ServiceMethodExport]
    private long StringTimeSpanArgsReceive(string args1, TimeSpan args2) =>
        args1.Length + (long)args2.TotalHours;
}