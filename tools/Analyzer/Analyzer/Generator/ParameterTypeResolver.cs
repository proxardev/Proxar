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
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace ServiceDispatchMethodGenerator
{

    public class ParameterTypeResolver
    {
        /// <summary>
        /// 获取参数类型的完整CLR类型名称
        /// </summary>
        public static string GetFullTypeName(ParameterSyntax parameter, SemanticModel semanticModel)
        {
            // 获取参数的类型语法节点
            TypeSyntax typeSyntax = parameter.Type;

            if (typeSyntax == null)
                return null;

            // 通过语义模型获取类型符号信息
            TypeInfo typeInfo = semanticModel.GetTypeInfo(typeSyntax);
            ITypeSymbol typeSymbol = typeInfo.Type;

            if (typeSymbol == null)
                return typeSyntax.ToString(); // 回退到原始文本

            // 返回完整的CLR类型名称
            return typeSymbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
        }

        /// <summary>
        /// 获取简化的完整名称（不带global::前缀）
        /// </summary>
        public static string GetFullTypeNameWithoutGlobal(ITypeSymbol typeSymbol, SemanticModel semanticModel)
        {
            // 使用自定义格式，去掉 global:: 前缀
            var format = new SymbolDisplayFormat(
                typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameAndContainingTypesAndNamespaces,
                genericsOptions: SymbolDisplayGenericsOptions.IncludeTypeParameters
            );

            return typeSymbol.ToDisplayString(format);
        }
    }
}