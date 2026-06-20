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


namespace Proxar.ServiceCore;

/// <summary>
/// 标记一个服务类为外部可访问的服务，并指定其外部代理的唯一标识符（ProxyId）。
/// 源生成器会为此类生成对应的外部代理（ExternalProxy），供客户端或其他外部系统进行远程调用。
/// </summary>
/// <remarks>
/// 标记了此特性的服务还必须使用 <see cref="ServiceMethodExportAttribute"/> 标记需要导出的具体协议方法。
/// </remarks>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public sealed class ServiceExportAttribute : Attribute
{
    /// <summary>
    /// 获取外部代理的唯一标识符（ProxyId）。
    /// </summary>
    /// <remarks>
    /// 该 ID 应与 <c>ModuleExternalProxyPreId</c>（在 .csproj 中配置）组合成最终路由 ID。
    /// </remarks>
    public long ProxyId { get; }

    /// <summary>
    /// 使用指定的外部代理标识符初始化 <see cref="ServiceExportAttribute"/> 的新实例。
    /// </summary>
    /// <param name="proxyId">外部代理的唯一标识符，在当前模块内唯一。</param>
    public ServiceExportAttribute(long proxyId)
    {
        ProxyId = proxyId;
    }
}