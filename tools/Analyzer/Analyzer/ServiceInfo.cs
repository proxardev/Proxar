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

    public class ServiceInfo
    {
        public static (int, int) ReservedProto = (900, 999);
        public static int StartProto = 1;
        public const string BaseServiceClassFullName = "Proxar.ServiceCore.ServiceBase";
        public INamedTypeSymbol ServiceClassSymbol { get; set; }
        public SemanticModel SemanticModel { get; }
        public ClassDeclarationSyntax ClassDeclarationSyntax { get; }

        public bool IsService
        {
            get
            {
                return ServiceClassSymbol != null;
            }
        }


        public ServiceInfo(SyntaxNodeAnalysisContext context, MethodDeclarationSyntax method)
        {
            SemanticModel = context.SemanticModel;
            var methodDeclarationSyntax = (MethodDeclarationSyntax)context.Node;
            var classDeclarationSyntax = methodDeclarationSyntax
                .Ancestors()
                .OfType<ClassDeclarationSyntax>()
                .FirstOrDefault();
            this.TryInit(classDeclarationSyntax);
            if (!IsService)
            {
                return;
            }
            ClassDeclarationSyntax = classDeclarationSyntax;
        }

        public ServiceInfo(SemanticModel semanticModel, ClassDeclarationSyntax classDeclarationSyntax)
        {
            SemanticModel = semanticModel;
            this.TryInit(classDeclarationSyntax);
            if (!IsService)
            {
                return;
            }
            ClassDeclarationSyntax = classDeclarationSyntax;
        }

        private void TryInit(ClassDeclarationSyntax classDeclarationSyntax)
        {
            var serviceSymbol = SemanticModel.GetDeclaredSymbol(classDeclarationSyntax) as INamedTypeSymbol;
            if (serviceSymbol == null)
            {
                return;
            }
            if (!IsServiceClass(serviceSymbol, BaseServiceClassFullName))
            {
                return;
            }
            ServiceClassSymbol = serviceSymbol;
        }


        public string GetClassName()
        {
            return ClassDeclarationSyntax.Identifier.Text;
        }


        public string GetFullClassName()
        {
            return ServiceClassSymbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
        }

        public List<string> GetUsings()
        {
            var root = ClassDeclarationSyntax.SyntaxTree.GetRoot();
            var usings = root.DescendantNodes()
                .OfType<UsingDirectiveSyntax>()
                .Select(u => u.ToString())
                .ToList();
            return usings;
        }

        public string GetUsingString()
        {
            var usings = GetUsings();
            return string.Join("\n", usings);
        }

        public string GetNamespace()
        {
            return ServiceClassSymbol.ContainingNamespace?.ToDisplayString();
        }

        public string GetAccessibilityType()
        {
            var accessibility = ServiceClassSymbol.DeclaredAccessibility.ToString().ToLower();
            return accessibility;
        }

        public string GetConstProtoClassName()
        {
            return $"{ServiceClassSymbol.Name}_Proto";
        }

        public string GetEnumProtoClassName()
        {
            return $"{ServiceClassSymbol.Name}_Proto_Enum";
        }

        public string GetDirectParentProxyClassName()
        {
            return GetProxyClassName(ServiceClassSymbol.BaseType.Name);
        }

        public string GetProxyClassName()
        {
            return GetProxyClassName(ServiceClassSymbol.Name);
        }

        public static string GetProxyClassName(string className)
        {
            return $"{className}Proxy";
        }

        public string GetInterfaceName()
        {
            return $"I{GetClassName()}";
        }

        public static string GetInterfaceName(string className)
        {
            return $"I{className}";
        }

        public bool IsServiceBaseClas()
        {
            return ServiceClassSymbol.ToDisplayString() == BaseServiceClassFullName;
        }



        public static bool IsServiceClass(INamedTypeSymbol typeSymbol, string fullNamespaceName)
        {
            var baseType = typeSymbol;
            while (baseType != null)
            {
                if (baseType.ToDisplayString() == fullNamespaceName)
                    return true;
                baseType = baseType.BaseType;
            }
            return false;
        }

        public static bool IsReservedProto(int proto)
        {

            return ReservedProto.Item1 <= proto && proto <= ReservedProto.Item2;
        }

        public static bool IsReservedProto(IMethodSymbol methodSymbol)
        {
            var proto = GetProto(methodSymbol);
            return IsReservedProto(proto);
        }

        public static string GetReservedProtoDescString()
        {
            return ReservedProto.ToString();
        }

        public static bool IsInvalidProto(int proto)
        {
            return proto < StartProto;
        }

        private static string GetDispatchFuncPrefix()
        {
            return "Dispatch_";
        }

        public bool IsProtoMethod(IMethodSymbol methodSymbol)
        {
            var proto = -1;
            var name = methodSymbol.Name;
            var succ = TryGetProtoByAttribute(methodSymbol, out proto);
            if (succ)
            {
                return true;
            }
            succ = TryGetProtoByMethodName(name, out proto);
            if (succ)
            {
                return true;
            }
            return false;
        }

        public static int GetProto(IMethodSymbol methodSymbol)
        {
            var proto = -1;
            var name = methodSymbol.Name;
            var succ = TryGetProtoByAttribute(methodSymbol, out proto);
            if (succ)
            {
                return proto;
            }
            succ = TryGetProtoByMethodName(name, out proto);
            if (succ)
            {
                return proto;
            }
            return -1;
        }

        public static bool TryGetProtoByAttribute(IMethodSymbol methodSymbol, out int proto)
        {
            proto = -1;
            var attr = methodSymbol.GetAttributes()
                .Where(a => a.AttributeClass?.Name == "ServiceMethodAttribute")
                .FirstOrDefault();
            if (attr == null)
            {
                return false;
            }
            if (attr.ConstructorArguments.Length == 0)
            {
                throw new InvalidOperationException($"{methodSymbol.Name} ServiceMethodAttribute 未提供参数");
            }
            proto = (int)attr.ConstructorArguments[0].Value;
            return true;
        }

        public static bool TryGetProtoByMethodName(string methodName, out int proto)
        {
            proto = -1;
            if (!methodName.StartsWith(GetDispatchFuncPrefix()))
            {
                return false;
            }
            var span = methodName.AsSpan();

            span = span.Slice(GetDispatchFuncPrefix().Length);
            int underscoreIndex = span.IndexOf('_');
            if (underscoreIndex == -1)
            {
                return false;
            }

            var numberSpan = span.Slice(0, underscoreIndex);
            var parseSucc = int.TryParse(numberSpan.ToString(), out proto);
            if (!parseSucc)
            {
                return false;
            }
            return true;
        }


        public List<IMethodSymbol> GetProtoMethodSymbols(int proto)
        {
            var methods = ServiceClassSymbol
                .GetMembers()
                .OfType<IMethodSymbol>()
                .Where(method => GetProto(method) == proto)
                .ToList();
            return methods;
        }


        public List<IMethodSymbol> GetProtoMethodSymbols()
        {
            var methods = ServiceClassSymbol
                .GetMembers()
                .OfType<IMethodSymbol>()
                .Where(method => IsProtoMethod(method))
                .ToList();
            return methods;
        }

        public (string, string, int) ParseMethodNameProtoName(IMethodSymbol methodSymbol)
        {
            var succ = TryGetProtoByAttribute(methodSymbol, out var proto1);
            if (succ)
            {
                return (methodSymbol.Name, methodSymbol.Name, proto1);
            }
            var x = ParseMethodNameProtoName(methodSymbol.Name);
            var x2 = ParseMethodNameProtoName2(methodSymbol.Name);
            return (x.Item1, x2.Item1, x2.Item2);
        }

        private static (string, int) ParseMethodNameProtoName(string methodName)
        {
            var span = methodName.AsSpan();

            // 跳过 "Dispatch_"
            span = span.Slice(GetDispatchFuncPrefix().Length);
            int underscoreIndex = span.IndexOf('_');

            var numberSpan = span.Slice(0, underscoreIndex);
            var number = int.Parse(numberSpan.ToString());

            var descriptionSpan = span.Slice(underscoreIndex + 1);
            var proto = $"{descriptionSpan.ToString()}_{number}";
            return (proto, number);
        }

        private static (string, int) ParseMethodNameProtoName2(string methodName)
        {
            var span = methodName.AsSpan();

            // 跳过 "Dispatch_"
            span = span.Slice(GetDispatchFuncPrefix().Length);
            int underscoreIndex = span.IndexOf('_');

            var numberSpan = span.Slice(0, underscoreIndex);
            var number = int.Parse(numberSpan.ToString());

            var descriptionSpan = span.Slice(underscoreIndex + 1);
            var proto = $"Pro_{number}_{descriptionSpan.ToString()}";
            return (proto, number);
        }

    }
}