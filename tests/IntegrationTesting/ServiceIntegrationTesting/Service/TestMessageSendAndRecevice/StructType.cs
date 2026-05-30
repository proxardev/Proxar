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

namespace ServiceIntegrationTesting.Models;

[MessagePackObject]
public struct Point2D
{
    [Key(0)]
    public int X;
    [Key(1)]
    public int Y;
    public Point2D(int x, int y) { X = x; Y = y; }
    public int Sum() => X + Y;
}

[MessagePackObject]
public struct Point3D
{
    [Key(0)]
    public int X;
    [Key(1)]
    public int Y;
    [Key(2)]
    public int Z;
    public Point3D(int x, int y, int z) { X = x; Y = y; Z = z; }
    public int Sum() => X + Y + Z;
    public Point3D Double() => new(X * 2, Y * 2, Z * 2);
}

[MessagePackObject]
public struct LabeledValue
{
    [Key(0)]
    public int Value;
    [Key(1)]
    public string Label;
    public LabeledValue(int value, string label) { Value = value; Label = label; }
}

[MessagePackObject]
public class SimpleClass
{
    [Key(0)]
    public int Id { get; set; }
    public int Compute() => Id * 100;
}

[MessagePackObject]
public struct StructWithClass
{
    [Key(0)]
    public int Value;
    [Key(1)]
    public SimpleClass? Ref;
    public StructWithClass(int value, SimpleClass? r) { Value = value; Ref = r; }
}