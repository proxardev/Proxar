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
using System.Collections.Concurrent;

namespace ServiceIntegrationTesting;

public partial class TestService_ReceiveMessage
{
    [ServiceMethod(50)]
    [ServiceMethodExport]
    private int ListIntArgsReceive(List<int> args1) =>
        args1.Sum();

    [ServiceMethod(51)]
    [ServiceMethodExport]
    private int ListStringArgsReceive(List<string> args1) =>
        args1.Sum(s => s.Length);

    [ServiceMethod(52)]
    [ServiceMethodExport]
    private int ListPoint3DArgsReceive(List<Point3D> args1) =>
        args1.Sum(p => p.Sum());

    [ServiceMethod(53)]
    [ServiceMethodExport]
    private int ListPlayerScoreArgsReceive(List<PlayerScore> args1) =>
        args1.Sum(p => p.TotalScore());

    [ServiceMethod(54)]
    [ServiceMethodExport]
    private int IntArrayArgsReceive(int[] args1) =>
        args1.Sum() * 2;

    [ServiceMethod(55)]
    [ServiceMethodExport]
    private int DictionaryArgsReceive(Dictionary<string, int> args1) =>
        args1.Values.Sum();

    [ServiceMethod(56)]
    [ServiceMethodExport]
    private int HashSetArgsReceive(HashSet<int> args1) =>
        args1.Sum();

    [ServiceMethod(57)]
    [ServiceMethodExport]
    private int ConcurrentDictArgsReceive(ConcurrentDictionary<string, int> args1) =>
        args1.Values.Sum();
}