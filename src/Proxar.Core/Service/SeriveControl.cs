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


using MessagePack;
using Proxar.ServiceCore.Interfaces;
using Proxar.ServiceSynchronizationContext;
using Proxar.Tasks;

namespace Proxar.ServiceCore;


/// <summary>
/// 提供创建服务、获取服务代理、发送消息等核心操作的服务静态工具类。
/// </summary>
public static partial class Service
{
    /// <summary>
    /// 创建指定类型的服务实例。
    /// </summary>
    /// <typeparam name="T">要创建的服务类型，必须继承自 <see cref="ServiceBase"/> 且具有无参数构造函数。</typeparam>
    /// <returns>新创建服务的唯一标识符。</returns>
    public static long CreateService<T>()
        where T : ServiceBase, new()
    {
        return CreateService<T>(default);
    }

    internal static long CreateService<T>(ServiceBootstrapper? serviceBootstrapper)
        where T : ServiceBase, new()
    {
        var serviceId = ServiceManager.Instance.CreateService<T>();
        Send(serviceId, ProtoBase.ServiceStart, serviceBootstrapper);
        return serviceId;
    }

    /// <summary>
    /// 创建指定类型的唯一服务实例（进程内唯一）。
    /// </summary>
    /// <typeparam name="T">要创建的唯一服务类型，必须继承自 <see cref="ServiceBase"/> 且具有无参数构造函数。</typeparam>
    /// <returns>新创建服务的唯一标识符。</returns>
    public static long CreateUniqueService<T>()
        where T : ServiceBase, new()
    {
        var serviceId = ServiceManager.Instance.CreateUniqueService<T>();
        Send(serviceId, ProtoBase.ServiceStart, default(ServiceBootstrapper));
        return serviceId;
    }

    /// <summary>
    /// 获取已注册的唯一服务的标识符。
    /// </summary>
    /// <typeparam name="T">唯一服务的类型。</typeparam>
    /// <returns>唯一服务的标识符。</returns>
    public static long GetUniqueService<T>()
        where T : ServiceBase
    {
        var serviceId = ServiceManager.Instance.GetUniqueService<T>();
        return serviceId;
    }

    /// <summary>
    /// 关闭指定服务 ID 列表中的所有服务。
    /// </summary>
    /// <param name="serviceIdList">要关闭的服务 ID 列表。</param>
    public static void CloseService(List<long> serviceIdList)
    {
        foreach (long serviceId in serviceIdList)
        {
            var proxy = GetServiceProxy<ServiceBaseProxy>(serviceId);
            proxy.ExecuteServiceClose();
        }
    }

    /// <summary>
    /// 获取指向指定服务的强类型代理实例。
    /// </summary>
    /// <typeparam name="T">代理接口类型，必须实现 <see cref="IServiceProxy"/>。</typeparam>
    /// <param name="serviceId">目标服务的唯一标识符。</param>
    /// <returns>强类型代理实例。</returns>
    public static T GetServiceProxy<T>(long serviceId)
        where T : class, IServiceProxy
    {
        return (T.Create(serviceId) as T)!;
    }

    /// <summary>
    /// 获取指向指定服务的强类型代理实例，并注入自定义的 <see cref="IMessageInvoker"/>。
    /// </summary>
    /// <typeparam name="T">代理接口类型，必须实现 <see cref="IServiceProxy"/>。</typeparam>
    /// <param name="serviceId">目标服务的唯一标识符。</param>
    /// <param name="messageInvoker">用于发送消息的自定义调用器。</param>
    /// <returns>强类型代理实例。</returns>
    public static T GetServiceProxy<T>(long serviceId, IMessageInvoker messageInvoker)
        where T : class, IServiceProxy
    {
        return (T.Create(serviceId, messageInvoker) as T)!;
    }

    /// <summary>
    /// 将指定的操作投递到当前 Actor 的消息队列中，在下一帧执行。
    /// </summary>
    /// <param name="action">要执行的操作。</param>
    public static void NextFrame(Action action)
    {
        SynchronizationContextHelper
        .GetSynchronization<ActorSynchronizationContext>()!
        .Post(action);
    }

    /// <summary>
    /// 创建一个下一帧完成的 <see cref="ZFTask"/>，可用于异步等待下一帧。
    /// </summary>
    /// <returns>一个将在下一帧完成的 <see cref="ZFTask"/>。</returns>
    public static ZFTask NextFrame()
    {
        var task = ZFTask.CreateZFTask();
        NextFrame(task.SetResult);
        return task;
    }

    ///// <summary>
    ///// 为指定名称绑定一个服务 ID，便于通过名称查找服务。
    ///// </summary>
    ///// <param name="name">服务名称。</param>
    ///// <param name="serviceId">服务 ID。</param>
    ///// <returns>如果设置成功返回 <c>true</c>，否则返回 <c>false</c>。</returns>
    //public static bool SetService(string name, long serviceId)
    //{
    //    return ServiceManager.Instance.SetService(name, serviceId);
    //}

    //internal static long GetServiceIdByName(string name)
    //{
    //    return ServiceManager.Instance.GetServiceIdByName(name);
    //}

    internal static void WriterHeander(ref MessagePackWriter writer,
        long serviceId, long msgSeq, int proto,
        long headerData)
    {
        writer.Write(serviceId);
        writer.Write(msgSeq);
        writer.Write(proto);
        if (headerData != 0)
        {
            writer.Write(headerData);
        }
        writer.Flush();
    }

    internal static (long, long, int) ReadHeander(ref MessagePackReader reader)
    {
        var serviceId = reader.ReadInt64();
        var mesSeq = reader.ReadInt64();
        var proto = reader.ReadInt32();
        return (serviceId, mesSeq, proto);
    }
}