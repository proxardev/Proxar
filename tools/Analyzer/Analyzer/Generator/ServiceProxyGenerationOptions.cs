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

namespace ServiceDispatchMethodGenerator;


public enum ServiceProxyGenerationType
{
    InternalProxy = 0,
    ExternalProxy = 1,
}


public class ServiceProxyGenerationOptions
{
    public ServiceProxyGenerationType Type { get; set; } = 0;

    public string Namespace { get; set; } = string.Empty;
    public string Suffix { get; set; } = string.Empty;

    public string ClassName { get; set; } = string.Empty;

    public string BaseInterfaceName { get; set; } = string.Empty;

    public bool GeneralSpacialProxy { get; set; } = false;

    public string FilterAttribute { get; set; } = string.Empty;

    public string MessageInvoker { get; set; } = string.Empty;

    public int ModuleExternalProxyPreId { get; set; } = 0;





}




public static class ServiceProxyGenerationOptionsExtension
{
    public static string GetInterfaceName(this ServiceProxyGenerationOptions self, ServiceInfo serviceInfo)
    {
        var interfaceName = $"{serviceInfo.GetInterfaceName()}{self.Suffix}";
        return interfaceName;
    }

    public static long GetProxyId(this ServiceProxyGenerationOptions self, ServiceInfo serviceInfo)
    {
        if (self.Type == ServiceProxyGenerationType.ExternalProxy)
        {
            var proxyId = serviceInfo.ServiceClassSymbol
                .GetSymbolAttrValue<long>(ExternalProxyAttribute.AttributeName, ExternalProxyAttribute.ProxyField);
            return self.ModuleExternalProxyPreId * 10000 + proxyId;

            //return proxyId;
        }
        return 0;
    }
}