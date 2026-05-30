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
    [ServiceMethod(100)]
    [ServiceMethodExport]
    private int GenericStructIntArgsReceive(GenericStruct<int> args1) =>
        args1.Value * args1.Count;

    [ServiceMethod(101)]
    [ServiceMethodExport]
    private int GenericStructStringArgsReceive(GenericStruct<string> args1) =>
        args1.Value.Length * args1.Count;

    [ServiceMethod(102)]
    [ServiceMethodExport]
    private int GenericStructPlayerScoreArgsReceive(GenericStruct<PlayerScore> args1) =>
        args1.Value.TotalScore() * args1.Count;

    [ServiceMethod(103)]
    [ServiceMethodExport]
    private int GenericStructNestedGenericStructArgsReceive(GenericStruct<GenericStruct<int>> args1) =>
        (args1.Value.Value * args1.Value.Count) * args1.Count;

    [ServiceMethod(104)]
    [ServiceMethodExport]
    private int GenericStructStructWithClassArgsReceive(GenericStruct<StructWithClass> args1) =>
        (args1.Value.Value + (args1.Value.Ref?.Id ?? 0)) * args1.Count;

    [ServiceMethod(105)]
    [ServiceMethodExport]
    private int GenericClassIntArgsReceive(GenericClass<int> args1) =>
        args1.Compute() + args1.Data;

    [ServiceMethod(106)]
    [ServiceMethodExport]
    private int GenericClassListIntArgsReceive(GenericClass<List<int>> args1) =>
        args1.Compute() + args1.Data.Sum();

    [ServiceMethod(107)]
    [ServiceMethodExport]
    private int GenericClassPoint3DArgsReceive(GenericClass<Point3D> args1) =>
        args1.Compute() + args1.Data.Sum();

    [ServiceMethod(108)]
    [ServiceMethodExport]
    private int GenericClassStructWithClassArgsReceive(GenericClass<StructWithClass> args1) =>
        args1.Compute() + args1.Data.Value + (args1.Data.Ref?.Id ?? 0);

    [ServiceMethod(109)]
    [ServiceMethodExport]
    private int GenericClassGenericClassIntArgsReceive(GenericClass<GenericClass<int>> args1) =>
        args1.Compute() + args1.Data.Compute() + args1.Data.Data;
}