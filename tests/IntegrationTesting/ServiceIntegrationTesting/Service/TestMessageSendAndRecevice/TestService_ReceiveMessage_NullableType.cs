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
    [ServiceMethod(60)]
    [ServiceMethodExport]
    private int? NullableIntArgsReceive(int? args1) => args1 * 2;

    [ServiceMethod(61)]
    [ServiceMethodExport]
    private int? NullableIntNullArgsReceive(int? args1) => args1 ?? -1;

    [ServiceMethod(62)]
    [ServiceMethodExport]
    private long? NullableLongArgsReceive(long? args1) => args1 + 50;

    [ServiceMethod(63)]
    [ServiceMethodExport]
    private long? NullableLongNullArgsReceive(long? args1) => args1 ?? 0L;

    [ServiceMethod(64)]
    [ServiceMethodExport]
    private float? NullableFloatArgsReceive(float? args1) => args1 * 2;

    [ServiceMethod(65)]
    [ServiceMethodExport]
    private float? NullableFloatNullArgsReceive(float? args1) => args1 ?? -1.0f;

    [ServiceMethod(66)]
    [ServiceMethodExport]
    private double? NullableDoubleArgsReceive(double? args1) => args1 * 3;

    [ServiceMethod(67)]
    [ServiceMethodExport]
    private double? NullableDoubleNullArgsReceive(double? args1) => args1 ?? -1.0;

    [ServiceMethod(68)]
    [ServiceMethodExport]
    private decimal? NullableDecimalArgsReceive(decimal? args1) => args1 * 1.5m;

    [ServiceMethod(69)]
    [ServiceMethodExport]
    private decimal? NullableDecimalNullArgsReceive(decimal? args1) => args1 ?? -1m;

    [ServiceMethod(70)]
    [ServiceMethodExport]
    private bool? NullableBoolArgsReceive(bool? args1) => !args1;

    [ServiceMethod(71)]
    [ServiceMethodExport]
    private int NullableBoolNullArgsReceive(bool? args1) => args1 == null ? 1 : -1;

    [ServiceMethod(72)]
    [ServiceMethodExport]
    private byte? NullableByteArgsReceive(byte? args1) => (byte?)(args1 + 10);

    [ServiceMethod(73)]
    [ServiceMethodExport]
    private int NullableByteNullArgsReceive(byte? args1) => args1 == null ? 1 : -1;

    [ServiceMethod(74)]
    [ServiceMethodExport]
    private sbyte? NullableSbyteArgsReceive(sbyte? args1) => (sbyte?)(-args1);

    [ServiceMethod(75)]
    [ServiceMethodExport]
    private int NullableSbyteNullArgsReceive(sbyte? args1) => args1 == null ? 1 : -1;

    [ServiceMethod(76)]
    [ServiceMethodExport]
    private short? NullableShortArgsReceive(short? args1) => (short?)(args1 + 100);

    [ServiceMethod(77)]
    [ServiceMethodExport]
    private int NullableShortNullArgsReceive(short? args1) => args1 == null ? 1 : -1;

    [ServiceMethod(78)]
    [ServiceMethodExport]
    private ushort? NullableUshortArgsReceive(ushort? args1) => (ushort?)(args1 * 2);

    [ServiceMethod(79)]
    [ServiceMethodExport]
    private int NullableUshortNullArgsReceive(ushort? args1) => args1 == null ? 1 : -1;

    [ServiceMethod(80)]
    [ServiceMethodExport]
    private uint? NullableUintArgsReceive(uint? args1) => args1 * 3;

    [ServiceMethod(81)]
    [ServiceMethodExport]
    private int NullableUintNullArgsReceive(uint? args1) => args1 == null ? 1 : -1;

    [ServiceMethod(82)]
    [ServiceMethodExport]
    private ulong? NullableUlongArgsReceive(ulong? args1) => args1 + 1000;

    [ServiceMethod(83)]
    [ServiceMethodExport]
    private int NullableUlongNullArgsReceive(ulong? args1) => args1 == null ? 1 : -1;

    [ServiceMethod(84)]
    [ServiceMethodExport]
    private char? NullableCharArgsReceive(char? args1) => char.ToUpper(args1!.Value);

    [ServiceMethod(85)]
    [ServiceMethodExport]
    private char? NullableCharNullArgsReceive(char? args1) => args1 ?? '?';
}