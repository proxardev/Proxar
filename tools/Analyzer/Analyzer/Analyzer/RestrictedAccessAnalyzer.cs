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


using Analyzer;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;

namespace Proxar.Analyzers
{
    /// <summary>
    /// 注册为编译期分析器，识别对 [RestrictedAccess] 标记的入口的一切外部引用。
    /// 规则：只有「声明类型（含嵌套类型）内部」才允许引用，其余一律报 RS001。
    /// </summary>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class RestrictedAccessAnalyzer : DiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
            => ImmutableArray.Create(Rules.RestrictedAccessViolation);

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();

            // 覆盖各类「引用」语法节点
            context.RegisterSyntaxNodeAction(AnalyzeInvocation, SyntaxKind.InvocationExpression);
            context.RegisterSyntaxNodeAction(AnalyzeMemberAccess, SyntaxKind.SimpleMemberAccessExpression);
            context.RegisterSyntaxNodeAction(AnalyzeAssignment, SyntaxKind.SimpleAssignmentExpression);
            context.RegisterSyntaxNodeAction(AnalyzeEventSubscription, SyntaxKind.AddAssignmentExpression,
                                                                      SyntaxKind.SubtractAssignmentExpression);
            context.RegisterSyntaxNodeAction(AnalyzeObjectCreation, SyntaxKind.ObjectCreationExpression);
            context.RegisterSyntaxNodeAction(AnalyzeTypeReference, SyntaxKind.TypeOfExpression,
                                                                      SyntaxKind.Parameter,
                                                                      SyntaxKind.VariableDeclaration,
                                                                      SyntaxKind.ClassDeclaration,
                                                                      SyntaxKind.StructDeclaration,
                                                                      SyntaxKind.InterfaceDeclaration,
                                                                      SyntaxKind.GenericName);
        }

        #region 各类引用分析

        /// <summary>方法调用：svc.Send(...) / new Foo() 之外的普通调用。</summary>
        private static void AnalyzeInvocation(SyntaxNodeAnalysisContext ctx)
        {
            var invocation = (InvocationExpressionSyntax)ctx.Node;
            var symbol = ctx.SemanticModel.GetSymbolInfo(invocation).Symbol as IMethodSymbol;
            if (symbol == null) return;

            ReportIfRestricted(ctx, symbol, invocation);
        }

        /// <summary>成员访问作为表达式（读属性 / 读字段）。</summary>
        private static void AnalyzeMemberAccess(SyntaxNodeAnalysisContext ctx)
        {
            var access = (MemberAccessExpressionSyntax)ctx.Node;

            // 排除：这是调用表达式的「主体」（如 foo.Bar() 中 Bar 的访问），由 AnalyzeInvocation 处理
            if (access.Parent is InvocationExpressionSyntax) return;

            var symbol = ctx.SemanticModel.GetSymbolInfo(access).Symbol;
            if (symbol == null) return;

            ReportIfRestricted(ctx, symbol, access);
        }

        /// <summary>赋值左侧成员访问（写字段 / 写属性 set）。</summary>
        private static void AnalyzeAssignment(SyntaxNodeAnalysisContext ctx)
        {
            var assignment = (AssignmentExpressionSyntax)ctx.Node;
            if (assignment.Left is not MemberAccessExpressionSyntax leftAccess) return;

            var symbol = ctx.SemanticModel.GetSymbolInfo(leftAccess).Symbol;
            if (symbol == null) return;

            // 属性赋值 → 检查 set 方法；字段赋值 → 检查字段
            var target = symbol is IPropertySymbol prop ? prop.SetMethod : symbol;
            if (target == null) return; // 只读属性无 set，但赋值本身会报错，这里不重复拦截

            ReportIfRestricted(ctx, target, leftAccess);
        }

        /// <summary>事件订阅 / 取消订阅：+= / -= 。</summary>
        private static void AnalyzeEventSubscription(SyntaxNodeAnalysisContext ctx)
        {
            var assignment = (AssignmentExpressionSyntax)ctx.Node;
            if (assignment.Left is not MemberAccessExpressionSyntax leftAccess) return;

            var symbol = ctx.SemanticModel.GetSymbolInfo(leftAccess).Symbol as IEventSymbol;
            if (symbol == null) return;

            ReportIfRestricted(ctx, symbol, leftAccess);
        }

        /// <summary>对象构造：new T(...)。</summary>
        private static void AnalyzeObjectCreation(SyntaxNodeAnalysisContext ctx)
        {
            var creation = (ObjectCreationExpressionSyntax)ctx.Node;
            var symbol = ctx.SemanticModel.GetSymbolInfo(creation).Symbol as IMethodSymbol; // 构造器
            if (symbol == null) return;

            ReportIfRestricted(ctx, symbol, creation);
        }

        /// <summary>类型引用：typeof(T) / 参数类型 / 变量声明 / 基类 / 泛型参数等。</summary>
        private static void AnalyzeTypeReference(SyntaxNodeAnalysisContext ctx)
        {
            INamedTypeSymbol typeSymbol = null;

            switch (ctx.Node)
            {
                case TypeOfExpressionSyntax typeOf:
                    {
                        var info = ctx.SemanticModel.GetTypeInfo(typeOf.Type).Type as INamedTypeSymbol;
                        typeSymbol = info;
                        break;
                    }
                case ParameterSyntax param:
                    {
                        var paramSymbol = ctx.SemanticModel.GetDeclaredSymbol(param) as IParameterSymbol;
                        typeSymbol = paramSymbol?.Type as INamedTypeSymbol;
                        break;
                    }
                case VariableDeclarationSyntax varDecl:
                    {
                        typeSymbol = ctx.SemanticModel.GetTypeInfo(varDecl.Type).Type as INamedTypeSymbol;
                        break;
                    }
                case ClassDeclarationSyntax cls:
                case StructDeclarationSyntax st:
                case InterfaceDeclarationSyntax iface:
                    {
                        // 基类 / 接口列表中的类型引用
                        var typeDecl = (BaseTypeDeclarationSyntax)ctx.Node;
                        foreach (var baseType in typeDecl.BaseList?.Types ?? default)
                        {
                            if (baseType == null) continue;
                            var t = ctx.SemanticModel.GetTypeInfo(baseType.Type).Type as INamedTypeSymbol;
                            if (t != null && SymbolRestrictionChecker.IsRestricted(t))
                            {
                                var caller = GetContainingType(ctx.SemanticModel, typeDecl);
                                if (!SymbolRestrictionChecker.IsAllowed(caller, t))
                                {
                                    ctx.ReportDiagnostic(Diagnostic.Create(
                                        Rules.RestrictedAccessViolation,
                                        baseType.Type.GetLocation(),
                                        SymbolRestrictionChecker.FormatSymbol(t),
                                        caller?.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat) ?? "<global>"));
                                }
                            }
                        }
                        return;
                    }
                case GenericNameSyntax generic:
                    {
                        var info = ctx.SemanticModel.GetTypeInfo(generic).Type as INamedTypeSymbol;
                        typeSymbol = info;
                        break;
                    }
            }

            if (typeSymbol == null) return;
            if (!SymbolRestrictionChecker.IsRestricted(typeSymbol)) return;

            var callerType = GetContainingType(ctx.SemanticModel, ctx.Node);
            if (!SymbolRestrictionChecker.IsAllowed(callerType, typeSymbol))
            {
                ctx.ReportDiagnostic(Diagnostic.Create(
                    Rules.RestrictedAccessViolation,
                    ctx.Node.GetLocation(),
                    SymbolRestrictionChecker.FormatSymbol(typeSymbol),
                    callerType?.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat) ?? "<global>"));
            }
        }

        #endregion

        /// <summary>若符号受限制，且调用方不在允许范围内，则报告诊断。</summary>
        private static void ReportIfRestricted(SyntaxNodeAnalysisContext ctx, ISymbol symbol, SyntaxNode node)
        {
            if (!SymbolRestrictionChecker.IsRestricted(symbol)) return;

            var callerType = GetContainingType(ctx.SemanticModel, node);
            if (SymbolRestrictionChecker.IsAllowed(callerType, symbol)) return; // 内部调用，放行

            ctx.ReportDiagnostic(Diagnostic.Create(
                Rules.RestrictedAccessViolation,
                node.GetLocation(),
                SymbolRestrictionChecker.FormatSymbol(symbol),
                callerType?.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat) ?? "<global>"));
        }

        /// <summary>从语法节点向上找到「包含它的类型声明」，再取其符号。</summary>
        private static INamedTypeSymbol GetContainingType(SemanticModel semanticModel, SyntaxNode node)
        {
            // 局部函数 / lambda 不影响「调用方类型」判定：继续向上找真正的类型声明
            var typeDecl = node.FirstAncestorOrSelf<TypeDeclarationSyntax>();
            if (typeDecl != null)
            {
                return semanticModel.GetDeclaredSymbol(typeDecl) as INamedTypeSymbol;
            }
            return null;
        }
    }
}
