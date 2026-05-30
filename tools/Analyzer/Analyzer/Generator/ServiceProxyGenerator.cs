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
            Suffix = "",
            BaseInterfaceName = $"IServiceProxy",
            GeneralSpacialProxy = true
        };
        var (succ, sourceCode) = GenerateServiceProxyClass(serviceInfo, opetions);
        var fileName = $"{serviceInfo.GetClassName().Replace('<', '_').Replace('>', '_')}_Proxy.g.cs";
        sourceProductionContext.AddSource(fileName, SourceText.From(sourceCode, Encoding.UTF8));

        GenerateExternalProxyClass(moduleExternalProxyPreId, sourceProductionContext, serviceInfo);
    }

    public static void GenerateExternalProxyClass(int moduleExternalProxyPreId, SourceProductionContext sourceProductionContext, ServiceInfo serviceInfo)
    {
        var opetions = new ServiceProxyGenerationOptions
        {
            Type = ServiceProxyGenerationType.ExternalProxy,
            Suffix = "External",
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
        var fileName = $"{serviceInfo.GetClassName().Replace('<', '_').Replace('>', '_')}_Proxy{opetions.Suffix}.g.cs";
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

    public static string GenerateProxyConstructionMethod(ServiceInfo serviceInfo, ServiceProxyGenerationOptions serviceProxyGenerationOptions)
    {
        if (serviceInfo.IsServiceBaseClas())
        {
            return "";
        }
        var method = $@"
    public {serviceInfo.GetProxyClassName()}{serviceProxyGenerationOptions.Suffix}(long serviceId):base(serviceId)
    {{
    }}

    public {serviceInfo.GetProxyClassName()}{serviceProxyGenerationOptions.Suffix}(long serviceId, IMessageInvoker messageInvoker):base(serviceId, messageInvoker)
    {{
    }}
";
        return method;
    }

    public static string GenerateInheritClassName(ServiceInfo serviceInfo, ServiceProxyGenerationOptions serviceProxyGenerationOptions)
    {
        if (serviceInfo.IsServiceBaseClas())
        {
            return "";
        }

        var baseProxyClassName = serviceInfo.GetDirectParentProxyClassName();
        return $": {baseProxyClassName}{serviceProxyGenerationOptions.Suffix}, {serviceProxyGenerationOptions.GetInterfaceName(serviceInfo)}";
    }

    public static string GenerateProxyCreateMethod(ServiceInfo serviceInfo, ServiceProxyGenerationOptions serviceProxyGenerationOptions)
    {
        if (serviceInfo.IsServiceBaseClas())
        {
            return "";
        }
        var proxyClassName = serviceInfo.GetProxyClassName();
        var method = $@"


    public new static {serviceProxyGenerationOptions.BaseInterfaceName} Create(long serviceId)
    {{
        return new {proxyClassName}{serviceProxyGenerationOptions.Suffix}(serviceId);
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

        var proxyClassName = serviceInfo.GetProxyClassName();

        var inheritClassName = GenerateInheritClassName(serviceInfo, proxyOption);

        var spacialServiceProxyInfo = "";
        if (proxyOption.GeneralSpacialProxy)
        {
            spacialServiceProxyInfo = MakePartServiceProxy(serviceInfo, proxyOption);
        }

        var proxyIntfDeclaration = $@"

{accessibility} partial class {proxyClassName}{proxyOption.Suffix}  {inheritClassName}
{{
#pragma warning disable CS0108
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

    public static string MakePartServiceProxy(ServiceInfo serviceInfo, ServiceProxyGenerationOptions serviceProxyGenerationOptions)
    {
        var className = serviceInfo.GetClassName();
        var accessibility = serviceInfo.GetAccessibilityType();
        var proxyMethod = string.Join("", serviceInfo.GetProtoMethodSymbols().Select(x => MakeServiceDispathMethodProxy(className, x)));

        var proxyClassName = serviceInfo.GetProxyClassName();
        var baseProxyClassName = serviceInfo.GetDirectParentProxyClassName();

        var inheritClassName = GenerateInheritClassName(serviceInfo, serviceProxyGenerationOptions);

        var baseClass = $"{serviceInfo.ServiceClassSymbol.BaseType.Name}Proxy,";
        if (serviceInfo.IsServiceBaseClas())
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

            proxyMethodDeclaration += $@"
#pragma warning disable CS0108
    public {proxyClassName}_Raw Raw => new(GetServiceId());
#pragma warning restore CS0108

    {accessibility} struct {proxyClassName}_Raw
    {{
        private long ServiceId;
        {accessibility} {proxyClassName}_Raw(long serviceId)
        {{
            this.ServiceId = serviceId;
        }}
    {proxyMethod2}
    }}

";
        }
        if (queue0Methods.Count > 0)
        {
            var proxyMethod2 = string.Join("",
                queue0Methods.Select(x => MakeServiceDispathMethodProxy(className, x, false, false, MethodTypeDef.Queue0)));

            proxyMethodDeclaration += $@"
#pragma warning disable CS0108
    public {proxyClassName}_Queue0 Queue0 => new(GetServiceId());
#pragma warning restore CS0108

    {accessibility} struct {proxyClassName}_Queue0
    {{
        private long ServiceId;
        {accessibility} {proxyClassName}_Queue0(long serviceId)
        {{
            this.ServiceId = serviceId;
        }}
    {proxyMethod2}
    }}

";
        }
        if (queue0RawMethods.Count > 0)
        {
            var proxyMethod2 = string.Join("",
                queue0RawMethods.Select(x => MakeServiceDispathMethodProxy(className, x, false, false, MethodTypeDef.Queue0Raw)));

            proxyMethodDeclaration += $@"
#pragma warning disable CS0108
    public {proxyClassName}_Queue0Raw Queue0Raw => new(GetServiceId());
#pragma warning restore CS0108

    {accessibility} struct {proxyClassName}_Queue0Raw
    {{
        private long ServiceId;
        {accessibility} {proxyClassName}_Queue0Raw(long serviceId)
        {{
            this.ServiceId = serviceId;
        }}
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