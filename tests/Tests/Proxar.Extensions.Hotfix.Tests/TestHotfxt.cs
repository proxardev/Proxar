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
using Xunit.Priority;

namespace Proxar.Extensions.Hotfix.Tests;

//[TestCaseOrderer(typeof(TestOrder))]
[TestCaseOrderer("Xunit.Priority.PriorityOrderer", "Xunit.Priority")]
public class TestHotfxt
{
    public static bool ApplyHotfix(out string? error)
    {
        bool ok = HotfixManager.Instance.LoadHotfixPatch("../../../Proxar.Extensions.Hotfix.Tests.Patch/bin/Proxar.Extensions.Hotfix.Tests.Patch", out error);

        return ok;
    }


    [Fact]
    [Priority(-100)]
    public void ApplyHotfix_Should_Override_Behavior()
    {
        var calc = new Calculator();

        bool ok = ApplyHotfix(out var errInfo);

        ok.Should().BeTrue(errInfo);
        calc.Add(2, 3).Should().Be(6); // 乘法
        calc.Add((long)2, 3).Should().Be(2 * 3 + 3);
        StaticClassHotfix.ExecuteAction(Test);
        HotfixManager.Instance.Version.Should().Be(1);
    }

    private static void Test()
    {

    }


    [Fact]
    public void Hotfix_SharedAssembly_IsNotDuplicated()
    {
        // 获取热更前共有程序集的实例
        var originalAsm = typeof(Calculator).Assembly;


        // 重新获取当前域中同名的程序集实例
        var afterAsm = AppDomain.CurrentDomain.GetAssemblies()
            .Where(a => a.GetName().Name == originalAsm.GetName().Name)
            .ToList();

        // 必须是同一个实例，否则说明被重复加载了
        afterAsm.Count.Should().Be(1);
        originalAsm.Should().BeSameAs(afterAsm.First());
    }
}
