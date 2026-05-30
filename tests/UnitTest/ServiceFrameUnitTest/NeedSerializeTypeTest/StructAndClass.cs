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


namespace FrameUnitTest.NeedSerializeTypeTest;
#pragma warning disable CS0649

struct SimpleStruct { }
struct StructWithValueTypeField
{
    public int Value;
}
struct StructWithStringField
{
    public string Value;
}
struct StructWithStructField
{
    public StructWithValueTypeField Value;
}

struct StructWithClassField
{
    public SimpleClass Value;
}

class SimpleClass { }

class SimpleClassWithValueTypeField
{
    public int Value;
}

class SimpleClassWithStringTypeField
{
    public string? Value;
}

class SimpleClassWithStructTypeField
{
    public SimpleStruct Value;
}

class SimpleClassWithClassField
{
    public SimpleClassWithValueTypeField? Value;
}

class GenericClass<T>
{
    public T? Value;
}

enum MyEnum { A, B, C }

struct GenericStruct<T>
{
    public T Value;
}

#pragma warning restore CS0649