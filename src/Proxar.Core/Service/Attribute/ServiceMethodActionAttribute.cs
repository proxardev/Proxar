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
/// 标记协议方法的额外行为，控制源生成器为该协议方法生成的代理变体。
/// </summary>
/// <remarks>
/// 可以指定是否生成原始参数（RawArgs）版本的调用，以及是否将该方法的调用放入优先处理队列（Queue0）。
/// 此特性与 <see cref="ServiceMethodAttribute"/> 搭配使用。
/// </remarks>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
public sealed class ServiceMethodActionAttribute : Attribute
{
    /// <summary>
    /// 获取一个值，指示源生成器是否应为此协议方法生成一个原始参数（RawArgs）版本的代理方法。
    /// 原始参数版本不执行序列化，直接传递消息负载，适合零拷贝场景。
    /// </summary>
    public bool RawArgsMethod { get; }

    /// <summary>
    /// 获取一个值，指示源生成器是否应为此协议方法生成一个将调用放入 0 号队列（优先处理）的代理方法。
    /// </summary>
    internal bool Queue0ArgsMethod { get; }

    /// <summary>
    /// 使用指定的原始参数模式初始化 <see cref="ServiceMethodActionAttribute"/> 的新实例。
    /// 默认不生成 0 号队列版本。
    /// </summary>
    /// <param name="rawArgsMethod">如果为 <c>true</c>，则生成原始参数版本的代理方法。</param>
    public ServiceMethodActionAttribute(bool rawArgsMethod)
    {
        RawArgsMethod = rawArgsMethod;
    }

    /// <summary>
    /// （内部使用）同时设置原始参数模式和优先队列模式。
    /// </summary>
    /// <param name="rawArgsMethod">是否生成原始参数版本。</param>
    /// <param name="queue0ArgsMethod">是否生成 0 号队列版本。</param>
    internal ServiceMethodActionAttribute(bool rawArgsMethod, bool queue0ArgsMethod)
    {
        RawArgsMethod = rawArgsMethod;
        Queue0ArgsMethod = queue0ArgsMethod;
    }
}