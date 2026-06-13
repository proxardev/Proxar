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
using Proxar.IdGenerator;
using Proxar.ServiceCore.Interfaces;
using Proxar.ServiceSynchronizationContext;
using Proxar.Threading;

namespace Proxar.ServiceCore;

public abstract partial class ServiceBase
{
    private long serviceId;
    private Queue<IServiceMessage>? queue = new Queue<IServiceMessage>(100);
    private Queue<IServiceMessage> queue0 = new Queue<IServiceMessage>();
    private ActorSynchronizationContext actorSynchronizationContext;

    private bool inGlobalQueue = false;
    private SpinLockScope msgSpinLock { get; } = new SpinLockScope();
    //private IIdGenerator<long> msgSeqGenerator { get; set; } = new Int64IdGenerator() { InitValue = 1 };
    //private IIdGenerator<long> msgSeqGenerator { get; set; } = SnowflakeIdHelper.CreateSnowflakeIdGenerator();
    private IIdGenerator<long> msgSeqGenerator { get; set; } = Game.Instance.IdGenerator2;

    private ServiceStatue serviceStatue = ServiceStatue.None;

    public IServiceGroup ServiceGroup { get; init; }


    public ServiceBase()
    {
        actorSynchronizationContext = new ActorSynchronizationContext(this);
        ServiceGroup = ActorThreadScope.ServiceGroup;
    }

    protected internal long NewMessageSeq()
    {
        return msgSeqGenerator.NewId();
    }


    public void SetServiceId(long serviceId)
    {
        this.serviceId = serviceId;
    }

    public long GetServiceId()
    {
        return this.serviceId;
    }

    public void PushMessage(IServiceMessage message)
    {
        this.msgSpinLock.Execute(this.PushMessage2, message);
    }

    private void PushMessage2(IServiceMessage message)
    {
        if (this.queue == null)
        {
            return;
        }
        this.queue.Enqueue(message);
        this.CheckPushGlobalQueue();
    }

    internal void PushQueue0Message(IServiceMessage message)
    {
        this.msgSpinLock.Execute(this.PushQueue0Message2, message);
    }

    private void PushQueue0Message2(IServiceMessage message)
    {
        if (this.queue0 == null)
        {
            return;
        }
        this.queue0.Enqueue(message);
        this.CheckPushGlobalQueue();
    }

    private void CheckPushGlobalQueue()
    {
        if (inGlobalQueue)
        {
            return;
        }
        inGlobalQueue = true;
        ServiceManager.Instance.PushGlobalQueue(serviceId);
    }

    internal void ClearAllMessage()
    {
        this.msgSpinLock.Execute(this.ClearAllMessage2);
    }

    private void ClearAllMessage2()
    {
        this.InvalidQueue12();
        this.queue0.Clear();
        if (this.queue == null)
        {
            return;
        }
        var count = this.queue.Count;
        for (int i = 0; i < count; i++)
        {
            this.queue.Dequeue();
        }
    }

    internal void InvalidQueue1()
    {
        this.msgSpinLock.Execute(this.InvalidQueue12);
    }

    private void InvalidQueue12()
    {
        this.queue = null;
    }

    private void SetServiceStatueRunning()
    {
        this.serviceStatue = ServiceStatue.Running;
    }

    private void SetServiceStatueWaitClose()
    {
        this.serviceStatue = ServiceStatue.WaitClose;
    }

    private void SetServiceStatueClose()
    {
        this.serviceStatue = ServiceStatue.Close;
    }

    public ServiceStatue GetServiceStatue()
    {
        return this.serviceStatue;
    }

    public bool IsServiceStatue(ServiceStatue serviceStatue)
    {
        return this.serviceStatue == serviceStatue;
    }
}
