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
/// 标记一个协议方法应被导出到外部代理（ExternalProxy）中，供客户端或其他外部系统调用。
/// 此特性需与 <see cref="ServiceExportAttribute"/> 配合使用。
/// </summary>
/// <remarks>
/// 带有此特性的方法会被源生成器自动生成到外部代理类中。
/// </remarks>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
public sealed class ServiceMethodExportAttribute : Attribute
{
}