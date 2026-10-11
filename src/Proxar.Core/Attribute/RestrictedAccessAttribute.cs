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

namespace Proxar.CustomAttribute;

/// <summary>
/// 标记「禁止外部使用的入口」。
/// 被标记的成员（方法 / 属性 / 字段 / 事件 / 构造器 / 类 / 接口 / 结构 / 委托）只允许
/// 在其声明类型（含嵌套类型）内部被引用；任何外部引用都会被 Proxar.Analyzers (RS001) 拦截。
///
/// 用法：
///   [RestrictedAccess] void Send(...);      // 方法
///   [RestrictedAccess] int Counter { get; } // 属性
///   [RestrictedAccess] class InternalBus    // 整个类型（构造器 / 成员同理受限）
///
/// 无需（也不推荐）用任何白名单特性来「放行」特定调用方；
/// 若框架内部（如生成的代理类）确需调用，由源代码生成器自动插入
/// #pragma warning disable RS001 / restore 来豁免，业务手写代码不会获得该豁免。
/// </summary>
[AttributeUsage(
    AttributeTargets.Method
    | AttributeTargets.Property
    | AttributeTargets.Field
    | AttributeTargets.Event
    | AttributeTargets.Constructor
    | AttributeTargets.Class
    | AttributeTargets.Interface
    | AttributeTargets.Struct
    | AttributeTargets.Delegate,
    Inherited = false,
    AllowMultiple = false)]
internal sealed class RestrictedAccessAttribute : Attribute { }
