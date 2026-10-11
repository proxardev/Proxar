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

namespace ServiceIntegrationTesting;

public partial class TestService_ReceiveMessage
{
    [ServiceMethod]
    [ServiceMethodExport]
    private int? NullableIntArgsReceive(int? args1) => args1 * 2;

    [ServiceMethod]
    [ServiceMethodExport]
    private int? NullableIntNullArgsReceive(int? args1) => args1 ?? -1;

    [ServiceMethod]
    [ServiceMethodExport]
    private long? NullableLongArgsReceive(long? args1) => args1 + 50;

    [ServiceMethod]
    [ServiceMethodExport]
    private long? NullableLongNullArgsReceive(long? args1) => args1 ?? 0L;

    [ServiceMethod]
    [ServiceMethodExport]
    private float? NullableFloatArgsReceive(float? args1) => args1 * 2;

    [ServiceMethod]
    [ServiceMethodExport]
    private float? NullableFloatNullArgsReceive(float? args1) => args1 ?? -1.0f;

    [ServiceMethod]
    [ServiceMethodExport]
    private double? NullableDoubleArgsReceive(double? args1) => args1 * 3;

    [ServiceMethod]
    [ServiceMethodExport]
    private double? NullableDoubleNullArgsReceive(double? args1) => args1 ?? -1.0;

    [ServiceMethod]
    [ServiceMethodExport]
    private decimal? NullableDecimalArgsReceive(decimal? args1) => args1 * 1.5m;

    [ServiceMethod]
    [ServiceMethodExport]
    private decimal? NullableDecimalNullArgsReceive(decimal? args1) => args1 ?? -1m;

    [ServiceMethod]
    [ServiceMethodExport]
    private bool? NullableBoolArgsReceive(bool? args1) => !args1;

    [ServiceMethod]
    [ServiceMethodExport]
    private int NullableBoolNullArgsReceive(bool? args1) => args1 == null ? 1 : -1;

    [ServiceMethod]
    [ServiceMethodExport]
    private byte? NullableByteArgsReceive(byte? args1) => (byte?)(args1 + 10);

    [ServiceMethod]
    [ServiceMethodExport]
    private int NullableByteNullArgsReceive(byte? args1) => args1 == null ? 1 : -1;

    [ServiceMethod]
    [ServiceMethodExport]
    private sbyte? NullableSbyteArgsReceive(sbyte? args1) => (sbyte?)(-args1);

    [ServiceMethod]
    [ServiceMethodExport]
    private int NullableSbyteNullArgsReceive(sbyte? args1) => args1 == null ? 1 : -1;

    [ServiceMethod]
    [ServiceMethodExport]
    private short? NullableShortArgsReceive(short? args1) => (short?)(args1 + 100);

    [ServiceMethod]
    [ServiceMethodExport]
    private int NullableShortNullArgsReceive(short? args1) => args1 == null ? 1 : -1;

    [ServiceMethod]
    [ServiceMethodExport]
    private ushort? NullableUshortArgsReceive(ushort? args1) => (ushort?)(args1 * 2);

    [ServiceMethod]
    [ServiceMethodExport]
    private int NullableUshortNullArgsReceive(ushort? args1) => args1 == null ? 1 : -1;

    [ServiceMethod]
    [ServiceMethodExport]
    private uint? NullableUintArgsReceive(uint? args1) => args1 * 3;

    [ServiceMethod]
    [ServiceMethodExport]
    private int NullableUintNullArgsReceive(uint? args1) => args1 == null ? 1 : -1;

    [ServiceMethod]
    [ServiceMethodExport]
    private ulong? NullableUlongArgsReceive(ulong? args1) => args1 + 1000;

    [ServiceMethod]
    [ServiceMethodExport]
    private int NullableUlongNullArgsReceive(ulong? args1) => args1 == null ? 1 : -1;

    [ServiceMethod]
    [ServiceMethodExport]
    private char? NullableCharArgsReceive(char? args1) => char.ToUpper(args1!.Value);

    [ServiceMethod]
    [ServiceMethodExport]
    private char? NullableCharNullArgsReceive(char? args1) => args1 ?? '?';
}