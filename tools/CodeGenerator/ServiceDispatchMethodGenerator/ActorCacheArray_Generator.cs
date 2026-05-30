using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System.Collections.Immutable;
using System.Text;

namespace MyGenerator;

[Generator(LanguageNames.CSharp)]
public class ActorCacheArrayGenerator : IIncrementalGenerator
{
    private const string TargetBaseTypeName = "ZF.AbstractSynchronizationContextSingleInstance";

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var instanceUsages = context.SyntaxProvider
            .CreateSyntaxProvider(
                predicate: static (node, _) => IsInstanceAccess(node),
                transform: static (ctx, _) => ExtractTypeInfo(ctx))
            .Where(static t => t is not null)
            .Collect();

        context.RegisterSourceOutput(instanceUsages, (spc, typeInfos) =>
        {
            if (typeInfos.IsEmpty) return;

            var sortedTypes = typeInfos
                .Select(t => t!)
                .Distinct(TypeInfoComparer.Instance)
                .OrderBy(t => t.FullTypeName)
                .Select((t, index) => (t, index))
                .ToList();

            spc.AddSource("ActorTypeIndexProvider.g.cs",
                SourceText.From(GenerateIndexProvider(sortedTypes), Encoding.UTF8));
        });
    }

    private static bool IsInstanceAccess(SyntaxNode node)
    {
        if (node is not MemberAccessExpressionSyntax memberAccess)
            return false;

        if (memberAccess.Name.Identifier.Text != "Instance")
            return false;

        return memberAccess.Expression is IdentifierNameSyntax or GenericNameSyntax;
    }

    private static TypeInfo? ExtractTypeInfo(GeneratorSyntaxContext context)
    {
        var memberAccess = (MemberAccessExpressionSyntax)context.Node;
        var semanticModel = context.SemanticModel;

        // 获取左侧表达式的符号信息
        var symbolInfo = semanticModel.GetSymbolInfo(memberAccess.Expression);

        // 尝试获取类型符号（处理 Type<T>.Instance 的情况）
        INamedTypeSymbol? namedType = null;

        if (symbolInfo.Symbol is INamedTypeSymbol type)
        {
            namedType = type;
        }
        else if (symbolInfo.Symbol is IPropertySymbol prop)
        {
            // 处理通过属性访问的情况
            namedType = prop.ContainingType;
        }
        else if (memberAccess.Expression is GenericNameSyntax genericName)
        {
            // 直接获取泛型类型的符号
            var typeInfo = semanticModel.GetTypeInfo(memberAccess.Expression);
            namedType = typeInfo.Type as INamedTypeSymbol;
        }

        if (namedType == null)
            return null;

        // ? 关键修复：严格排除未绑定的泛型类型
        if (!IsValidConstructedType(namedType))
            return null;

        // 检查是否继承自目标基类
        if (!InheritsFromBase(namedType))
            return null;

        // 提取实际类型参数（对于泛型）或类型本身（对于非泛型）
        ITypeSymbol targetType;

        if (namedType.IsGenericType && namedType.TypeArguments.Length == 1)
        {
            var typeArg = namedType.TypeArguments[0];

            // ? 再次确认类型参数不是开放泛型参数
            if (typeArg is ITypeParameterSymbol)
                return null;

            // ? 确认类型参数是具体的、已构造的类型
            if (typeArg is INamedTypeSymbol argType && !IsFullyConstructed(argType))
                return null;

            targetType = typeArg;
        }
        else
        {
            // 非泛型单例：XXSyncSingle
            targetType = namedType;
        }

        // ? 最终检查：排除所有泛型参数类型
        if (targetType is ITypeParameterSymbol)
            return null;

        var fullName = targetType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);

        // ? 额外安全检查：排除包含 ` 或 <T> 的名称（开放泛型标记）
        if (fullName.Contains('`') || fullName.Contains("<T>") || fullName == "T")
            return null;

        return new TypeInfo(
            targetType.Name,
            fullName,
            targetType.ContainingNamespace.IsGlobalNamespace
                ? null
                : targetType.ContainingNamespace.ToDisplayString(),
            namedType.IsGenericType
        );
    }

    /// <summary>
    /// 检查是否为有效的构造类型（排除开放泛型定义）
    /// </summary>
    private static bool IsValidConstructedType(INamedTypeSymbol type)
    {
        // 排除未绑定的泛型类型（如 ObjectCachePoolSingle<>）
        if (type.IsUnboundGenericType)
            return false;

        // 排除泛型类型定义本身（如 ObjectCachePoolSingle<T> 的类定义）
        if (type.IsDefinition && type.TypeParameters.Length > 0)
            return false;

        // 如果是泛型类型，确保所有类型参数都是具体类型
        if (type.IsGenericType)
        {
            foreach (var arg in type.TypeArguments)
            {
                if (arg is ITypeParameterSymbol)
                    return false;

                // 递归检查嵌套泛型
                if (arg is INamedTypeSymbol namedArg && !IsFullyConstructed(namedArg))
                    return false;
            }
        }

        return true;
    }

    /// <summary>
    /// 确保类型完全构造（没有开放的泛型参数）
    /// </summary>
    private static bool IsFullyConstructed(INamedTypeSymbol type)
    {
        if (type.IsUnboundGenericType)
            return false;

        if (type.IsDefinition && type.TypeParameters.Length > 0)
            return false;

        if (type.IsGenericType)
        {
            foreach (var arg in type.TypeArguments)
            {
                if (arg is ITypeParameterSymbol)
                    return false;

                if (arg is INamedTypeSymbol namedArg && !IsFullyConstructed(namedArg))
                    return false;
            }
        }

        return true;
    }

    private static bool InheritsFromBase(INamedTypeSymbol type)
    {
        var current = type.BaseType;
        while (current != null)
        {
            if (current.ContainingNamespace.ToDisplayString() == "ZF" &&
                current.Name == "AbstractSynchronizationContextSingleInstance" &&
                current.IsGenericType)
            {
                return true;
            }
            current = current.BaseType;
        }
        return false;
    }

    private static string GenerateIndexProvider(List<(TypeInfo type, int index)> types)
    {
        var sb = new StringBuilder();
        sb.AppendLine("// <auto-generated />");
        sb.AppendLine("using System;");
        sb.AppendLine("using System.Collections.Generic;");
        sb.AppendLine();
        sb.AppendLine("namespace ZF.Cache;");
        sb.AppendLine();
        sb.AppendLine("/// <summary>");
        sb.AppendLine("/// 为所有继承自 AbstractSynchronizationContextSingleInstance 的类型提供索引映射");
        sb.AppendLine("/// </summary>");
        sb.AppendLine("public static class ActorTypeIndexProvider");
        sb.AppendLine("{");
        sb.AppendLine("    private static readonly Dictionary<Type, int> _map = new()");
        sb.AppendLine("    {");

        foreach (var (type, index) in types)
        {
            var cleanTypeName = type.FullTypeName.Replace("global::", "");
            sb.AppendLine($"        [typeof({cleanTypeName})] = {index},");
        }

        sb.AppendLine("    };");
        sb.AppendLine();
        sb.AppendLine("    public static int GetIndex<T>() => _map.TryGetValue(typeof(T), out var i) ? i : -1;");
        sb.AppendLine("    public static int GetIndex(Type t) => _map.TryGetValue(t, out var i) ? i : -1;");
        sb.AppendLine("    public static int Count => _map.Count;");
        sb.AppendLine("}");

        return sb.ToString();
    }

    private class TypeInfo : IEquatable<TypeInfo>
    {
        public string Name { get; }
        public string FullTypeName { get; }
        public string? Namespace { get; }
        public bool IsFromGenericInstance { get; }

        public TypeInfo(string name, string fullTypeName, string? @namespace, bool isFromGenericInstance = false)
        {
            Name = name;
            FullTypeName = fullTypeName;
            Namespace = @namespace;
            IsFromGenericInstance = isFromGenericInstance;
        }

        public bool Equals(TypeInfo? other) => other?.FullTypeName == FullTypeName;
        public override bool Equals(object? obj) => Equals(obj as TypeInfo);
        public override int GetHashCode() => FullTypeName.GetHashCode();
    }

    private class TypeInfoComparer : IEqualityComparer<TypeInfo>
    {
        public static readonly TypeInfoComparer Instance = new();
        public bool Equals(TypeInfo? x, TypeInfo? y) => x?.FullTypeName == y?.FullTypeName;
        public int GetHashCode(TypeInfo obj) => obj.FullTypeName.GetHashCode();
    }
}