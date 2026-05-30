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
    [ServiceMethod(30)]
    [ServiceMethodExport]
    private int Point2DArgsReceive(Point2D args1) => args1.Sum() * 2;

    [ServiceMethod(31)]
    [ServiceMethodExport]
    private (int, Point3D) Point3DArgsReceive(Point3D args1) => (args1.Sum() + 10, args1.Double());

    [ServiceMethod(32)]
    [ServiceMethodExport]
    private string LabeledValueArgsReceive(LabeledValue args1) => $"{args1.Label}:{args1.Value + 50}";

    [ServiceMethod(33)]
    [ServiceMethodExport]
    private int StructWithClassArgsReceive(StructWithClass args1) => args1.Value + (args1.Ref?.Compute() ?? 0);
}