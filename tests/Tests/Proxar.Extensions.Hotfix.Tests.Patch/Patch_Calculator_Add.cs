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

using HarmonyLib;

namespace Proxar.Extensions.Hotfix.Tests.Patch;


[HarmonyPatch(typeof(Calculator), nameof(Calculator.Add), new Type[] { typeof(long), typeof(int) })]
public static class Patch_Calculator_Add2_IntInt
{
    public static bool Prefix(long a, int b, ref long __result)
    {
        __result = a * b + b;
        return false;
    }
}


[HarmonyPatch(typeof(Calculator), nameof(Calculator.Add), new Type[] { typeof(int), typeof(int) })]
public static class Patch_Calculator_Add
{
    public static bool Prefix(int a, int b, ref int __result)
    {
        __result = a * b; // 改为乘法
        return false; // 跳过原方法
    }
}