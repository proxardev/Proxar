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
using System.Linq;

namespace Proxar.Analyzers
{
    /// <summary>判定「符号是否受 [RestrictedAccess] 约束」以及「调用方是否被允许引用它」。</summary>
    internal static class SymbolRestrictionChecker
    {
        /// <summary>特性名称（按简单名匹配，避免强依赖具体程序集）。</summary>
        private const string AttributeName = "RestrictedAccessAttribute";
        private static readonly SymbolDisplayFormat NameFormat =
            SymbolDisplayFormat.MinimallyQualifiedFormat;

        /// <summary>
        /// 判断指定符号（或其所实现/重写的链）是否被 [RestrictedAccess] 标记。
        /// 处理：
        /// 1) 符号自身带特性；
        /// 2) 显式接口实现：沿 ExplicitInterfaceImplementations 递归；
        /// 3) 重写链：沿 OverriddenMethod 向上。
        /// 4) 类型成员：若其声明类型被标记，则成员也受限。
        /// </summary>
        public static bool IsRestricted(ISymbol symbol)
        {
            if (symbol == null) return false;

            // 成员：自身 + 接口实现链 + 重写链
            if (symbol is IMethodSymbol method)
            {
                if (HasAttribute(method)) return true;
                foreach (var impl in method.ExplicitInterfaceImplementations)
                    if (IsRestricted(impl)) return true;
                var overridden = method.OverriddenMethod;
                while (overridden != null)
                {
                    if (HasAttribute(overridden)) return true;
                    overridden = overridden.OverriddenMethod;
                }
            }
            else if (symbol is IPropertySymbol property)
            {
                if (HasAttribute(property)) return true;
                foreach (var impl in property.ExplicitInterfaceImplementations)
                    if (IsRestricted(impl)) return true;
            }
            else if (symbol is IEventSymbol eventSymbol)
            {
                if (HasAttribute(eventSymbol)) return true;
                foreach (var impl in eventSymbol.ExplicitInterfaceImplementations)
                    if (IsRestricted(impl)) return true;
            }
            else if (symbol is IFieldSymbol field)
            {
                if (HasAttribute(field)) return true;
            }

            // 类型成员：声明类型被标记 → 其所有成员受限
            if (symbol is not INamedTypeSymbol && symbol.ContainingType != null)
            {
                if (HasAttribute(symbol.ContainingType)) return true;
            }

            // 类型符号本身
            if (symbol is INamedTypeSymbol namedType)
            {
                if (HasAttribute(namedType)) return true;
            }

            return false;
        }

        /// <summary>
        /// 判断「调用方类型」是否允许引用「被限制的符号」。
        /// 规则：调用方类型必须是「符号声明类型」本身，或其嵌套类型（含多层嵌套）。
        /// 若 restrictedSymbol 本身是类型符号（如把受限类用作参数/基类），则其「声明类型」视为自身。
        /// </summary>
        public static bool IsAllowed(ITypeSymbol callerType, ISymbol restrictedSymbol)
        {
            if (callerType == null) return false;

            // 被限制符号的「声明类型」：类型符号自身；成员符号取其 ContainingType
            INamedTypeSymbol declaredType = restrictedSymbol as INamedTypeSymbol
                ?? restrictedSymbol.ContainingType;
            if (declaredType == null) return false; // 全局符号（如命名空间函数）罕见，保守放行交由外层判断

            // 沿 callerType 的嵌套链向上，看是否命中 declaredType
            var current = callerType;
            while (current != null)
            {
                if (SymbolEqualityComparer.Default.Equals(current, declaredType))
                    return true;
                current = current.ContainingType;
            }
            return false;
        }

        /// <summary>生成诊断消息中的「被限制成员」可读名。</summary>
        public static string FormatSymbol(ISymbol symbol) =>
            symbol.ToDisplayString(NameFormat);

        private static bool HasAttribute(ISymbol symbol) =>
            symbol.GetAttributes().Any(attr =>
            {
                var name = attr.AttributeClass?.Name;
                return name == AttributeName
                    || name == AttributeName.Substring(0, AttributeName.Length - "Attribute".Length);
            });
    }
}
