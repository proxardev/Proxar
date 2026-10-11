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
    [ServiceMethod]
    [ServiceMethodExport]
    private int ListIntArgsReceive(List<int> args1) =>
        args1.Sum();

    [ServiceMethod]
    [ServiceMethodExport]
    private int ListStringArgsReceive(List<string> args1) =>
        args1.Sum(s => s.Length);

    [ServiceMethod]
    [ServiceMethodExport]
    private int ListPoint3DArgsReceive(List<Point3D> args1) =>
        args1.Sum(p => p.Sum());

    [ServiceMethod]
    [ServiceMethodExport]
    private int ListPlayerScoreArgsReceive(List<PlayerScore> args1) =>
        args1.Sum(p => p.TotalScore());

    [ServiceMethod]
    [ServiceMethodExport]
    private int IntArrayArgsReceive(int[] args1) =>
        args1.Sum() * 2;

    [ServiceMethod]
    [ServiceMethodExport]
    private int DictionaryArgsReceive(Dictionary<string, int> args1) =>
        args1.Values.Sum();

    [ServiceMethod]
    [ServiceMethodExport]
    private int HashSetArgsReceive(HashSet<int> args1) =>
        args1.Sum();

    [ServiceMethod]
    [ServiceMethodExport]
    private int ConcurrentDictArgsReceive(ConcurrentDictionary<string, int> args1) =>
        args1.Values.Sum();
}