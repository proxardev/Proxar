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
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Analyzer
{
    [Flags]
    public enum InvocationType : ushort
    {
        None = 0,
        Call = 1,
    }

    public class ServiceInvokeInfo
    {

        public SyntaxNodeAnalysisContext context { get; set; }
        public ExpressionSyntax ProtoExpr { get; set; }
        public List<ExpressionSyntax> ArgsExprList { get; set; }
        public InvocationExpressionSyntax InvocationExpressionSyntax { get; set; }

        public InvocationType InvocationType { get; set; } = InvocationType.None;


        public void SetFlag(InvocationType flag)
        {
            this.InvocationType |= flag;
        }

        public bool HasFlag(InvocationType flag)
        {
            return (this.InvocationType & flag) == flag;
        }


        public IMethodSymbol GetServiceMethod()
        {
            var service = context.Compilation.GetTypeByMetadataName($"{GetServiceNamespace()}.{GetServiceProxyName()}");
            return service.GetMembers(GetServiceProtoMethodStrName())
                .OfType<IMethodSymbol>()
                .FirstOrDefault();
        }

        public List<ITypeSymbol> GetProtoMethodParametersTypeList()
        {
            var method = GetServiceMethod();
            if (method == null)
            {
                return null;
            }
            var list = method.Parameters.Select(x => x.Type).ToList();
            return list;
        }

        public List<ITypeSymbol> GetCallMethodParametersTypeList()
        {
            var genericTypeSymbols = GetGenericTypeSymbolList();
            if (genericTypeSymbols != null)
            {
                return genericTypeSymbols;
            }

            var semanticModel = context.SemanticModel;
            var res = ArgsExprList
                .Select(argsExpr => semanticModel.GetTypeInfo(argsExpr).Type)
                .ToList();
            return res;
        }

        public string GetServiceNamespace()
        {
            var semanticModel = context.SemanticModel;
            var symbolInfo = semanticModel.GetSymbolInfo(ProtoExpr);
            return symbolInfo.Symbol.ContainingNamespace.ToDisplayString();
        }

        public string GetServiceName()
        {
            var semanticModel = context.SemanticModel;
            var symbolInfo = semanticModel.GetSymbolInfo(ProtoExpr);
            var containingType = symbolInfo.Symbol.ContainingType;
            var className = containingType.Name;


            var serviceName = className.Substring(0, className.Length - 6);
            return serviceName;
        }

        public string GetServiceProxyName()
        {
            var serviceName = GetServiceName();
            return ServiceInfo.GetProxyClassName(serviceName, "");
        }

        public string GetServiceProtoMethodStrName()
        {
            var symbolInfo = context.SemanticModel.GetSymbolInfo(ProtoExpr);
            var protoName = symbolInfo.Symbol.Name;
            var methodSuffix = protoName.Substring(4);
            var methodName = $"Dispatch_{methodSuffix}";
            return methodName;
        }

        public string GetServiceProtoStrName()
        {
            var symbolInfo = context.SemanticModel.GetSymbolInfo(ProtoExpr);
            var protoName = symbolInfo.Symbol.Name;
            return protoName;
        }

        public List<ITypeSymbol> GetGenericTypeSymbolList()
        {
            var semanticModel = context.SemanticModel;
            MemberAccessExpressionSyntax memberAccessExpressionSyntax = InvocationExpressionSyntax.Expression as MemberAccessExpressionSyntax;

            var generic = memberAccessExpressionSyntax.Name as GenericNameSyntax;
            if (generic == null)
            {
                return null;
            }
            var symbolInfo = semanticModel.GetSymbolInfo(ProtoExpr);
            var list = generic.TypeArgumentList
                .Arguments
                .Select(x => (ITypeSymbol)semanticModel.GetSymbolInfo(x).Symbol)
                .ToList();
            if (this.HasFlag(InvocationType.Call))
            {
                if (list.Count == 1)
                {
                    return null;
                }
                return list.Skip(1).ToList();
            }
            return list;
        }
    }
}