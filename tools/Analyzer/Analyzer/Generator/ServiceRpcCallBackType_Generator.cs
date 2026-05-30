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
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading;

//[Generator(LanguageNames.CSharp)]
public class ServiceRpcCallBackType_Generator
{
    public IncrementalValueProvider<string> Initialize2(IncrementalGeneratorInitializationContext context)
    {
        // 扫描 Call<TRet> 方法调用
        var typeList = new List<string>();
        var callInvocations = context.SyntaxProvider
            .CreateSyntaxProvider(
                predicate: (node, _) => IsCallMethodInvocation(node),
                transform: (ctx, cancellationToken) =>
                {
                    var type = GetCallType(ctx, cancellationToken);
                    typeList.Add(type);
                    return type;
                }
                )
            .Where(type => type != null)

            .Collect()
            .Select((x, _) => GenerateServiceBaseDispatchFunc2(x))
            ;

        return callInvocations;

    }

    private string GenerateServiceBaseDispatchFunc2(ImmutableArray<string> typeNames)
    {
        var typeList = typeNames.Where(t => !string.IsNullOrEmpty(t)).Cast<string>().Distinct().ToList();

        var switchHandleList = typeList
            .Select(x => $"\t\t\t\"{x}\" => GetTable<{x}>().TrySet(msgSeq, msg)")
            .ToList();
        switchHandleList.Add("\t\t\t_ => false");
        var switchHandleFunc = string.Join(",\n", switchHandleList);
        var switchHandleList2 = typeList
            .Select(x => $"\t\t\t\"{x}\" => GetTable<{x}>().TrySetException(msgSeq, exception)")
            .ToList();
        switchHandleList2.Add("\t\t\t_ => false");
        var switchHandleFunc2 = string.Join(",\n", switchHandleList2);

        var typeDictStringEnumerable = typeList
            .Select(x => $"\t\t{{typeof({x}), \"{x}\"}}");
        var typeDictString = string.Join(",\n", typeDictStringEnumerable);

        var pendingDictStringEnumerable = typeList
            .Select(x => $"\t\t{{typeof({x}), new PendingTable<{x}>()}}");
        var pendingDictString = string.Join(",\n", pendingDictStringEnumerable);

        var serviceBaseDispatchFunc = $@"

    private readonly Dictionary<Type, object> _pendingTable = new()
    {{
{pendingDictString}
    }};

    private readonly Dictionary<Type, string> Type2StringName = new Dictionary<Type, string>()
    {{
{typeDictString}
    }};

    protected override Dictionary<Type, string> GetType2StringNameDict()
    {{
        return Type2StringName;
    }}

    protected override Dictionary<Type, object> GetPendingTable()
    {{
        return _pendingTable;
    }}

    protected override bool RpcCallBack2(long msgSeq, IServiceMessage msg)
    {{
        var type = this.GetMsgCallResultType(msgSeq);
        this.RemoveMsgCallResultType(msgSeq);
        var name = this.Type2StringName[type];

        return name switch
        {{
{switchHandleFunc}
        }};
    }}

    protected override bool SetRpcCallBackException2(long msgSeq, Exception exception)
    {{
        var type = this.GetMsgCallResultType(msgSeq);
        this.RemoveMsgCallResultType(msgSeq);
        var name = this.Type2StringName[type];

        return name switch
        {{
{switchHandleFunc2}
        }};
    }}



";
        return serviceBaseDispatchFunc;
    }

    public List<string> GetTypeList(ImmutableArray<string> typeNames)
    {
        var validTypes = typeNames.Where(t => !string.IsNullOrEmpty(t)).Cast<string>().Distinct().ToList();
        return validTypes;
    }

    private static bool IsCallMethodInvocation(SyntaxNode node)
    {
        return false;
        // TODO 后面完善下call结果的类型检查收集
        //return node switch
        //{
        //    // 匹配 Service.Result<T>() 的直接调用
        //    InvocationExpressionSyntax
        //    {
        //        Expression: MemberAccessExpressionSyntax
        //        {
        //            Expression: IdentifierNameSyntax { Identifier.ValueText: "Service" },
        //            Name: GenericNameSyntax { Identifier.ValueText: "Result" }
        //        }
        //    } => true,

        //    // 匹配 this.Service.Result<T>() 或 base.Service.Result<T>()
        //    //InvocationExpressionSyntax
        //    //{
        //    //    Expression: MemberAccessExpressionSyntax
        //    //    {
        //    //        Expression: MemberAccessExpressionSyntax
        //    //        {
        //    //            Expression: (ThisExpressionSyntax or BaseExpressionSyntax),
        //    //            Name: IdentifierNameSyntax { Identifier.ValueText: "Service" }
        //    //        },
        //    //        Name: GenericNameSyntax { Identifier.ValueText: "Result" }
        //    //    }
        //    //} => true,
        //    _ => false
        //};
    }

    private static string GetCallType(GeneratorSyntaxContext s, CancellationToken cancellationToken)

    {

        var symbol = s.SemanticModel.GetDeclaredSymbol(s.Node, cancellationToken);
        //return symbol as INamedTypeSymbol;
        var semantic = s.SemanticModel;
        var invoke = (InvocationExpressionSyntax)s.Node;
        var method = semantic.GetSymbolInfo(invoke).Symbol as IMethodSymbol;
        var x = method?.TypeArguments.FirstOrDefault() as INamedTypeSymbol;
        return x!.ToDisplayString();
    }

    private static void GenerateExactFormatSource(SourceProductionContext context, ImmutableArray<string> typeNames)
    {
        var validTypes = typeNames.Where(t => !string.IsNullOrEmpty(t)).Cast<string>().Distinct().ToList();

        if (validTypes.Count == 0)
            return;

        var source = GenerateExactFormatClass(validTypes);
        context.AddSource("ExactTypeHashMapping.g.cs", source);
    }

    private static string GenerateExactFormatClass(List<string> typeNames)
    {
        var sb = new StringBuilder();
        sb.AppendLine(@"// <auto-generated/>
using System;
using System.Collections.Generic;

namespace TypeHashMapping
{
    public static class TypeHashMapper
    {");

        // 生成你要的确切格式
        sb.AppendLine("        public static readonly Dictionary<Type, uint> Type2Hash = new Dictionary<Type, uint>()");
        sb.AppendLine("        {");

        foreach (var typeName in typeNames)
        {
            var hash = ComputeHash(typeName);
            sb.AppendLine($"            {{ typeof({typeName}), {hash}u }},");
        }

        sb.AppendLine("        };");
        sb.AppendLine("    }");
        sb.AppendLine("}");

        return sb.ToString();
    }

    private static uint ComputeHash(string input)
    {
        if (string.IsNullOrEmpty(input)) return 0;

        // 使用简单的哈希算法，确保生成 uint 值
        var hash = 5381u;
        foreach (var c in input)
        {
            hash = ((hash << 5) + hash) ^ c;
        }
        return hash & 0xFFFFFFFF; // 确保是 uint
    }
}