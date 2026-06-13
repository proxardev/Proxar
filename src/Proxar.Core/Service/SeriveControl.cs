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

public static partial class Service
{

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

    public static long CreateUniqueService<T>()
        where T : ServiceBase, new()
    {
        var serviceId = ServiceManager.Instance.CreateUniqueService<T>();
        Send(serviceId, ProtoBase.ServiceStart, default(ServiceBootstrapper));
        return serviceId;
    }

    public static long GetUniqueService<T>()
        where T : ServiceBase
    {
        var serviceId = ServiceManager.Instance.GetUniqueService<T>();
        return serviceId;
    }

    public static void CloseService(List<long> serviceIdList)
    {
        foreach (long serviceId in serviceIdList)
        {
            var proxy = GetServiceProxy<ServiceBaseProxy>(serviceId);
            proxy.ExecuteServiceClose();
        }
    }

    public static T GetServiceProxy<T>(long serviceId)
        where T : class, IServiceProxy
    {
        return (T.Create(serviceId) as T)!;
    }

    public static T GetServiceProxy<T>(long serviceId, IMessageInvoker messageInvoker)
        where T : class, IServiceProxy
    {
        return (T.Create(serviceId, messageInvoker) as T)!;
    }

    public static void NextFrame(Action action)
    {
        SynchronizationContextHelper
        .GetSynchronization<ActorSynchronizationContext>()!
        .Post(action);
    }

    public static ZFTask NextFrame()
    {
        var task = ZFTask.CreateZFTask();
        NextFrame(task.SetResult);
        return task;
    }



    public static bool SetService(string name, long serviceId)
    {
        return ServiceManager.Instance.SetService(name, serviceId);
    }

    internal static long GetServiceIdByName(string name)
    {
        return ServiceManager.Instance.GetServiceIdByName(name);
    }

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