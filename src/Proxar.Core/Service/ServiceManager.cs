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


using Proxar.AppHost;
using Proxar.AppHost.Interfaces;
using Proxar.Core;
using Proxar.IdGenerator.Interfaces;
using Proxar.IdGenerator.SnowflakeId;
using Proxar.ServiceCore.Dispatch;
using System.Buffers;
using System.Collections.Concurrent;
namespace Proxar.ServiceCore;

internal class ServiceManager : SingletonFast<ServiceManager>
{
    private ConcurrentDictionary<long, ServiceBase> serviceDict = new ConcurrentDictionary<long, ServiceBase>();
    private ConcurrentDictionary<long, ServiceBase> waitClearServiceDict = new ConcurrentDictionary<long, ServiceBase>();
    private ConcurrentQueue<long> serviceQueue = new ConcurrentQueue<long>();
    //private Queue<long> serviceQueue2 = new Queue<long>();
    //private SpinLockScope lockScope = new SpinLockScope();
    internal List<Thread> Threads = new List<Thread>();
    //private Int64IdSafeGenerator idGenerator = new Int64IdSafeGenerator() { InitValue = 1 };
    private SnowflakeIdGenerator idGenerator = CreateServiceIdGenerator();

    internal List<ServiceWorkerThread> ServiceWorkerThreads = new List<ServiceWorkerThread>();

    private ConcurrentDictionary<int, ConcurrentDictionary<string, long>> serviceMapping = new();

    private static SnowflakeIdGenerator CreateServiceIdGenerator()
    {
        var generator = new SnowflakeIdGenerator(ProxarHost.Instance.AppOptions.WorkerId, ServiceConfig.SnowflakeInfo);
        return generator;
    }

    public void AddService(long serviceId, ServiceBase service)
    {
        var succ = serviceDict.TryAdd(serviceId, service);
        if (!succ)
        {
            serviceDict.TryGetValue(serviceId, out var xxx);
            throw new Exception($"AddService error {serviceId}");
        }
    }

    public void RemoveService(long serviceId)
    {
        serviceDict.Remove(serviceId, out var service);
    }

    public void MoveToWaitClear(long serviceId)
    {
        serviceDict.Remove(serviceId, out var service);
        if (service == null)
        {
            return;
        }
        waitClearServiceDict.TryAdd(serviceId, service);
    }

    public ServiceBase? TakeWaitClearServiceAndClear(long serviceId)
    {
        waitClearServiceDict.Remove(serviceId, out var service);
        return service;
    }

    public ServiceBase? GetService(long serviceId)
    {
        serviceDict.TryGetValue(serviceId, out var service);
        return service;
    }

    internal long CreateService<T>(IIdGenerator<long> idGenerator)
        where T : ServiceBase, new()
    {
        long serviceId = idGenerator.NewId();

        return CreateService<T>(serviceId);
    }

    internal long CreateService<T>(long serviceId)
        where T : ServiceBase, new()
    {
        var service = new T();
        service.SetServiceId(serviceId);

        AddService(serviceId, service);

        return serviceId;
    }

    internal long CreateService<T>()
        where T : ServiceBase, new()
    {
        return CreateService<T>(idGenerator);
    }

    public void RegisterServiceGroup(IServiceGroup serviceGroup)
    {
        serviceMapping.TryAdd(serviceGroup.GroupId, new());
    }

    internal long CreateUniqueService<T>(long serviceId)
        where T : ServiceBase, new()
    {
        var serviceGroup = ActorThreadScope.ServiceGroup;
        var mapping = serviceMapping[serviceGroup.GroupId];
        var name = typeof(T).FullName!;
        serviceId = mapping.GetOrAdd(name, CreateUniqueService<T>, serviceId);
        return serviceId;
    }

    internal long CreateUniqueService<T>(string name, long serviceId)
        where T : ServiceBase, new()
    {
        return CreateService<T>(serviceId);
    }

    public long GetUniqueService<T>()
        where T : ServiceBase
    {
        var serviceGroup = ActorThreadScope.ServiceGroup;
        var mapping = serviceMapping[serviceGroup.GroupId];
        var name = typeof(T).FullName!;
        var succ = mapping.TryGetValue(name, out var serviceId);
        return serviceId;
    }

    public bool SetService(string name, long serviceId)
    {
        var serviceGroup = ActorThreadScope.ServiceGroup;
        var mapping = serviceMapping[serviceGroup.GroupId];
        var res = mapping.TryAdd(name, serviceId);
        return res;
    }

    public long GetServiceIdByName(string name)
    {
        var serviceGroup = ActorThreadScope.ServiceGroup;
        var mapping = serviceMapping[serviceGroup.GroupId];
        mapping.TryGetValue(name, out var serviceId);
        return serviceId;
    }

    public void PushGlobalQueue(long serviceId)
    {
        this.serviceQueue.Enqueue(serviceId);
        WorkerSignal.OnEnqueued(); // [FIX-A]
    }

    // [STATS] 诊断用：全局就绪队列深度（每秒打印一次，ConcurrentQueue.Count 是近似值）
    internal int GlobalQueueDepth => this.serviceQueue.Count;

    public long DequeueGlobalQueue()
    {
        var serviceId = 0L;
        if (serviceQueue.TryDequeue(out serviceId))
        {
            WorkerSignal.OnDequeued(); // [FIX-A]
        }
        return serviceId;
    }

    public void Start()
    {
        this.PrepareMemoryBuffer();
        // 开启线程

        int total = ServiceConfig.ThreadCount;
        var maxGroupSize = 5;
        int remaining = total;

        for (int i = 0; i < total; i++)
        {
            int groupSize = Math.Min(maxGroupSize, remaining);
            for (int j = 0; j < groupSize; j++)
            {
                var factor = (int)(1.0 / (1 << j) * 100);
                var count = 0;
                if (factor < 25)
                {
                    count = 2;
                }
                var work = new ServiceWorkerThread(factor, count);
                ServiceWorkerThreads.Add(work);
            }

            remaining -= groupSize;
            if (remaining <= 0)
            {
                break;
            }
        }


    }



    public void PrepareMemoryBuffer()
    {
        var memoryList = new List<(int, int)>()
        {
            (16, 1024*2),
            (64, 1024*4),
            (128, 1024*2),
            (256, 1024*2),
            (512, 1024*2),
            (1024 * 1, 1024),

        };

        MemoryPool<byte> pool = MemoryPool<byte>.Shared;
        var memoryRentList = new List<IMemoryOwner<byte>>();
        foreach (var item in memoryList)
        {
            for (int i = 0; i < item.Item2; i++)
            {
                memoryRentList.Add(
                    pool.Rent(item.Item1)
                    );
            }
        }
        foreach (var owner in memoryRentList)
        {
            owner.Dispose();
        }
    }
}