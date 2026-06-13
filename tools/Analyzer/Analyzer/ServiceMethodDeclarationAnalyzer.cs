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
using Microsoft.CodeAnalysis.Diagnostics;
using ServiceDispatchMethodGenerator;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Analyzer
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class ServiceMethodDeclarationAnalyzer : DiagnosticAnalyzer
    {

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get
            {
                var builder = ImmutableArray.CreateBuilder<DiagnosticDescriptor>();
                builder.Add(Rules.ErrorRule_0003);
                builder.Add(Rules.ErrorRule_0004);
                builder.Add(Rules.ErrorRule_0005);
                builder.Add(Rules.ErrorRule_0999);
                builder.Add(Rules.ErrorRule_1001);
                builder.Add(Rules.ErrorRule_1002);
                builder.Add(Rules.ErrorRule_1003);

                builder.Add(Rules.DebugRule);
                return builder.ToImmutable();
            }
        }


        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();

            Action<SyntaxNodeAnalysisContext> action = (context1) =>
            {
                ExecuteAnalyze(AnalyzeServiceMethodDeclaration, context1);
            };

            context.RegisterSyntaxNodeAction(action, SyntaxKind.MethodDeclaration);

            Action<SyntaxNodeAnalysisContext> action2 = (context1) =>
            {
                ExecuteAnalyze(AnalyzeExternalExportService, context1);
            };

            context.RegisterSyntaxNodeAction(action2, SyntaxKind.ClassDeclaration);
        }

        private void ExecuteAnalyze(Action<SyntaxNodeAnalysisContext> action, SyntaxNodeAnalysisContext context)
        {
            try
            {
                action.Invoke(context);
            }
            catch (Exception e)
            {
                context.AnalysisErrorOutput(Rules.ErrorRule_0999, e.Message + " stacktrace " + e.StackTrace);
                throw;
            }
        }


        private void AnalyzeServiceMethodDeclaration(SyntaxNodeAnalysisContext context)
        {
            var methodDeclarationSyntax = (MethodDeclarationSyntax)context.Node;
            var classDeclarationSyntax = methodDeclarationSyntax
                .Ancestors()
                .OfType<ClassDeclarationSyntax>()
                .FirstOrDefault();
            if (classDeclarationSyntax == null)
            {
                return;
            }
            var serviceInfo = new ServiceInfo(context, classDeclarationSyntax);
            if (!serviceInfo.IsService)
            {
                return;
            }
            var methodSymbol = context.SemanticModel.GetDeclaredSymbol(methodDeclarationSyntax);
            if (!serviceInfo.IsProtoMethod(methodSymbol))
            {
                return;
            }
            var proto = ServiceInfo.GetProto(methodSymbol);
            AnalyzeReservedProtocolUsage(context, methodDeclarationSyntax, proto);
            AnalyzeInvalidProtocolUsage(context, methodDeclarationSyntax, proto);
            AnalyzeRepeatProtocolUsage(context, serviceInfo, methodDeclarationSyntax, proto);
        }

        private void AnalyzeRepeatProtocolUsage(SyntaxNodeAnalysisContext context, ServiceInfo serviceInfo, MethodDeclarationSyntax methodDeclarationSyntax, int proto)
        {
            var sameProtoMethodSymbols = serviceInfo.GetProtoMethodSymbols(proto)
                .Where(x => x.Name != methodDeclarationSyntax.Identifier.Text)
                .ToList();
            if (sameProtoMethodSymbols.Count == 0)
            {
                return;
            }
            var otherLocation = sameProtoMethodSymbols
                .Select(m => m.DeclaringSyntaxReferences.First().GetSyntax().GetLocation())
                .ToList();
            var sameProtoMethodNames = sameProtoMethodSymbols
                .Select(x => x.Name)
                .ToList();
            var alertInfo = string.Join("\n", sameProtoMethodNames);
            context.ReportDiagnostic(Diagnostic.Create(
                Rules.ErrorRule_0003,
                methodDeclarationSyntax.GetLocation(),
                proto,
                alertInfo
                )
                );
        }

        private void AnalyzeReservedProtocolUsage(SyntaxNodeAnalysisContext context, MethodDeclarationSyntax methodDeclarationSyntax, int proto)
        {
            if (!ServiceInfo.IsReservedProto(proto))
            {
                return;
            }
            context.ReportDiagnostic(Diagnostic.Create(
                Rules.ErrorRule_0004,
                methodDeclarationSyntax.GetLocation(),
                proto,
                ServiceInfo.GetReservedProtoDescString()
                )
                );
        }

        private void AnalyzeInvalidProtocolUsage(SyntaxNodeAnalysisContext context, MethodDeclarationSyntax methodDeclarationSyntax, int proto)
        {
            if (!ServiceInfo.IsInvalidProto(proto))
            {
                return;
            }
            context.ReportDiagnostic(Diagnostic.Create(
                Rules.ErrorRule_0005,
                methodDeclarationSyntax.GetLocation(),
                proto,
                ServiceInfo.StartProto
                )
                );
        }


        private void AnalyzeExternalExportService(SyntaxNodeAnalysisContext context)
        {
            var classDeclarationSyntax = (ClassDeclarationSyntax)context.Node;
            if (classDeclarationSyntax == null)
            {
                return;
            }
            var serviceInfo = new ServiceInfo(context, classDeclarationSyntax);
            if (!serviceInfo.IsService)
            {
                return;
            }
            bool isExternalService = serviceInfo.ServiceClassSymbol.GetAttributes().Any(attr =>
                    attr.AttributeClass?.ToDisplayString() == ExternalProxyAttribute.AttributeName);
            if (!isExternalService)
            {
                return;
            }
            AnalyzeExternalProxyModuleId(context, serviceInfo);
            AnalyzeExternalProxyIdRepeat(context, serviceInfo);
        }


        private void AnalyzeExternalProxyModuleId(SyntaxNodeAnalysisContext context, ServiceInfo serviceInfo)
        {
            var options = context.Options.AnalyzerConfigOptionsProvider;
            options.GlobalOptions.TryGetValue($"build_property.{ExternalProxyAttribute.ModuleExternalProxyPreIdField}", out var idStr);
            int moduleId = int.TryParse(idStr, out var id) ? id : 0;
            var succ = serviceInfo.ServiceClassSymbol
                .TryGetSymbolAttrValue<long>(ExternalProxyAttribute.AttributeName, ExternalProxyAttribute.ProxyField, out var proxyId);
            if (!succ)
            {
                return;
            }

            if (moduleId <= 0)
            {
                var diagnostic = Diagnostic.Create(
                    Rules.ErrorRule_1001,
                    serviceInfo.ClassDeclarationSyntax.GetLocation(),
                    ExternalProxyAttribute.AttributeName,
                    ExternalProxyAttribute.ModuleExternalProxyPreIdField,
                    ExternalProxyAttribute.ModuleExternalProxyPreIdField,
                    ExternalProxyAttribute.ModuleExternalProxyPreIdField,
                    ExternalProxyAttribute.ModuleExternalProxyPreIdField
                    );
                context.ReportDiagnostic(diagnostic);
            }
            if (proxyId < ExternalProxyAttribute.MinProxyId || proxyId > ExternalProxyAttribute.MaxProxyId)
            {
                var diagnostic = Diagnostic.Create(
                    Rules.ErrorRule_1002,
                    serviceInfo.ClassDeclarationSyntax.GetLocation(),
                    ExternalProxyAttribute.AttributeName,
                    proxyId,
                    ExternalProxyAttribute.MinProxyId,
                    ExternalProxyAttribute.MaxProxyId);
                context.ReportDiagnostic(diagnostic);
            }
        }

        private void AnalyzeExternalProxyIdRepeat(SyntaxNodeAnalysisContext context, ServiceInfo serviceInfo)
        {
            var compilation = serviceInfo.SemanticModel.Compilation;

            // 获取 ServiceExportAttribute 的类型符号
            var externalProxyAttributeType = compilation.GetTypeByMetadataName(ExternalProxyAttribute.AttributeName);
            if (externalProxyAttributeType == null) return;

            // 收集所有带 [ServiceExport] 的类及其 ExportId
            var externalClasses = new List<(INamedTypeSymbol ClassSymbol, long ExportId)>();
            GatherExternalProxyClasses(compilation.GlobalNamespace, externalProxyAttributeType, externalClasses);

            // 从分析器配置中读取模块前缀 ID
            var options = context.Options.AnalyzerConfigOptionsProvider;
            options.GlobalOptions.TryGetValue("build_property.ModuleExternalProxyPreId", out var moduleIdStr);
            int moduleId = int.TryParse(moduleIdStr, out var id) ? id : 0;

            // 检查最终 ID 是否重复 (最终ID = moduleId * 10000 + exportId)
            var usedIds = new Dictionary<long, INamedTypeSymbol>();
            foreach (var (classSymbol, proxyId) in externalClasses)
            {

                if (usedIds.TryGetValue(proxyId, out var existingClass))
                {
                    // 报告重复 ID 错误
                    var diagnostic = Diagnostic.Create(
                        Rules.ErrorRule_1003,
                        classSymbol.DeclaringSyntaxReferences.FirstOrDefault().GetSyntax().GetLocation(),
                        ExternalProxyAttribute.AttributeName,
                        proxyId,
                        existingClass.ToDisplayString());
                    context.ReportDiagnostic(diagnostic);

                    var diagnostic2 = Diagnostic.Create(
                        Rules.ErrorRule_1003,
                        existingClass.DeclaringSyntaxReferences.FirstOrDefault().GetSyntax().GetLocation(),
                        ExternalProxyAttribute.AttributeName,
                        proxyId,
                        classSymbol.ToDisplayString());
                    context.ReportDiagnostic(diagnostic2);
                }
                else
                {
                    usedIds[proxyId] = classSymbol;
                }
            }
        }

        /// <summary>
        /// 递归遍历命名空间和类型树，收集所有标记了指定特性的非抽象类及其 ProxyId。
        /// </summary>
        private static void GatherExternalProxyClasses(
            INamespaceOrTypeSymbol container,
            INamedTypeSymbol exportAttrType,
            List<(INamedTypeSymbol ClassSymbol, long ProxyId)> result)
        {
            foreach (var member in container.GetMembers())
            {
                if (member is INamespaceSymbol ns)
                {
                    // 递归进入子命名空间
                    GatherExternalProxyClasses(ns, exportAttrType, result);
                }
                else if (member is INamedTypeSymbol type)
                {
                    // 检查该类是否直接标记了目标特性
                    var attr = type.GetAttributes().FirstOrDefault(a =>
                        a.AttributeClass?.Equals(exportAttrType, SymbolEqualityComparer.Default) == true);
                    var succ = type.TryGetSymbolAttrValue<long>(ExternalProxyAttribute.AttributeName, ExternalProxyAttribute.ProxyField, out var proxyId);
                    if (succ)
                    {
                        result.Add((type, proxyId));
                    }

                    // 递归处理嵌套类型
                    GatherExternalProxyClasses(type, exportAttrType, result);
                }
            }
        }

    }
}