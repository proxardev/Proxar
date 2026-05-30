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


using FluentAssertions;
using Proxar.ServiceCore;
using System.Collections.Concurrent;
using System.Reflection;

namespace FrameUnitTest.NeedSerializeTypeTest;
#pragma warning disable CS0649

public class Test
{

    [Theory]

    [InlineData(typeof(byte))]
    [InlineData(typeof(sbyte))]

    [InlineData(typeof(byte?))]
    [InlineData(typeof(sbyte?))]



    [InlineData(typeof(short))]
    [InlineData(typeof(ushort))]

    [InlineData(typeof(short?))]
    [InlineData(typeof(ushort?))]



    [InlineData(typeof(int))]
    [InlineData(typeof(uint))]

    [InlineData(typeof(int?))]
    [InlineData(typeof(uint?))]



    [InlineData(typeof(long))]
    [InlineData(typeof(ulong))]

    [InlineData(typeof(long?))]
    [InlineData(typeof(ulong?))]



    [InlineData(typeof(float))]
    [InlineData(typeof(double))]
    [InlineData(typeof(decimal))]

    [InlineData(typeof(float?))]
    [InlineData(typeof(double?))]
    [InlineData(typeof(decimal?))]


    [InlineData(typeof(bool))]
    [InlineData(typeof(char))]


    [InlineData(typeof(bool?))]
    [InlineData(typeof(char?))]


    [InlineData(typeof(string))]

    //特殊处理

    [InlineData(typeof(DateTime))]
    [InlineData(typeof(TimeSpan))]
    [InlineData(typeof(Guid))]


    [InlineData(typeof(DateTime?))]
    [InlineData(typeof(TimeSpan?))]
    [InlineData(typeof(Guid?))]



    [InlineData(typeof(SimpleStruct))]
    [InlineData(typeof(StructWithValueTypeField))]

    [InlineData(typeof(SimpleStruct?))]
    [InlineData(typeof(StructWithValueTypeField?))]



    [InlineData(typeof(StructWithStructField))]
    [InlineData(typeof(StructWithStructField?))]




    [InlineData(typeof(DayOfWeek))]

    //enum
    [InlineData(typeof(DayOfWeek?))]

    //enum
    [InlineData(typeof((int, int)))]

    //值元组
    [InlineData(typeof((int, int)?))]

    //值元组
    [InlineData(typeof((int, StructWithValueTypeField)))]


    [InlineData(typeof((int, StructWithValueTypeField)?))]



    //泛型结构体
    [InlineData(typeof(GenericStruct<int>))]

    [InlineData(typeof(GenericStruct<int>?))]


    [InlineData(typeof(GenericStruct<StructWithValueTypeField>))]

    [InlineData(typeof(GenericStruct<StructWithValueTypeField>?))]
    public void TestNotNeedSerialize(Type type)
    {
        var method = typeof(Service)!
            .GetMethod(nameof(Service.IsNeedSerialize), BindingFlags.Static | BindingFlags.NonPublic)!
            .MakeGenericMethod(type)!;
        var result = method.Invoke(null, null);
        result.Should().NotBeNull();
        var result2 = (bool)result;
        result2.Should().BeFalse($"Type {type.Name} should not need serialize");
    }



    [Theory]
    [InlineData(typeof(SimpleClass))]
    [InlineData(typeof(SimpleClassWithValueTypeField))]
    [InlineData(typeof(SimpleClassWithStringTypeField))]
    [InlineData(typeof(SimpleClassWithStructTypeField))]
    [InlineData(typeof(SimpleClassWithClassField))]
    [InlineData(typeof(List<int>))]
    [InlineData(typeof(List<string>))]
    [InlineData(typeof(HashSet<int>))]
    [InlineData(typeof(Queue<int>))]
    [InlineData(typeof(ConcurrentQueue<int>))]
    [InlineData(typeof(ConcurrentStack<int>))]
    [InlineData(typeof(List<SimpleStruct>))]
    [InlineData(typeof(Dictionary<int, long>))]

    [InlineData(typeof(int[]))]
    [InlineData(typeof(int?[]))]
    [InlineData(typeof(long[]))]
    [InlineData(typeof(long[,]))]

    [InlineData(typeof((int, string)))]
    [InlineData(typeof((int, SimpleClass)))]

    [InlineData(typeof(StructWithStringField))]
    [InlineData(typeof(StructWithClassField))]
    [InlineData(typeof(GenericStruct<string>))]
    [InlineData(typeof(GenericStruct<SimpleClass>))]

    [InlineData(typeof(GenericClass<int>))]
    [InlineData(typeof(GenericClass<StructWithStringField>))]
    [InlineData(typeof(GenericClass<StructWithStructField>))]
    [InlineData(typeof(GenericClass<SimpleClass>))]
    [InlineData(typeof(GenericClass<List<int>>))]

    [InlineData(typeof(IEnumerable<int>))]
    public void TestNeedSerialize(Type type)
    {
        var method = typeof(Service)!
            .GetMethod(nameof(Service.IsNeedSerialize), BindingFlags.Static | BindingFlags.NonPublic)!
            .MakeGenericMethod(type)!;
        var result = method.Invoke(null, null);
        result.Should().NotBeNull();
        var result2 = (bool)result;
        result2.Should().BeTrue($"Type {type.Name} should need serialize");
    }
}
#pragma warning restore CS0649