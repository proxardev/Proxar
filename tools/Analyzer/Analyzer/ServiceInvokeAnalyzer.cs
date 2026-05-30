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
using System.Diagnostics;
using System.Linq;

namespace Analyzer
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class ServiceInvokeAnalyzer : DiagnosticAnalyzer
    {

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get
            {
                var builder = ImmutableArray.CreateBuilder<DiagnosticDescriptor>();
                builder.Add(Rules.ErrorRule_0001);
                builder.Add(Rules.ErrorRule_0002);
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
                ExecuteAnalyze(AnalyzeInvocation, context1);
            };

            context.RegisterSyntaxNodeAction(action, SyntaxKind.InvocationExpression);
        }

        private void ExecuteAnalyze(Action<SyntaxNodeAnalysisContext> action, SyntaxNodeAnalysisContext context)
        {
            try
            {
                action.Invoke(context);
            }
            catch (Exception e)
            {
                Debugger.Launch();
                context.AnalysisErrorOutput(Rules.ErrorRule_0999, e.StackTrace);
                context.AnalysisErrorOutput(Rules.ErrorRule_0999, e.Message + "12312 " + e.StackTrace);
                throw;
            }
        }


        private void AnalyzeInvocation(SyntaxNodeAnalysisContext context)
        {
            var invocation = (InvocationExpressionSyntax)context.Node;
            ServiceInvokeInfo serviceInvokeInfo = null;

            // 模式1: Service.Result<T>().Call(...)
            if (IsResultCallPattern(context, invocation, out serviceInvokeInfo))
            {
                ValidateParameters(context, serviceInvokeInfo);
            }

            // 模式2: Service.Send(...)
            if (IsSendPattern(context, invocation, out serviceInvokeInfo))
            {
                ValidateParameters(context, serviceInvokeInfo);
            }
        }

        private bool IsResultCallPattern(SyntaxNodeAnalysisContext context,
            InvocationExpressionSyntax invocation,
            out ServiceInvokeInfo serviceCallInfo)
        {
            serviceCallInfo = null;

            // 检查是否是 .Call() 调用
            if (!(invocation.Expression is MemberAccessExpressionSyntax memberAccess))
                return false;

            if (memberAccess.Name.Identifier.Text != "Call")
                return false;

            // 检查调用者是否是 .Result<T>() 调用
            if (!(memberAccess.Expression is InvocationExpressionSyntax resultInvocation))
                return false;

            if (!(resultInvocation.Expression is MemberAccessExpressionSyntax resultMemberAccess))
                return false;

            if (resultMemberAccess.Name.Identifier.Text != "Result")
                return false;

            var res = CommonValid(context, invocation, out serviceCallInfo);
            serviceCallInfo?.SetFlag(InvocationType.Call);
            return res;
        }

        private bool CommonValid(SyntaxNodeAnalysisContext context, InvocationExpressionSyntax invocation, out ServiceInvokeInfo serviceCallInfo)
        {
            serviceCallInfo = null;
            // 提取参数
            var arguments = invocation.ArgumentList.Arguments;
            if (arguments.Count < 2)
            {
                return false;
            }
            var protoExpr = arguments[1].Expression;
            var semanticModel = context.SemanticModel;
            if (!IsProtocolConstantAccess(protoExpr, semanticModel))
            {
                return false;
            }

            var argumentList = arguments
                .Skip(2)
                .Select(selector => selector.Expression)
                .ToList();

            serviceCallInfo = new ServiceInvokeInfo()
            {
                context = context,
                InvocationExpressionSyntax = invocation,
                ProtoExpr = protoExpr,
                ArgsExprList = argumentList,
            };

            return true;
        }

        private bool IsSendPattern(SyntaxNodeAnalysisContext context,
            InvocationExpressionSyntax invocation,
            out ServiceInvokeInfo serviceCallInfo)
        {
            serviceCallInfo = null;
            // 检查是否是 .Send 调用
            if (!(invocation.Expression is MemberAccessExpressionSyntax memberAccess))
                return false;

            if (memberAccess.Name.Identifier.Text != "Send")
                return false;

            // 检查调用者是否是 "Service"（静态类或实例）
            var caller = memberAccess.Expression;

            // 模式：Service.Send（静态类）
            if (caller is IdentifierNameSyntax id && id.Identifier.Text == "Service")
            {
                // 可选：通过语义模型确认是静态类（见下方）
            }
            //// 模式：_service.Send（实例字段）
            //else if (caller is IdentifierNameSyntax)
            //{
            //    // 实例调用，也接受
            //}
            //// 模式：this.Service.Send / other.Service.Send
            //else if (caller is MemberAccessExpressionSyntax)
            //{
            //    // 链式成员访问
            //}
            //else
            //{
            //    return false; // 不认识的调用模式
            //}

            return CommonValid(context, invocation, out serviceCallInfo);
        }

        private bool IsProtocolConstantAccess(
            ExpressionSyntax expression,
            SemanticModel semanticModel)
        {
            // 必须是成员访问表达式：Xxx_Proto.Pro_123_Xxx
            if (!(expression is MemberAccessExpressionSyntax memberAccess))
                return false;

            // 获取符号信息
            var symbolInfo = semanticModel.GetSymbolInfo(expression);

            // 必须是字段
            if (!(symbolInfo.Symbol is IFieldSymbol fieldSymbol))
                return false;

            // 必须是常量
            if (!fieldSymbol.IsConst)
                return false;

            // 必须是静态
            if (!fieldSymbol.IsStatic)
                return false;

            // 获取常量值作为协议号
            if (!(fieldSymbol.ConstantValue is int))
                return false;

            // 解析服务名：从所在类名提取
            var containingType = fieldSymbol.ContainingType;
            var className = containingType.Name;

            // 验证命名约定：必须以 _Proto 结尾
            if (!className.EndsWith("_Proto"))
                return false;
            return true;
        }

        private void ValidateParameters(SyntaxNodeAnalysisContext context, ServiceInvokeInfo serviceInvokeInfo)
        {
            var semanticModel = context.SemanticModel;
            context.DebugOutput($"debug info3 {serviceInvokeInfo.GetServiceMethod() == null} " +
                $"{serviceInvokeInfo.GetServiceName()} {serviceInvokeInfo.GetServiceProtoMethodStrName()}");

            var protoMethodParamsTypeList = serviceInvokeInfo.GetProtoMethodParametersTypeList();
            var callMethodParamTypeList = serviceInvokeInfo.GetCallMethodParametersTypeList();
            var method = serviceInvokeInfo.GetServiceMethod();
            var argsExprList = serviceInvokeInfo.ArgsExprList;
            if (protoMethodParamsTypeList == null)
            {
                Debugger.Launch();
            }
            context.DebugOutput($"debug info5 {serviceInvokeInfo.GetServiceMethod() == null} {callMethodParamTypeList?.Count} {callMethodParamTypeList == null}" +
                $"{serviceInvokeInfo.GetServiceName()} {serviceInvokeInfo.GetServiceProtoMethodStrName()}");


            var infos = callMethodParamTypeList.Select(t => t.ToDisplayString()).ToList();
            var info = string.Join(", ", infos);
            var infos2 = protoMethodParamsTypeList.Select(t => t.ToDisplayString()).ToList();
            var info2 = string.Join(", ", infos2);
            context.DebugOutput($"debug info5 {serviceInvokeInfo.GetServiceMethod() == null} " +
                $"{serviceInvokeInfo.GetServiceName()} {serviceInvokeInfo.GetServiceProtoMethodStrName()}");
            var x = serviceInvokeInfo.GetGenericTypeSymbolList();
            var info3 = "xxx";
            if (x != null)
            {

                var infos3 = x.Select(t => t.ToDisplayString()).ToList();
                info3 = string.Join(", ", infos3);
            }
            context.DebugOutput($"debug info {serviceInvokeInfo.GetServiceNamespace()} " +
                $"{serviceInvokeInfo.GetServiceName()} {method.ToDisplayString()} {protoMethodParamsTypeList.Count} {argsExprList.Count}");

            // 参数类型校验
            var compareArgsCount = Math.Min(protoMethodParamsTypeList.Count, argsExprList.Count);
            for (var i = 0; i < compareArgsCount; i++)
            {
                var paramType = protoMethodParamsTypeList[i];
                var argsExpr = argsExprList[i];
                var argType = callMethodParamTypeList[i];

                if (!argType.Equals(paramType, SymbolEqualityComparer.Default))
                {
                    context.ReportDiagnostic(Diagnostic.Create(
                        Rules.ErrorRule_0001,
                        argsExpr.GetLocation(),
                        paramType.ToDisplayString(),
                        argType.ToDisplayString(),
                        method.ToDisplayString()
                        ));
                }
            }

            // 参数数量校验
            if (protoMethodParamsTypeList.Count != argsExprList.Count)
            {
                var invocationExpression = serviceInvokeInfo.InvocationExpressionSyntax;
                context.ReportDiagnostic(Diagnostic.Create(
                        Rules.ErrorRule_0002,
                        invocationExpression.GetLocation(),
                        protoMethodParamsTypeList.Count,
                        argsExprList.Count,
                        method.ToDisplayString()
                        )
                    );
            }
            context.DebugOutput($"debug info end  {serviceInvokeInfo.GetServiceNamespace()} " +
                $"{serviceInvokeInfo.GetServiceName()} {method.ToDisplayString()}");

        }
    }
}