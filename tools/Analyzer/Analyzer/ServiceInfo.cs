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
using Proxar.Abstractions.Service;
using Proxar.Abstractions.Service.Interfaces;
using Proxar.ServiceCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Analyzer
{

    public class ServiceInfo
    {
        public static (int, int) ReservedProto = (ServiceConst.MinReservedProto, ServiceConst.MaxReservedProto);
        public static string ServiceMethodAttributeName => typeof(ServiceMethodAttribute).FullName;
        public static string ServiceExportAttribute => typeof(ServiceExportAttribute).FullName;
        public static string ModuleExternalProxyPreIdField = "ModuleExternalProxyPreId";
        public static string ExternalProxyIdField = "proxyId";
        public static string ServiceMethodExportAttribute => typeof(ServiceMethodExportAttribute).FullName;
        public static string ServiceMethodActionAttribute => typeof(ServiceMethodActionAttribute).FullName;
        public static string KnownServiceAttribute => typeof(KnownServiceAttribute).FullName;
        public static string KnownServiceIdField => "sequence";
        public static string IServiceFullName => typeof(IService).FullName;
        public static string IServiceProxy => "IServiceProxy";
        public static string IExternalProxy => "IExternalProxy";

        public INamedTypeSymbol ServiceClassSymbol { get; set; }
        public SemanticModel SemanticModel { get; }
        public Compilation Compilation { get; }
        public ClassDeclarationSyntax ClassDeclarationSyntax { get; }

        public bool IsService
        {
            get
            {
                return ServiceClassSymbol != null;
            }
        }


        public ServiceInfo(SyntaxNodeAnalysisContext context, ClassDeclarationSyntax classDeclarationSyntax)
        {
            SemanticModel = context.SemanticModel;
            Compilation = context.Compilation;
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
            Compilation = semanticModel.Compilation;
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
            if (!IsServiceClass(serviceSymbol))
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
            return ServiceClassSymbol.GetDefaultFullName();
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

        public string GetDirectParentProxyClassName(string mark)
        {
            return GetProxyClassName(ServiceClassSymbol.BaseType.Name, mark);
        }

        public string GetProxyClassName(string mark)
        {
            return GetProxyClassName(ServiceClassSymbol.Name, mark);
        }

        public static string GetProxyClassName(string className, string mark)
        {
            if (mark.Count() != 0)
            {
                return $"{className}_{mark}Proxy";
            }
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

        public bool IsServiceBaseClass()
        {
            var valid = ServiceClassSymbol.Interfaces.Any(x => x.ToDisplayString() == IServiceFullName);
            return valid;
        }



        public bool IsServiceClass(INamedTypeSymbol typeSymbol)
        {
            var valid = typeSymbol.AllInterfaces.Any(x => x.ToDisplayString() == IServiceFullName);
            return valid;
        }

        public static bool IsReservedProto(int proto)
        {

            return ReservedProto.Item1 <= proto && proto <= ReservedProto.Item2;
        }

        public static bool IsReservedProto(IMethodSymbol methodSymbol)
        {
            return TryGetProto(methodSymbol, out var _);
        }

        public static string GetReservedProtoDescString()
        {
            return ReservedProto.ToString();
        }

        public static bool IsInvalidProto(int proto)
        {
            return proto < ServiceConst.StartProto;
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

        public static bool TryGetProto(IMethodSymbol methodSymbol, out int proto)
        {
            proto = -1;
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

        public static bool IsAppointProto(IMethodSymbol methodSymbol)
        {
            var attr = methodSymbol.GetAttributes()
                .Where(a => a.AttributeClass.ToDisplayString() == ServiceMethodAttributeName)
                .FirstOrDefault();
            if (attr == null)
            {
                return false;
            }
            if (attr.ConstructorArguments.Length != 0)
            {
                return true;
            }
            return false;
        }

        public static bool TryGetProtoByAttribute(IMethodSymbol methodSymbol, out int proto)
        {
            proto = -1;
            var attr = methodSymbol.GetAttributes()
                .Where(a => a.AttributeClass.ToDisplayString() == ServiceMethodAttributeName)
                .FirstOrDefault();
            if (attr == null)
            {
                return false;
            }
            if (attr.ConstructorArguments.Length == 0)
            {
                var methodName = methodSymbol.Name;
                proto = Fnv1a32.HashToRange(methodName);
                return true;
            }
            var arg = attr.ConstructorArguments[0];
            proto = (int)arg.Value;
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
                .Where(method => { var get = TryGetProto(method, out var x); return get && x == proto; })
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

        public int GetOriginalProxyId()
        {
            TryGetOriginalProxyId(ServiceClassSymbol, out var proxyId);
            return proxyId;
        }


        public int GetProxyId(int moduleExternalProxyPreId)
        {
            var succ = TryGetOriginalProxyId(ServiceClassSymbol, out var proxyId);
            if (!succ)
            {
                return 0;
            }
            return moduleExternalProxyPreId * 10000 + proxyId;
        }

        public static bool TryGetOriginalProxyId(INamedTypeSymbol namedTypeSymbol, out int id)
        {
            id = 0;
            if (!namedTypeSymbol.IsDefineAttrValue(ServiceInfo.ServiceExportAttribute))
            {
                return false;
            }
            var succ = namedTypeSymbol.TryGetSymbolAttrValue<int>(ServiceInfo.ServiceExportAttribute, ServiceInfo.ExternalProxyIdField, out id);
            if (!succ)
            {
                id = Fnv1a32.HashToRange(namedTypeSymbol.GetDefaultFullName(), ServiceConst.AutoProxyIdStart, ServiceConst.AutoProxyIdEnd);
            }

            return true;
        }

        public static bool IsAppointProxyId(INamedTypeSymbol namedTypeSymbol)
        {
            if (!namedTypeSymbol.IsDefineAttrValue(ServiceInfo.ServiceExportAttribute))
            {
                return false;
            }
            var attr = namedTypeSymbol.GetAttributes()
                .Where(a => a.AttributeClass.ToDisplayString() == ServiceExportAttribute)
                .FirstOrDefault();
            if (attr == null)
            {
                return false;
            }
            if (attr.ConstructorArguments.Length != 0)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 从 ServiceInfo 中提取 [KnownService] 特性的 Sequence 值
        /// </summary>
        public static bool TryGetKnownServiceSequence(INamedTypeSymbol namedTypeSymbol, out int sequenceId)
        {
            var classSymbol = namedTypeSymbol;
            sequenceId = 0;
            if (!namedTypeSymbol.IsDefineAttrValue(ServiceInfo.KnownServiceAttribute))
            {
                return false;
            }
            var succ = namedTypeSymbol.TryGetSymbolAttrValue<int>(ServiceInfo.KnownServiceAttribute, ServiceInfo.KnownServiceIdField, out sequenceId);
            if (succ)
            {
                return true;
            }
            sequenceId = Fnv1a32.HashToRange(namedTypeSymbol.GetDefaultFullName(), ServiceConst.AutoKnownServiceIdStart, ServiceConst.AutoKnownServiceIdEnd);

            return true;
        }

        public static bool IsAppointKnownServiceId(INamedTypeSymbol namedTypeSymbol)
        {
            if (!namedTypeSymbol.IsDefineAttrValue(ServiceInfo.KnownServiceAttribute))
            {
                return false;
            }
            var attr = namedTypeSymbol.GetAttributes()
                .Where(a => a.AttributeClass.ToDisplayString() == KnownServiceAttribute)
                .FirstOrDefault();
            if (attr == null)
            {
                return false;
            }
            if (attr.ConstructorArguments.Length != 0)
            {
                return true;
            }
            return false;
        }

    }
}