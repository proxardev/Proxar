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
    [ServiceMethod(90)]
    [ServiceMethodExport]
    private DateTime DateTimeArgsReceive(DateTime args1) =>
        args1.AddDays(1);

    [ServiceMethod(91)]
    [ServiceMethodExport]
    private Guid GuidArgsReceive(Guid args1) =>
        args1;

    [ServiceMethod(92)]
    [ServiceMethodExport]
    private (int, string, Point3D) ValueTupleArgsReceive((int, string, Point3D) args1) =>
        (args1.Item1 * 2, args1.Item2.ToUpper(), new Point3D(
            args1.Item3.X * 2,
            args1.Item3.Y * 2,
            args1.Item3.Z * 2));
}