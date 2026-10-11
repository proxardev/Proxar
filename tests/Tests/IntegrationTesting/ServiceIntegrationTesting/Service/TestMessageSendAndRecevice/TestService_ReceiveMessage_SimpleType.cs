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
    [ServiceMethod]
    [ServiceMethodExport]
    private int IntArgsReceive(int args1) => SimpleTypeOps.Transform(args1);
    [ServiceMethod]
    [ServiceMethodExport]
    private long LongArgsReceive(long args1) => SimpleTypeOps.Transform(args1);
    [ServiceMethod]
    [ServiceMethodExport]
    private short ShortArgsReceive(short args1) => SimpleTypeOps.Transform(args1);
    [ServiceMethod]
    [ServiceMethodExport]
    private byte ByteArgsReceive(byte args1) => SimpleTypeOps.Transform(args1);
    [ServiceMethod]
    [ServiceMethodExport]
    private sbyte SbyteArgsReceive(sbyte args1) => SimpleTypeOps.Transform(args1);
    [ServiceMethod]
    [ServiceMethodExport]
    private float FloatArgsReceive(float args1) => SimpleTypeOps.Transform(args1);
    [ServiceMethod]
    [ServiceMethodExport]
    private double DoubleArgsReceive(double args1) => SimpleTypeOps.Transform(args1);
    [ServiceMethod]
    [ServiceMethodExport]
    private decimal DecimalArgsReceive(decimal args1) => SimpleTypeOps.Transform(args1);
    [ServiceMethod]
    [ServiceMethodExport]
    private bool BoolArgsReceive(bool args1) => SimpleTypeOps.Transform(args1);

    [ServiceMethod]
    [ServiceMethodExport]
    private char CharArgsReceive(char args1) => SimpleTypeOps.Transform(args1);

    [ServiceMethod]
    [ServiceMethodExport]
    private uint UintArgsReceive(uint args1) => SimpleTypeOps.Transform(args1);
    [ServiceMethod]
    [ServiceMethodExport]
    private ulong UlongArgsReceive(ulong args1) => SimpleTypeOps.Transform(args1);
    [ServiceMethod]
    [ServiceMethodExport]
    private ushort UshortArgsReceive(ushort args1) => SimpleTypeOps.Transform(args1);

    // 416-419: 预留
}