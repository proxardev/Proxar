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
using System;
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
        }

        private void ExecuteAnalyze(Action<SyntaxNodeAnalysisContext> action, SyntaxNodeAnalysisContext context)
        {
            try
            {
                //context.DebugOutput("asdaaaaaaaaaa");
                //if (context.SemanticModel != null)
                //{
                //    throw new Exception("adsssssssssss");
                //}
                action.Invoke(context);
            }
            catch (Exception e)
            {
                context.AnalysisErrorOutput(Rules.ErrorRule_0999, e.Message + "1111 " + e.StackTrace);
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
            var serviceInfo = new ServiceInfo(context, methodDeclarationSyntax);
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
    }
}