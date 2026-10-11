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


using MessagePack;

namespace ServiceIntegrationTesting;

[MessagePackObject]
public struct GenericStruct<T>
{
    [Key(0)] public T Value;
    [Key(1)] public int Count;

    public GenericStruct(T value, int count) { Value = value; Count = count; }
}

[MessagePackObject]
public class GenericClass<T>
{
    [Key(0)] public T Data { get; set; } = default!;
    [Key(1)] public int Id { get; set; }

    public int Compute() => Id * 100;
}