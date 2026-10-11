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
using Proxar.ServiceCore.Interfaces;
using TestShared.TestClass;
namespace ServiceIntegrationTesting.TestClass;

public abstract class TestClass_MessageBase<TService, T> : TestClassBase
    where TService : ServiceBase
     where T : class, IServiceProxyBase
{

    [Theory]
    [InlineData(nameof(TestService_MessageProxy.SimpleArgs_SendReceiveSucc))]
    [InlineData(nameof(TestService_MessageProxy.SimpleArgs2_SendReceiveSucc))]

    [InlineData(nameof(TestService_MessageProxy.DateTime_SendReceiveSucc))]
    [InlineData(nameof(TestService_MessageProxy.Guid_SendReceiveSucc))]
    [InlineData(nameof(TestService_MessageProxy.ValueTuple_SendReceiveSucc))]

    [InlineData(nameof(TestService_MessageProxy.Point2D_SendReceiveSucc))]
    [InlineData(nameof(TestService_MessageProxy.Point3D_SendReceiveSucc))]
    [InlineData(nameof(TestService_MessageProxy.LabeledValue_SendReceiveSucc))]
    [InlineData(nameof(TestService_MessageProxy.StructWithClass_SendReceiveSucc))]

    // 40段
    [InlineData(nameof(TestService_MessageProxy.PlayerScore_SendReceiveSucc))]
    [InlineData(nameof(TestService_MessageProxy.PlayerInfo_SendReceiveSucc))]
    [InlineData(nameof(TestService_MessageProxy.TeamStats_SendReceiveSucc))]
    [InlineData(nameof(TestService_MessageProxy.PlayerWithPosition_SendReceiveSucc))]
    [InlineData(nameof(TestService_MessageProxy.SendPlayerInfoAndChannge_SendReceiveRightSucc))]

    /// 50段
    [InlineData(nameof(TestService_MessageProxy.ListInt_SendReceiveSucc))]
    [InlineData(nameof(TestService_MessageProxy.ListString_SendReceiveSucc))]
    [InlineData(nameof(TestService_MessageProxy.ListPoint3D_SendReceiveSucc))]
    [InlineData(nameof(TestService_MessageProxy.ListPlayerScore_SendReceiveSucc))]
    [InlineData(nameof(TestService_MessageProxy.IntArray_SendReceiveSucc))]
    [InlineData(nameof(TestService_MessageProxy.Dictionary_SendReceiveSucc))]
    [InlineData(nameof(TestService_MessageProxy.HashSet_SendReceiveSucc))]
    [InlineData(nameof(TestService_MessageProxy.ConcurrentDict_SendReceiveSucc))]

    [InlineData(nameof(TestService_MessageProxy.GenericStructInt_SendReceiveSucc))]
    [InlineData(nameof(TestService_MessageProxy.GenericStructString_SendReceiveSucc))]
    [InlineData(nameof(TestService_MessageProxy.GenericStructPlayerScore_SendReceiveSucc))]
    [InlineData(nameof(TestService_MessageProxy.GenericStructNestedGenericStruct_SendReceiveSucc))]
    [InlineData(nameof(TestService_MessageProxy.GenericStructStructWithClass_SendReceiveSucc))]
    [InlineData(nameof(TestService_MessageProxy.GenericClassInt_SendReceiveSucc))]
    [InlineData(nameof(TestService_MessageProxy.GenericClassListInt_SendReceiveSucc))]
    [InlineData(nameof(TestService_MessageProxy.GenericClassPoint3D_SendReceiveSucc))]
    [InlineData(nameof(TestService_MessageProxy.GenericClassStructWithClass_SendReceiveSucc))]
    [InlineData(nameof(TestService_MessageProxy.GenericClassGenericClassInt_SendReceiveSucc))]

    [InlineData(nameof(TestService_MessageProxy.IntString_SendReceiveSucc))]
    [InlineData(nameof(TestService_MessageProxy.IntPoint3D_SendReceiveSucc))]
    [InlineData(nameof(TestService_MessageProxy.IntPlayerScore_SendReceiveSucc))]
    [InlineData(nameof(TestService_MessageProxy.LongString_SendReceiveSucc))]
    [InlineData(nameof(TestService_MessageProxy.StringListInt_SendReceiveSucc))]
    [InlineData(nameof(TestService_MessageProxy.StringIntArray_SendReceiveSucc))]
    [InlineData(nameof(TestService_MessageProxy.EnumInt_SendReceiveSucc))]
    [InlineData(nameof(TestService_MessageProxy.EnumString_SendReceiveSucc))]
    [InlineData(nameof(TestService_MessageProxy.Point2DPoint3D_SendReceiveSucc))]
    [InlineData(nameof(TestService_MessageProxy.Point3DPlayerScore_SendReceiveSucc))]
    [InlineData(nameof(TestService_MessageProxy.IntEnum_SendReceiveSucc))]
    [InlineData(nameof(TestService_MessageProxy.StringEnum_SendReceiveSucc))]
    [InlineData(nameof(TestService_MessageProxy.ListIntListString_SendReceiveSucc))]
    [InlineData(nameof(TestService_MessageProxy.IntArrayStringArray_SendReceiveSucc))]
    [InlineData(nameof(TestService_MessageProxy.IntDateTime_SendReceiveSucc))]
    [InlineData(nameof(TestService_MessageProxy.StringGuid_SendReceiveSucc))]
    [InlineData(nameof(TestService_MessageProxy.Point3DListInt_SendReceiveSucc))]
    [InlineData(nameof(TestService_MessageProxy.PlayerScoreString_SendReceiveSucc))]
    [InlineData(nameof(TestService_MessageProxy.IntDecimal_SendReceiveSucc))]
    [InlineData(nameof(TestService_MessageProxy.LongPoint3D_SendReceiveSucc))]
    [InlineData(nameof(TestService_MessageProxy.StringTimeSpan_SendReceiveSucc))]

    [InlineData(nameof(TestService_MessageProxy.NullableInt_SendReceiveSucc))]
    [InlineData(nameof(TestService_MessageProxy.NullableIntNull_SendReceiveSucc))]
    [InlineData(nameof(TestService_MessageProxy.NullableLong_SendReceiveSucc))]
    [InlineData(nameof(TestService_MessageProxy.NullableLongNull_SendReceiveSucc))]
    [InlineData(nameof(TestService_MessageProxy.NullableFloat_SendReceiveSucc))]
    [InlineData(nameof(TestService_MessageProxy.NullableFloatNull_SendReceiveSucc))]
    [InlineData(nameof(TestService_MessageProxy.NullableDouble_SendReceiveSucc))]
    [InlineData(nameof(TestService_MessageProxy.NullableDoubleNull_SendReceiveSucc))]
    [InlineData(nameof(TestService_MessageProxy.NullableDecimal_SendReceiveSucc))]
    [InlineData(nameof(TestService_MessageProxy.NullableDecimalNull_SendReceiveSucc))]
    [InlineData(nameof(TestService_MessageProxy.NullableBool_SendReceiveSucc))]
    [InlineData(nameof(TestService_MessageProxy.NullableBoolNull_SendReceiveSucc))]
    [InlineData(nameof(TestService_MessageProxy.NullableByte_SendReceiveSucc))]
    [InlineData(nameof(TestService_MessageProxy.NullableByteNull_SendReceiveSucc))]
    [InlineData(nameof(TestService_MessageProxy.NullableSbyte_SendReceiveSucc))]
    [InlineData(nameof(TestService_MessageProxy.NullableSbyteNull_SendReceiveSucc))]
    [InlineData(nameof(TestService_MessageProxy.NullableShort_SendReceiveSucc))]
    [InlineData(nameof(TestService_MessageProxy.NullableShortNull_SendReceiveSucc))]
    [InlineData(nameof(TestService_MessageProxy.NullableUshort_SendReceiveSucc))]
    [InlineData(nameof(TestService_MessageProxy.NullableUshortNull_SendReceiveSucc))]
    [InlineData(nameof(TestService_MessageProxy.NullableUint_SendReceiveSucc))]
    [InlineData(nameof(TestService_MessageProxy.NullableUintNull_SendReceiveSucc))]
    [InlineData(nameof(TestService_MessageProxy.NullableUlong_SendReceiveSucc))]
    [InlineData(nameof(TestService_MessageProxy.NullableUlongNull_SendReceiveSucc))]
    [InlineData(nameof(TestService_MessageProxy.NullableChar_SendReceiveSucc))]
    [InlineData(nameof(TestService_MessageProxy.NullableCharNull_SendReceiveSucc))]

    [InlineData(nameof(TestService_MessageProxy.Int_SendReceiveSucc))]
    [InlineData(nameof(TestService_MessageProxy.Long_SendReceiveSucc))]
    [InlineData(nameof(TestService_MessageProxy.Short_SendReceiveSucc))]
    [InlineData(nameof(TestService_MessageProxy.Byte_SendReceiveSucc))]
    [InlineData(nameof(TestService_MessageProxy.Sbyte_SendReceiveSucc))]
    [InlineData(nameof(TestService_MessageProxy.Float_SendReceiveSucc))]
    [InlineData(nameof(TestService_MessageProxy.Double_SendReceiveSucc))]
    [InlineData(nameof(TestService_MessageProxy.Decimal_SendReceiveSucc))]
    [InlineData(nameof(TestService_MessageProxy.Bool_SendReceiveSucc))]
    [InlineData(nameof(TestService_MessageProxy.Char_SendReceiveSucc))]
    [InlineData(nameof(TestService_MessageProxy.Uint_SendReceiveSucc))]
    [InlineData(nameof(TestService_MessageProxy.Ulong_SendReceiveSucc))]
    [InlineData(nameof(TestService_MessageProxy.Ushort_SendReceiveSucc))]

    [InlineData(nameof(TestService_MessageProxy.String_SendReceiveSucc))]
    [InlineData(nameof(TestService_MessageProxy.EnumStatusCode_SendReceiveSucc))]
    [InlineData(nameof(TestService_MessageProxy.EnumPriority_SendReceiveSucc))]
    [InlineData(nameof(TestService_MessageProxy.EnumFlags_SendReceiveSucc))]
    [InlineData(nameof(TestService_MessageProxy.EnumByte_SendReceiveSucc))]

    //170段
    [InlineData(nameof(TestService_MessageProxy.Send0DataArgs_SendMethodAndReceiveSucc))]
    [InlineData(nameof(TestService_MessageProxy.Send1DataArg_SendMethodAndReceiveSucc))]
    [InlineData(nameof(TestService_MessageProxy.Send2DataArgs_SendMethodAndReceiveSucc))]
    [InlineData(nameof(TestService_MessageProxy.Send3DataArgs_SendMethodAndReceiveSucc))]
    [InlineData(nameof(TestService_MessageProxy.Send4DataArgs_SendMethodAndReceiveSucc))]
    [InlineData(nameof(TestService_MessageProxy.Send5DataArgs_SendMethodAndReceiveSucc))]
    [InlineData(nameof(TestService_MessageProxy.Send6DataArgs_SendMethodAndReceiveSucc))]
    [InlineData(nameof(TestService_MessageProxy.Send7DataArgs_SendMethodAndReceiveSucc))]

    // 150段
    [InlineData(nameof(TestService_MessageProxy.Call1Arg_SendReceiveSucc))]
    [InlineData(nameof(TestService_MessageProxy.Call2Args_SendReceiveSucc))]
    [InlineData(nameof(TestService_MessageProxy.Call3Args_SendReceiveSucc))]
    [InlineData(nameof(TestService_MessageProxy.Call4Args_SendReceiveSucc))]
    [InlineData(nameof(TestService_MessageProxy.Call5Args_SendReceiveSucc))]
    [InlineData(nameof(TestService_MessageProxy.Call6Args_SendReceiveSucc))]
    [InlineData(nameof(TestService_MessageProxy.Call7Args_SendReceiveSucc))]
    [InlineData(nameof(TestService_MessageProxy.Call8Args_SendReceiveSucc))]
    [InlineData(nameof(TestService_MessageProxy.Call9Args_SendReceiveSucc))]

    [InlineData(nameof(TestService_MessageProxy.SendIntList_ToEnumerable_Success))]
    [InlineData(nameof(TestService_MessageProxy.SendIntList_ToReadOnlyList_Success))]
    [InlineData(nameof(TestService_MessageProxy.SendIntList_ToReadOnlyCollection_Success))]
    [InlineData(nameof(TestService_MessageProxy.SendIntArray_ToEnumerable_Success))]
    [InlineData(nameof(TestService_MessageProxy.SendIntArray_ToReadOnlyList_Success))]
    [InlineData(nameof(TestService_MessageProxy.SendIntArray_ToReadOnlyCollection_Success))]
    [InlineData(nameof(TestService_MessageProxy.SendIntHashSet_ToEnumerable_Success))]
    [InlineData(nameof(TestService_MessageProxy.SendIntHashSet_ToReadOnlyCollection_Success))]
    [InlineData(nameof(TestService_MessageProxy.SendIntDictionary_ToReadOnlyDictionary_Success))]
    [InlineData(nameof(TestService_MessageProxy.SendPlayerScoreList_ToEnumerable_Success))]
    [InlineData(nameof(TestService_MessageProxy.SendPlayerScoreList_ToReadOnlyList_Success))]
    [InlineData(nameof(TestService_MessageProxy.SendPlayerScoreList_ToReadOnlyCollection_Success))]
    [InlineData(nameof(TestService_MessageProxy.SendPlayerScoreArray_ToEnumerable_Success))]
    [InlineData(nameof(TestService_MessageProxy.SendPlayerScoreArray_ToReadOnlyList_Success))]
    [InlineData(nameof(TestService_MessageProxy.SendPlayerScoreArray_ToReadOnlyCollection_Success))]
    [InlineData(nameof(TestService_MessageProxy.SendPlayerScoreHashSet_ToEnumerable_Success))]
    [InlineData(nameof(TestService_MessageProxy.SendPlayerScoreHashSet_ToReadOnlyCollection_Success))]
    [InlineData(nameof(TestService_MessageProxy.SendIntKeyPlayerScoreDict_ToReadOnlyDictionary_Success))]
    [InlineData(nameof(TestService_MessageProxy.SendPlayerScoreKeyDict_ToReadOnlyDictionary_Success))]

    [InlineData(nameof(TestService_MessageProxy.Call_ExceptionOnRemote_CatchException))]
    internal async Task CallMethodTest(string methodName, int args = int.MinValue, int args2 = int.MinValue,
        int args3 = int.MinValue,
        int args4 = int.MinValue
        )
    {
        await this.CallMethod<TestService_Message>(methodName, args, args2, args3, args4);
    }
}