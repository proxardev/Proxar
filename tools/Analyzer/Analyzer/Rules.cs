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

namespace Analyzer
{
    public static class Rules
    {

        public static bool OpenDebugOutput = false;

        internal static readonly DiagnosticDescriptor ErrorRule_0001 = new DiagnosticDescriptor(
            "P0001",
            "呼叫服务方法实参类型约束",
            "实参类型期望:{0}. 实际类型:{1}. 目标方法:{2}.",
            "Usage",
            DiagnosticSeverity.Error,
            isEnabledByDefault: true,
            "实参类型，应与形参类型一致."
            );

        internal static readonly DiagnosticDescriptor ErrorRule_0002 = new DiagnosticDescriptor(
            "P0002",
            "呼叫服务方法实参数量约束",
            "实参数量期望:{0}. 实际数量:{1}. 目标方法:{2}.",
            "Usage",
            DiagnosticSeverity.Error,
            isEnabledByDefault: true,
            "实参数量，应与形参数量一致."
            );

        internal static readonly DiagnosticDescriptor ErrorRule_0003 = new DiagnosticDescriptor(
            "P0003",
            "服务方法协议唯一性约束",
            "协议 {0} 重复使用, 相同协议方法: {1}",
            "Usage",
            DiagnosticSeverity.Error,
            isEnabledByDefault: true,
            "一个服务中，禁止多次使用同一个协议号."
            );

        internal static readonly DiagnosticDescriptor ErrorRule_0004 = new DiagnosticDescriptor(
            "P0004",
            "服务方法保留协议约束",
            "协议 {0} 为保留协议，不允许业务使用.{1}为保留协议段",
            "Usage",
            DiagnosticSeverity.Error,
            isEnabledByDefault: true,
            "保留协议段为内部使用，不允许业务使用."
            );

        internal static readonly DiagnosticDescriptor ErrorRule_0005 = new DiagnosticDescriptor(
            "P0005",
            "协议定义范围约束",
            "协议 {0} 为无效协议，协议应该大于等于 {1}",
            "Usage",
            DiagnosticSeverity.Error,
            isEnabledByDefault: true
            );

        internal static readonly DiagnosticDescriptor ErrorRule_1001 = new DiagnosticDescriptor(
            "P1001",
            "项目未定义外部代理ID前缀",
            "项目标记使用 [{0}]，但未在 .csproj 中配置 {1} 属性。请添加 <{2}>值</{3}> 并确保值大于 0。同时声明 <CompilerVisibleProperty Include=\"{4}\" /> 以使分析器可见。",
            "Usage",
            DiagnosticSeverity.Error,
            isEnabledByDefault: true
            );

        internal static readonly DiagnosticDescriptor ErrorRule_1002 = new DiagnosticDescriptor(
            "P1002",
            "外部代理ID值范围不合法",
            "使用 [{0}] 特性，配置值{1}不合法，配置值范围应为{2}-{3}",
            "Usage",
            DiagnosticSeverity.Error,
            isEnabledByDefault: true
            );

        internal static readonly DiagnosticDescriptor ErrorRule_1003 = new DiagnosticDescriptor(
            "P1003",
            "外部代理ID值重复",
            "使用 [{0}] 特性，配置值不应重复，配置Id为{1}，相同Id类为：{2}",
            "Usage",
            DiagnosticSeverity.Error,
            isEnabledByDefault: true
            );

        // 定义信息级别诊断
        internal static readonly DiagnosticDescriptor DebugRule = new DiagnosticDescriptor(
            "DEBUG001",
            "调试信息",
            "[DEBUG] {0}",
            "Debug",
            DiagnosticSeverity.Warning,
            isEnabledByDefault: true,
            ""
            );

        internal static readonly DiagnosticDescriptor ErrorRule_0999 = new DiagnosticDescriptor(
            "P0999",
            "分析器处理异常",
            "错误信息:{0}",
            "Usage",
            DiagnosticSeverity.Error,
            isEnabledByDefault: true,
            ""
            );


    }
}