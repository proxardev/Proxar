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


namespace ServiceIntegrationTesting.Models;

public static class SimpleTypeOps
{
    public static int Transform(int v) => v * 2 + 1;
    public static long Transform(long v) => v * 3;
    public static short Transform(short v) => (short)(v + 100);
    public static ushort Transform(ushort v) => (ushort)(v * 2);
    public static byte Transform(byte v) => (byte)(v + 10);
    public static sbyte Transform(sbyte v) => (sbyte)(-v);
    public static float Transform(float v) => v * 1.5f;
    public static double Transform(double v) => v * 2.0;
    public static decimal Transform(decimal v) => v * 1.1m;
    public static bool Transform(bool v) => !v;
    public static char Transform(char v) => char.ToUpper(v);
    public static uint Transform(uint v) => v * 2;
    public static ulong Transform(ulong v) => v + 1000UL;
}