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


using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Testing;          // 新增
using Microsoft.CodeAnalysis.Testing.Verifiers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace Analyzer.Test
{
    [TestClass]
    public class AnalyzerServiceDeclaration
    {
        [TestMethod]
        public async Task TestMethod1()
        {
            var testCode = @"
using Proxar.ServiceCore;

namespace QuickStartDemo3
{
    internal partial class MyService1 : ServiceBase
    {
        [ServiceMethod(1)]
        private void Test()
        {

        }

        [ServiceMethod(1)]
        private void Test2()
        {

        }

    }
}";



            // 获取 Proxar.Core 程序集的路径（通过其中的一个公开类型）
            string corePath = typeof(Proxar.ServiceCore.ServiceBase).Assembly.Location;

            // 创建一个 CSharpAnalyzerTest 实例，它是非静态类，可配置
            var test = new CSharpAnalyzerTest<ServiceMethodDeclarationAnalyzer, MSTestVerifier>
            {
                TestCode = testCode,
            };

            // 将 Proxar.Core 程序集添加到编译引用中
            test.TestState.AdditionalReferences.Add(
                MetadataReference.CreateFromFile(corePath));

            // 添加预期的诊断：ID 为 "P1003"，严重性为 Error
            test.ExpectedDiagnostics.Add(
                new DiagnosticResult("P1003", DiagnosticSeverity.Error));

            await test.RunAsync();
        }
    }
}