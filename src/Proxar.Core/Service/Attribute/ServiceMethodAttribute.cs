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
/// 标记一个方法为服务协议方法，并指定其协议编号（Proto）。
/// 源生成器会为标记此特性的方法生成相应的代理调用代码和消息分发逻辑。
/// </summary>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
public sealed class ServiceMethodAttribute : Attribute
{
    /// <summary>
    /// 协议方法编号（Proto），在当前服务内唯一。
    /// </summary>
    public int Proto { get; }

    /// <summary>
    /// 如果为 <c>true</c>，调用方不会自动处理返回值，方法自身负责将响应发回调用方。
    /// </summary>
    public bool SelfHandleReturn { get; }

    /// <summary>
    /// 使用指定的协议编号初始化 <see cref="ServiceMethodAttribute"/> 的新实例。
    /// 调用方将自动等待并处理返回值（如有）。
    /// </summary>
    /// <param name="proto">协议方法编号。</param>
    public ServiceMethodAttribute(int proto)
    {
        Proto = proto;
    }

    /// <summary>
    /// 使用指定的协议编号和返回值处理模式初始化 <see cref="ServiceMethodAttribute"/> 的新实例。
    /// </summary>
    /// <param name="proto">协议方法编号。</param>
    /// <param name="selfHandleReturn">
    /// 如果为 <c>true</c>，方法自行负责发送响应（适用于延迟响应的场景）；
    /// 如果为 <c>false</c>（默认），框架自动将返回值发回调用方。
    /// </param>
    internal ServiceMethodAttribute(int proto, bool selfHandleReturn)
    {
        Proto = proto;
        SelfHandleReturn = selfHandleReturn;
    }
}