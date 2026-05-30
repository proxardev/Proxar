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
            "ZF0001",
            "呼叫服务方法实参类型约束",
            "实参类型期望:{0}. 实际类型:{1}. 目标方法:{2}.",
            "Usage",
            DiagnosticSeverity.Error,
            isEnabledByDefault: true,
            "实参类型，应与形参类型一致."
            );

        internal static readonly DiagnosticDescriptor ErrorRule_0002 = new DiagnosticDescriptor(
            "ZF0002",
            "呼叫服务方法实参数量约束",
            "实参数量期望:{0}. 实际数量:{1}. 目标方法:{2}.",
            "Usage",
            DiagnosticSeverity.Error,
            isEnabledByDefault: true,
            "实参数量，应与形参数量一致."
            );

        internal static readonly DiagnosticDescriptor ErrorRule_0003 = new DiagnosticDescriptor(
            "ZF0003",
            "服务方法协议唯一性约束",
            "协议 {0} 重复使用, 相同协议方法: {1}",
            "Usage",
            DiagnosticSeverity.Error,
            isEnabledByDefault: true,
            "一个服务中，禁止多次使用同一个协议号."
            );

        internal static readonly DiagnosticDescriptor ErrorRule_0004 = new DiagnosticDescriptor(
            "ZF0004",
            "服务方法保留协议约束",
            "协议 {0} 为保留协议，不允许业务使用.{1}为保留协议段",
            "Usage",
            DiagnosticSeverity.Error,
            isEnabledByDefault: true,
            "保留协议段为内部使用，不允许业务使用."
            );

        internal static readonly DiagnosticDescriptor ErrorRule_0005 = new DiagnosticDescriptor(
            "ZF0004",
            "协议定义范围约束",
            "协议 {0} 为无效协议，协议应该大于等于 {1}",
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
            "ZF0999",
            "分析器处理异常",
            "错误信息:{0}",
            "Usage",
            DiagnosticSeverity.Error,
            isEnabledByDefault: true,
            ""
            );


    }
}