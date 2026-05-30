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


using Proxar.ActorSingletonCore.Interfaces;
using Proxar.IdGenerator;
using Proxar.ServiceCore;
using Proxar.ServiceCore.Message;
using Proxar.Tasks;
namespace Proxar.ServiceSynchronizationContext;

public sealed partial class ActorSynchronizationContext : AbstractSynchronizationContext
{
    private ServiceBase attachedService;
    private Int64IdGenerator timerInt64IdGenerator = new Int64IdGenerator() { InitValue = 1 };
    private HashSet<ZFTask> pendingZFTask = new HashSet<ZFTask>();

    public List<IActorSingleton?> ActorSingletonList
        = new List<IActorSingleton?>(100);


    public ActorSynchronizationContext(ServiceBase service)
    {
        this.attachedService = service;
    }

    public ServiceBase GetService()
    {
        return attachedService;
    }

    internal void InitActorSynchronizationContext()
    {
    }

    public override void ExecuteSyncAction()
    {
        throw new NotImplementedException();
    }

    public override void Post(SendOrPostCallback d, object? state)
    {
        if (this.attachedService == null)
        {
            return;
        }
        var msg = new ContextActionMessage(d, state);
        msg.Init();
        this.attachedService.PushMessage(msg);
    }

    public override void Post(Action action)
    {
        if (this.attachedService == null)
        {
            return;
        }
        var msg = new ContextActionMessage(action);
        msg.Init();
        this.attachedService.PushMessage(msg);
    }

    public Int64IdGenerator GetTimerIdGenerator()
    {
        return timerInt64IdGenerator;
    }

    public void CloseServiceHandler()
    {
        foreach (var task in pendingZFTask)
        {
            task.Cancel();
            //task.straceInfo = TraceHelper.TraceInfo();
        }
        //ProxarLogger.Console($"close {this.service.GetServiceId()}");
        pendingZFTask.Clear();
        this.attachedService?.ClearAllMessage();
        this.attachedService = null!;
        foreach (var singleInstance in ActorSingletonList)
        {
            singleInstance?.Dispose();
        }
        ActorSingletonList.Clear();
    }

    public void RegisterPenddingZFTask(ZFTask task)
    {
        pendingZFTask.Add(task);
    }

    public void UnRegisterPenddingZFTask(ZFTask task)
    {
        pendingZFTask.Remove(task);
    }

    public void BindThread()
    {
        ActorThreadScope.BindActor();
    }
}