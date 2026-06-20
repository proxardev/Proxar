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


using Proxar.ServiceCore.Interfaces;
namespace Proxar.ServiceCore;

/// <summary>
/// 提供外部服务代理创建的静态工厂类。
/// </summary>
public static class ExternalService
{
    /// <summary>
    /// 获取外部服务代理（使用默认消息发送器）。
    /// </summary>
    public static T GetExternalServiceProxy<T>(long serviceId) where T : class, IExternalProxy
    {
        return (T.Create(serviceId) as T)!;
    }

    /// <summary>
    /// 获取外部服务代理（使用自定义消息发送器）。
    /// </summary>
    public static T GetExternalServiceProxy<T>(long serviceId, IMessageInvoker messageInvoker) where T : class, IExternalProxy
    {
        return (T.Create(serviceId, messageInvoker) as T)!;
    }
}