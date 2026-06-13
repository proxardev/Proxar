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
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using ServiceDispatchMethodGenerator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

[Generator(LanguageNames.CSharp)]
public class ServiceDispatchMessageMethodGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var rpcCallBackFunc = new ServiceRpcCallBackType_Generator().Initialize2(context);
        var serviceInfos = context.SyntaxProvider
            .CreateSyntaxProvider(
                predicate: (syntaxNode, cancellationToken) => IsCandidateMethod(syntaxNode),
                transform: (generatorSyntaxContext, cancellationToken) => GetMethodSymbol(generatorSyntaxContext))
            .Where(methodSymbol => methodSymbol is not null);
        var optionsProvider = context.AnalyzerConfigOptionsProvider;

        // 提取 ServiceModuleId
        var moduleIdProvider = optionsProvider
            .Select((options, ct) =>
            {
                // 尝试读取属性，失败时返回默认值 0
                options.GlobalOptions.TryGetValue("build_property.ModuleExternalProxyPreId", out var moduleIdStr);
                //return 111;
                return int.TryParse(moduleIdStr, out var moduleId) ? moduleId : 0;
            });

        var collected = serviceInfos.Collect();
        var combined = collected.Combine(rpcCallBackFunc).Combine(moduleIdProvider);



        context.RegisterSourceOutput(combined, (productionContext, info) =>
        {
            var ((serviceInfos, rpcInfo), ModuleExternalProxyPreId) = info;
            var uniqueServices = serviceInfos
                .GroupBy(info => info.GetFullClassName())  // 按类名分组
                .Select(g => g.First())                // 每组只留第一个
                .ToList();
            foreach (var uniqueService in uniqueServices)
            {

                var sourceCode = GenerateServiceDispatchPartialClass(uniqueService, "");
                var fileName = $"{uniqueService.GetNamespace()}.{uniqueService.GetClassName().Replace('<', '_').Replace('>', '_')}_DispatchTable.g.cs";
                productionContext.AddSource(fileName, SourceText.From(sourceCode, Encoding.UTF8));

                ServiceProxyClassGenerate.GenerateProxyClass(ModuleExternalProxyPreId, productionContext, uniqueService);
            }
        });
    }

    // 判断语法节点是否是候选方法
    private static bool IsCandidateMethod(SyntaxNode syntaxNode)
    {
        if (syntaxNode is not ClassDeclarationSyntax classDeclaration)
        {
            return false;
        }
        return true;
    }

    // 获取方法符号信息
    private static ServiceInfo GetMethodSymbol(GeneratorSyntaxContext context)
    {
        var classDeclaration = (ClassDeclarationSyntax)context.Node;
        ServiceInfo serviceInfo = new ServiceInfo(context.SemanticModel, classDeclaration);
        if (!serviceInfo.IsService)
        {
            return null;
        }
        return serviceInfo;
    }

    private static string GenerateSwithCaseLogic(ServiceInfo serviceInfo, IMethodSymbol methodSymbol)
    {
        var constProtoClassName = serviceInfo.GetConstProtoClassName();
        var isAsync = methodSymbol.IsAsync;

        var methodName = methodSymbol.Name;

        var isRequestCallBack = methodSymbol.IsRequestCallBackMethod();

        var (protoName, protoName2, proto) = serviceInfo.ParseMethodNameProtoName(methodSymbol);
        var argsTypes = methodSymbol.Parameters
                .Select(x => ParameterTypeResolver.GetFullTypeNameWithoutGlobal(x.Type, serviceInfo.SemanticModel))
                .ToList();
        var argsType = string.Join(", ", argsTypes);
        var isReturnTaskGenericType = isAsync
                && methodSymbol.ReturnType is INamedTypeSymbol namedType
                && namedType.IsGenericType;
        var isSyncReturnType = !isAsync && !methodSymbol.ReturnsVoid;
        var argsCnt = argsTypes.Count;

        var preLogic = "";
        var afterLogic = "";
        var continueSign = "";
        var resultInfo = "";
        if (isReturnTaskGenericType)
        {
            if (isRequestCallBack)
            {
                continueSign = ".Coroutine()";
            }
            else
            {
                preLogic = $@"
                    var responseServiceId = GetResponseServiceId();
                    var responseMsgIdx = GetResponseMsgIdx();
";
                continueSign = ".AutoResponse(responseServiceId, responseMsgIdx).Coroutine()";
            }
        }
        else if (isAsync)
        {
            continueSign = ".Coroutine()";
        }
        else if (isSyncReturnType)
        {
            preLogic = $@"
                    var responseServiceId = GetResponseServiceId();
                    var responseMsgIdx = GetResponseMsgIdx();
";
            resultInfo = $@"var result = ";
            afterLogic = $@"
                    var header = new MessageHeader(responseMsgIdx);
                    Service.Send(responseServiceId, ProtoBase.RpcCallBack, result, header: header);
";
        }
        var info = string.Empty;
        if (argsCnt == 0)
        {
            info = $@"
            case {constProtoClassName}.{protoName}:
                {{
{preLogic}
                    {resultInfo}this.{methodName}(){continueSign};
{afterLogic}
                }}
                break;";
        }
        else if (argsCnt == 1)
        {
            info = $@"
            case {constProtoClassName}.{protoName}:
                {{
{preLogic}
                    var args = msg.DeserializeArgs<{argsType}>();
                    {resultInfo}this.{methodName}(args){continueSign};
{afterLogic}
                }}
                break;";
        }
        else
        {
            var argsNameList = new List<string>();
            for (int i = 0; i < argsCnt; i++)
            {
                argsNameList.Add($"args{i + 1}");
            }
            var argsNameStr = string.Join(", ", argsNameList);
            info = $@"
            case {constProtoClassName}.{protoName}:
                {{
{preLogic}
                    var ({argsNameStr}) = msg.DeserializeArgs<{argsType}>();
                    {resultInfo}this.{methodName}({argsNameStr}){continueSign};
{afterLogic}
                }}
                break;";
        }
        return info;
    }

    // 生成分部类代码
    private static string GenerateServiceDispatchPartialClass(ServiceInfo serviceInfo, string rpcCallBackFunc)
    {
        var namespaceName = serviceInfo.GetNamespace();
        var className = serviceInfo.GetClassName();
        var accessibility = serviceInfo.GetAccessibilityType();

        var protoDefineClassName = serviceInfo.GetConstProtoClassName();
        var proto2MethodNameDict = new Dictionary<int, string>();
        var switchList = new List<string>();
        foreach (var methodSymbol in serviceInfo.GetProtoMethodSymbols())
        {
            var methodName = methodSymbol.Name;
            var (protoName, protoName2, proto) = serviceInfo.ParseMethodNameProtoName(methodSymbol);
            if (proto2MethodNameDict.ContainsKey(proto))
            {
                throw new Exception($"{className} {proto} proto repeat use by {methodName} and {proto2MethodNameDict[proto]}");
            }
            proto2MethodNameDict[proto] = methodName;
            var info = GenerateSwithCaseLogic(serviceInfo, methodSymbol);
            if (info == string.Empty)
            {
                throw new Exception($"{className} {proto} not generate case logic");
            }
            switchList.Add(info);
        }

        var switchInfo = string.Join("\n", switchList);
        var protoDefinesInfo = MakeProtoConstDefineClass(serviceInfo);
        var usingInfo = serviceInfo.GetUsingString();
        var defaultHandle = "base.Dispatch(proto, msg);";
        var fff = "override";
        if (serviceInfo.IsServiceBaseClas())
        {
            defaultHandle = "";
            fff = "virtual";
        }
        var code = $@"
using System;
using MessagePack;
using System.Collections.Generic;
using Proxar.ServiceCore;
using Proxar.ServiceCore.Interfaces;
using Proxar.ServiceCore.Message;


{(string.IsNullOrEmpty(namespaceName) ? "" : $"namespace {namespaceName};")}


{accessibility} partial class {className}
{{

    protected {fff} void Dispatch(int proto, IServiceMessage msg)
    {{
        switch (proto)
        {{
{switchInfo}
            default:
                {defaultHandle}
                break;
        }}
    }}

{rpcCallBackFunc}

}}


{protoDefinesInfo}


";

        return code;
    }

    public static string MakeProtoConstDefineClass(ServiceInfo serviceInfo)
    {

        var protoMap = new Dictionary<string, int>();
        var enumProtoMap = new Dictionary<string, int>();
        foreach (var methodSymbol in serviceInfo.GetProtoMethodSymbols())
        {
            var (protoName, protoName2, proto) = serviceInfo.ParseMethodNameProtoName(methodSymbol);
            var accessibility = methodSymbol.GenerateServiceProtoMethodAccessibility();
            protoMap[$"\t{accessibility.ToString().ToLower()} const int {protoName}"] = proto;
            protoMap[$"\t{accessibility.ToString().ToLower()} const int {protoName2}"] = proto;
            enumProtoMap[$"\t{protoName2}"] = proto;
        }
        var protos = protoMap
            .OrderBy(x => x.Value)
            .Select(x => $"{x.Key} = {x.Value};")
            .ToList();
        var enumProtos = enumProtoMap
            .OrderBy(x => x.Value)
            .Select(x => $"{x.Key} = {x.Value},")
            .ToList();
        var constProto = string.Join("\n", protos);
        var enumProto = string.Join("\n", enumProtos);

        var protoClassInfo = $@"

public static class {serviceInfo.GetConstProtoClassName()}
{{
{constProto}
}};

public enum {serviceInfo.GetEnumProtoClassName()}
{{
{enumProto}
}}
    ";
        return protoClassInfo;
    }


    public sealed class ServiceMethodInfo
    {
        public INamedTypeSymbol ClassSymbol;
        public List<(IMethodSymbol, bool)> MethodDeclarations;
        public bool IsTaskFunc = false;
        public List<string> Usings;
        public SemanticModel SemanticModel;

        public ServiceMethodInfo(INamedTypeSymbol classSymbol,
            List<(IMethodSymbol, bool)> methodDeclarations, bool isTaskFunc, List<string> usingList, SemanticModel SemanticModel)
        {
            ClassSymbol = classSymbol;
            MethodDeclarations = methodDeclarations;
            IsTaskFunc = isTaskFunc;
            Usings = usingList;
            this.SemanticModel = SemanticModel;
        }
    }

}