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
using Microsoft.CodeAnalysis.Text;
using ServiceDispatchMethodGenerator;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public static partial class ServiceProxyClassGenerate
{
    public static void GenerateProxyClass(int moduleExternalProxyPreId, SourceProductionContext sourceProductionContext, ServiceInfo serviceInfo)
    {
        var opetions = new ServiceProxyGenerationOptions
        {
            Prefix = "",
            BaseInterfaceName = $"IServiceProxy",
            GeneralSpacialProxy = true
        };
        var (succ, sourceCode) = GenerateServiceProxyClass(serviceInfo, opetions);
        var fileName = $"{serviceInfo.GetNamespace()}.{serviceInfo.GetClassName().Replace('<', '_').Replace('>', '_')}_Proxy.g.cs";
        sourceProductionContext.AddSource(fileName, SourceText.From(sourceCode, Encoding.UTF8));

        GenerateExternalProxyClass(moduleExternalProxyPreId, sourceProductionContext, serviceInfo);
    }

    public static void GenerateExternalProxyClass(int moduleExternalProxyPreId, SourceProductionContext sourceProductionContext, ServiceInfo serviceInfo)
    {
        var opetions = new ServiceProxyGenerationOptions
        {
            Type = ServiceProxyGenerationType.ExternalProxy,
            Prefix = "External",
            BaseInterfaceName = $"IExternalProxy",
            GeneralSpacialProxy = false,
            FilterAttribute = "Proxar.ServiceCore.ServiceMethodExportAttribute",
            MessageInvoker = "MessageInvoker",
            ModuleExternalProxyPreId = moduleExternalProxyPreId,
        };
        var (succ, sourceCode) = GenerateServiceProxyClass(serviceInfo, opetions);
        if (!succ)
        {
            return;
        }
        var fileName = $"{serviceInfo.GetNamespace()}.{serviceInfo.GetClassName().Replace('<', '_').Replace('>', '_')}_{opetions.Prefix}Proxy.g.cs";
        sourceProductionContext.AddSource(fileName, SourceText.From(sourceCode, Encoding.UTF8));
    }

    // 生成分部类代码
    private static (bool, string) GenerateServiceProxyClass(ServiceInfo serviceInfo, ServiceProxyGenerationOptions serviceProxyGenerationOptions)
    {
        var methods = serviceInfo.GetProtoMethodSymbols()
            .Where(x => serviceProxyGenerationOptions.FilterAttribute == string.Empty || x.HasAttribute(serviceProxyGenerationOptions.FilterAttribute))
            .ToList();
        var namespaceName = serviceInfo.GetNamespace();


        var usingInfo = serviceInfo.GetUsingString();
        var code = $@"
#nullable enable
using System;
using MessagePack;
using System.Collections.Generic;
using Proxar.ServiceCore;
using Proxar.ServiceCore.Interfaces;
using Proxar.Tasks;



{(string.IsNullOrEmpty(namespaceName) ? "" : $"namespace {namespaceName};")}

{MakeServiceProxyInterface(serviceInfo, serviceProxyGenerationOptions)}

{MakeServiceProxy(serviceInfo, serviceProxyGenerationOptions)}

";

        return (methods.Any(), code);
    }

    public static string GenerateProxyConstructionMethod(string proxyClassName, bool withBaseClass)
    {
        var baseClassConstructionStr = ":base(serviceId)";
        var baseClassConstructionStr2 = ":base(serviceId, messageInvoker)";
        if (!withBaseClass)
        {
            baseClassConstructionStr = "";
            baseClassConstructionStr2 = "";
        }
        var method = $@"

    /// <summary>
    /// 使用指定的服务 ID 初始化 <see cref=""{proxyClassName}""/> 的新实例。
    /// 消息发送器将使用 <see cref=""Service.MessageInvoker""/> 的默认实例。
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <param name=""serviceId"">目标服务的唯一标识符。</param>
    public {proxyClassName}(long serviceId){baseClassConstructionStr}
    {{
    }}

    /// <summary>
    /// 使用指定的服务 ID 和自定义 <see cref=""IMessageInvoker""/> 初始化 <see cref=""{proxyClassName}""/> 的新实例。
    /// </summary>
    /// <param name=""serviceId"">目标服务的唯一标识符。</param>
    /// <param name=""messageInvoker"">用于发送消息的自定义调用器。</param>
    public {proxyClassName}(long serviceId, IMessageInvoker messageInvoker){baseClassConstructionStr2}
    {{
    }}
";
        return method;
    }

    public static string GenerateProxyConstructionMethod(ServiceInfo serviceInfo, ServiceProxyGenerationOptions serviceProxyGenerationOptions)
    {
        if (serviceInfo.IsServiceBaseClass())
        {
            return "";
        }
        var proxyClassName = serviceInfo.GetProxyClassName(serviceProxyGenerationOptions.Prefix);

        return GenerateProxyConstructionMethod(proxyClassName, true);
    }

    public static string GenerateInheritClassName(ServiceInfo serviceInfo, ServiceProxyGenerationOptions serviceProxyGenerationOptions)
    {
        if (serviceInfo.IsServiceBaseClass())
        {
            return "";
        }

        var baseProxyClassName = serviceInfo.GetDirectParentProxyClassName(serviceProxyGenerationOptions.Prefix);
        return $": {baseProxyClassName}, {serviceProxyGenerationOptions.GetInterfaceName(serviceInfo)}";
    }

    public static string GenerateProxyCreateMethod(ServiceInfo serviceInfo, ServiceProxyGenerationOptions serviceProxyGenerationOptions)
    {
        if (serviceInfo.IsServiceBaseClass())
        {
            return "";
        }
        var proxyClassName = serviceInfo.GetProxyClassName(serviceProxyGenerationOptions.Prefix);
        var method = $@"


    public new static {serviceProxyGenerationOptions.BaseInterfaceName} Create(long serviceId)
    {{
        return new {proxyClassName}(serviceId);
    }}
";
        return method;
    }


    public static string MakeServiceProxyInterface(ServiceInfo serviceInfo, ServiceProxyGenerationOptions serviceProxyGenerationOptions)
    {
        var className = serviceInfo.GetClassName();
        var accessibility = serviceInfo.GetAccessibilityType();
        var methods = serviceInfo.GetProtoMethodSymbols()
            .Where(x => serviceProxyGenerationOptions.FilterAttribute == string.Empty || x.HasAttribute(serviceProxyGenerationOptions.FilterAttribute))
            .ToList();
        var proxyInterfaceMethod = string.Join("", methods.Select(x => MakeServiceDispathMethodProxyIntf(className, x)));


        var interfaceName = serviceProxyGenerationOptions.GetInterfaceName(serviceInfo);



        var proxyInterfaceDeclaration = $@"
/// <summary>
/// <see cref=""{className}""/> 服务的协议接口，由源生成器自动生成。
/// 包含该服务所有标记为 <see cref=""{ServiceInfo.ServiceMethodAttributeName}""/> 的协议方法。
/// </summary>
/// <remarks>
/// 实现由代理类提供。
/// </remarks>
{accessibility} interface {interfaceName}: {serviceProxyGenerationOptions.BaseInterfaceName}
{{
{proxyInterfaceMethod}
}}


";
        return proxyInterfaceDeclaration;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="serviceInfo"></param>
    /// <param name="proxyOption"></param>
    /// <returns></returns>
    public static string MakeServiceProxy(ServiceInfo serviceInfo, ServiceProxyGenerationOptions proxyOption)
    {
        var className = serviceInfo.GetClassName();
        var accessibility = serviceInfo.GetAccessibilityType();
        var methods = serviceInfo.GetProtoMethodSymbols()
            .Where(x => proxyOption.FilterAttribute == string.Empty || x.HasAttribute(proxyOption.FilterAttribute))
            .ToList();
        var proxyMethod = string.Join("", methods.Select(x => MakeServiceDispathMethodProxy(className, x, serviceProxyGenerationOptions: proxyOption)));

        var proxyClassName = serviceInfo.GetProxyClassName(proxyOption.Prefix);

        var inheritClassName = GenerateInheritClassName(serviceInfo, proxyOption);

        var spacialServiceProxyInfo = "";
        if (proxyOption.GeneralSpacialProxy)
        {
            spacialServiceProxyInfo = MakePartServiceProxy(serviceInfo, proxyOption);
        }

        var proxyIntfDeclaration = $@"

{accessibility} partial class {proxyClassName}  {inheritClassName}
{{
#pragma warning disable CS0108
    /// <summary>
    /// 服务代理的唯一标识符，用于在系统中区分不同的代理类。
    /// </summary>
    /// <remarks>
    /// 在外部代理中有效，内部代理中，只是一个占位字段。
    /// </remarks>
    public const  long ProxyId = {proxyOption.GetProxyId(serviceInfo)};
#pragma warning restore CS0108

{GenerateProxyConstructionMethod(serviceInfo, proxyOption)}


{GenerateProxyCreateMethod(serviceInfo, proxyOption)}

{proxyMethod}

{spacialServiceProxyInfo}

}}



";
        return proxyIntfDeclaration;
    }

    public static string GenerateSpacilProxyConstructionMethod(string proxyClassName, string accessibility)
    {
        var method = $@"

    /// <summary>
    /// 使用指定的服务 ID 初始化 <see cref=""{proxyClassName}""/> 的新实例。
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <param name=""serviceId"">目标服务的唯一标识符。</param>
    {accessibility} {proxyClassName}(long serviceId)
    {{
        this.ServiceId = serviceId;
    }}
";
        return method;
    }

    public static string MakePartServiceProxy(ServiceInfo serviceInfo, ServiceProxyGenerationOptions serviceProxyGenerationOptions)
    {
        var className = serviceInfo.GetClassName();
        var accessibility = serviceInfo.GetAccessibilityType();
        var proxyMethod = string.Join("", serviceInfo.GetProtoMethodSymbols().Select(x => MakeServiceDispathMethodProxy(className, x)));

        var proxyClassName = serviceInfo.GetProxyClassName(serviceProxyGenerationOptions.Prefix);
        var baseProxyClassName = serviceInfo.GetDirectParentProxyClassName(serviceProxyGenerationOptions.Prefix);

        var inheritClassName = GenerateInheritClassName(serviceInfo, serviceProxyGenerationOptions);

        var baseClass = $"{serviceInfo.ServiceClassSymbol.BaseType.Name}Proxy,";
        if (serviceInfo.IsServiceBaseClass())
        {
            baseClass = "";
        }
        var rawMethods = new List<IMethodSymbol>();
        var queue0Methods = new List<IMethodSymbol>();
        var queue0RawMethods = new List<IMethodSymbol>();
        foreach (var methodSymbol in serviceInfo.GetProtoMethodSymbols())
        {
            var raw = methodSymbol.IsProvideRawArgsProxy();
            var queue0 = methodSymbol.IsProvideQueue0ArgsProxy();
            if (raw && queue0)
            {
                queue0RawMethods.Add(methodSymbol);
            }
            if (raw)
            {
                rawMethods.Add(methodSymbol);
            }
            if (queue0)
            {
                queue0Methods.Add(methodSymbol);
            }
        }
        var proxyMethodDeclaration = $"// {serviceInfo.GetProtoMethodSymbols().Count} {queue0RawMethods.Count + rawMethods.Count + queue0Methods.Count}" +
            $"\n";
        if (rawMethods.Count > 0)
        {
            var proxyMethod2 = string.Join("",
                rawMethods.Select(x => MakeServiceDispathMethodProxy(className, x, false, false, MethodTypeDef.Raw)));
            var rawStructName = $"{proxyClassName}_Raw";
            proxyMethodDeclaration += $@"
#pragma warning disable CS0108
    /// <summary>
    /// 获取用于发送不序列化（Raw）消息的内部结构体。使用其调用方法，调用方式不经过序列化，直接传递原始字节。
    /// </summary>
    public {proxyClassName}_Raw Raw => new(GetServiceId());
#pragma warning restore CS0108

    /// <summary>
    /// 封装了不序列化（Raw）发送语义的内部结构体。所有通过此结构体调用的方法都将以原始字节方式发送。
    /// </summary>
    {accessibility} struct {proxyClassName}_Raw
    {{
        private long ServiceId;
{GenerateSpacilProxyConstructionMethod(rawStructName, accessibility)}

    {proxyMethod2}
    }}

";
        }
        if (queue0Methods.Count > 0)
        {
            var proxyMethod2 = string.Join("",
                queue0Methods.Select(x => MakeServiceDispathMethodProxy(className, x, false, false, MethodTypeDef.Queue0)));

            var queue0StructName = $"{proxyClassName}_Queue0";
            proxyMethodDeclaration += $@"
#pragma warning disable CS0108

    /// <summary>
    /// 获取用于向0号队列（优先队列）发送消息的内部结构体。使用其调用方法，消息将被放入优先队列。
    /// </summary>
    public {proxyClassName}_Queue0 Queue0 => new(GetServiceId());
#pragma warning restore CS0108

    /// <summary>
    /// 封装了向0号队列（优先队列）发送语义的内部结构体。所有通过此结构体调用的方法都会优先处理。
    /// </summary>
    {accessibility} struct {proxyClassName}_Queue0
    {{
        private long ServiceId;
{GenerateSpacilProxyConstructionMethod(queue0StructName, accessibility)}
    {proxyMethod2}
    }}

";
        }
        if (queue0RawMethods.Count > 0)
        {
            var proxyMethod2 = string.Join("",
                queue0RawMethods.Select(x => MakeServiceDispathMethodProxy(className, x, false, false, MethodTypeDef.Queue0Raw)));

            var queue0RawStructName = $"{proxyClassName}_Queue0Raw";
            proxyMethodDeclaration += $@"
#pragma warning disable CS0108
    /// <summary>
    /// 获取用于向0号队列（优先队列）发送不序列化（Raw）消息的内部结构体。
    /// </summary>
    public {proxyClassName}_Queue0Raw Queue0Raw => new(GetServiceId());
#pragma warning restore CS0108

    /// <summary>
    /// 封装了向0号队列发送不序列化（Raw）消息语义的内部结构体。所有通过此结构体调用的方法都将以原始字节方式优先处理。
    /// </summary>
    {accessibility} struct {proxyClassName}_Queue0Raw
    {{
        private long ServiceId;
{GenerateSpacilProxyConstructionMethod(queue0RawStructName, accessibility)}
    {proxyMethod2}
    }}

";
        }

        return proxyMethodDeclaration;
    }

    /// <summary>
    /// 生成方法，至少是internal的
    /// </summary>
    /// <param name="methodSymbol"></param>
    /// <returns></returns>
    public static Accessibility GenerateMethodAccessibility(IMethodSymbol methodSymbol)
    {

        var original = methodSymbol.DeclaredAccessibility;

        return original switch
        {
            Accessibility.Private => Accessibility.Internal,
            Accessibility.ProtectedAndInternal => Accessibility.Internal,
            _ => original
        };
    }

    public static string MakeServiceDispathProxyMethodDeclarations(string className,
        IMethodSymbol methodSymbol, bool isInterface,
        bool appointServiceId = false,
        bool staticMethos = false)
    {
        var accessibility = GenerateMethodAccessibility(methodSymbol);
        var methodName = methodSymbol.Name;
        var proto = ServiceInfo.GetProto(methodSymbol);
        var paramList = methodSymbol.Parameters
            .Select(x => $"{x.Type.ToDisplayString()} {x.Name}")
            .ToList();
        if (appointServiceId)
        {
            paramList.Insert(0, "long ServiceId");
        }
        var @params = string.Join(", ", paramList);
        var returnType = GetServiceDispathProxyMethodReturnType(methodSymbol, isInterface);

        var staticMark = staticMethos ? " static" : "";

        var method = $@"

    /// <summary>
    /// <see cref=""{className}.{methodName}""/>
    /// </summary>
    public {staticMark} {returnType} {methodName}({@params})";
        //{accessibility.ToString().ToLower()}{staticMark} {returnType} {methodName}({@params})";

        return method;
    }

    public static string GetServiceDispathProxyMethodReturnType(IMethodSymbol methodSymbol, bool isInterface = false)
    {
        if (methodSymbol.ReturnsVoid)
        {
            return "void";
        }
        var asyncMethodTag = "";
        if (isInterface)
        {
            asyncMethodTag = "";
        }
        if (!methodSymbol.IsAsync)
        {
            return $"{asyncMethodTag}ZFTask<{methodSymbol.ReturnType.ToDisplayString()}>";
        }
        var namedType = methodSymbol.ReturnType as INamedTypeSymbol;
        if (namedType != null && namedType.IsGenericType && namedType.ConstructedFrom.Name == "ZFTask")
        {
            return $"{asyncMethodTag}{methodSymbol.ReturnType.ToDisplayString()}";
        }
        return "void";
    }

    public static string MakeServiceDispathMethodProxyIntf(string className, IMethodSymbol methodSymbol)
    {
        var declarations = MakeServiceDispathProxyMethodDeclarations(className, methodSymbol, true);
        var method = $@"{declarations};";
        return method;
    }

    public static string MakeServiceDispathMethodProxy(string className, IMethodSymbol methodSymbol,
        bool appointServiceId = false,
        bool staticMethod = false,
        int methodType = 0, ServiceProxyGenerationOptions serviceProxyGenerationOptions = null)
    {
        var declarations = MakeServiceDispathProxyMethodDeclarations(className, methodSymbol, false, appointServiceId, staticMethod);
        var proto = ServiceInfo.GetProto(methodSymbol);
        var argsList = methodSymbol.Parameters
            .Select(x => $"{x.Name}").Prepend($"{proto}")
            .ToList();
        var args = string.Join(", ", argsList);
        var sendStatement = GetServiceDispathMethodProxySendFunc(methodSymbol, methodType);
        var messageInvokerArgs = "";
        if (serviceProxyGenerationOptions != null && serviceProxyGenerationOptions.MessageInvoker != string.Empty)
        {
            messageInvokerArgs = $", {serviceProxyGenerationOptions.MessageInvoker}, ProxyId";
        }
        var method = $@"
    {declarations}
    {{
        {sendStatement}(ServiceId, {args}{messageInvokerArgs});
    }}
";
        return method;
    }

    public static class MethodTypeDef
    {
        public const int Normal = 0;
        public const int Raw = 2;
        public const int Queue0 = 3;
        public const int Queue0Raw = 4;
    }

    public static string GetServiceDispathMethodProxySendFunc(IMethodSymbol methodSymbol, int methodType = 0)
    {
        var send = "Send";
        var call = "Call";
        if (methodType == MethodTypeDef.Raw)
        {
            send = "SendRaw";
            call = "CallRaw";
        }
        else if (methodType == MethodTypeDef.Queue0)
        {
            send = "SendQueue0";
            call = "CallQueue0";
        }
        else if (methodType == MethodTypeDef.Queue0Raw)
        {
            send = "SendQueue0Raw";
            call = "CallQueue0Raw";
        }

        var returnType = methodSymbol.ReturnType.ToDisplayString();
        if (methodSymbol.ReturnsVoid)
        {
            return $"Service.{send}";
        }
        if (!methodSymbol.IsAsync)
        {
            return $"return Service.Result<{returnType}>().{call}";
        }

        var namedType = methodSymbol.ReturnType as INamedTypeSymbol;
        if (!namedType.IsGenericType)
        {
            return $"Service.{send}";
        }
        returnType = namedType.TypeArguments[0].ToDisplayString();
        return $"return Service.Result<{returnType}>().{call}";
    }


}