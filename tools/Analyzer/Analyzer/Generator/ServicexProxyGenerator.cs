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


//public static partial class ServiceProxyClassGenerate
//{
//    // 生成远程接口 + 远程代理类
//    private static string GenerateRemoteProxyArtifacts(ServiceInfo serviceInfo, List<IMethodSymbol> exportedMethods)
//    {
//        var className = serviceInfo.GetClassName();
//        var accessibility = "public"; // 远程接口和代理通常公开
//        var namespaceName = serviceInfo.GetNamespace();

//        var interfaceCode = GenerateRemoteInterface(className, accessibility, exportedMethods);
//        var proxyCode = GenerateRemoteProxyClass(className, accessibility, exportedMethods);

//        return $@"
//#nullable enable
//using System;
//using System.Threading.Tasks;

//{(string.IsNullOrEmpty(namespaceName) ? "" : $"namespace {namespaceName};")}

//{interfaceCode}

//{proxyCode}
//";
//    }

//    private static string GenerateRemoteInterface(string className, string accessibility, List<IMethodSymbol> exportedMethods)
//    {
//        var methods = string.Join("\n", exportedMethods.Select(m =>
//            MakeServiceDispathMethodProxyIntf(className, m)));

//        return $@"
//{accessibility} interface I{className}Remote
//{{
//{methods}
//}}
//";
//    }

//    private static string GenerateRemoteProxyClass(string className, string accessibility, List<IMethodSymbol> exportedMethods)
//    {
//        var methods = string.Join("\n", exportedMethods.Select(m =>
//        {
//            var signature = MakeServiceDispathProxyMethodDeclarations(className, m, false);
//            var body = GenerateRemoteInvokerBody(m);
//            return $@"
//    {signature}
//    {{
//        {body}
//    }}";
//        }));

//        return $@"
//{accessibility} class {className}RemoteProxy : I{className}Remote
//{{
//    private readonly IMessageInvoker _invoker;
//    public long ServiceId {{ get; }}

//    public {className}RemoteProxy(long serviceId, IMessageInvoker invoker)
//    {{
//        ServiceId = serviceId;
//        _invoker = invoker ?? throw new ArgumentNullException(nameof(invoker));
//    }}

//{methods}
//}}
//";
//    }
//}