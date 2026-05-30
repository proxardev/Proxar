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


using Proxar.ServiceCore.Dispatch;
using Proxar.ServiceCore.Interfaces;


namespace Proxar.ServiceCore;


public abstract partial class ServiceBase
{
    private int willConsumeCount = 0;
    private List<IServiceMessage> threadMessageList = new List<IServiceMessage>(50);
    private ServiceWorkThread? serviceWorker = null;


    internal bool ThreadEnter(ServiceWorkThread worker)
    {
        this.serviceWorker = worker;
        var succ = this.msgSpinLock.Execute(this.CalThreadConsumeCount2);
        if (!succ)
        {
            return false;
        }
        var x = ServiceManager.Instance;
        SynchronizationContext.SetSynchronizationContext(actorSynchronizationContext);
        actorSynchronizationContext.BindThread();
        return true;

    }

    private bool CalThreadConsumeCount2()
    {
        var count0 = this.queue0.Count;
        if (count0 != 0)
        {
            this.willConsumeCount = count0;
            return true;
        }
        if (this.queue == null)
        {
            return false;
        }
        if (this.serviceWorker!.DispatchCount != 0)
        {
            this.willConsumeCount = this.serviceWorker.DispatchCount;
        }
        else
        {
            var count = this.queue.Count;
            count = Math.Max(1, count);
            this.willConsumeCount = (int)(1 + count * this.serviceWorker.Factor / 100.0);
        }
        return true;
    }


    internal bool ThreadConsumeMessage()
    {
        for (int i = 0; i < this.willConsumeCount; i++)
        {
            var succ = this.msgSpinLock.Execute(this.PopMessage2Cur);
            if (succ == false)
            {
                return false;
            }
            this.Dispatch();
        }
        return true;
    }

    private bool PopMessage2Cur()
    {
        if (this.queue0.Count == 0
            && (this.queue == null || this.queue.Count == 0))
        {
            this.inGlobalQueue = false;
            return false;
        }

        if (this.queue0.Count != 0)
        {
            this.curMessage = this.queue0.Dequeue();
            return true;
        }

        this.curMessage = this.queue!.Dequeue();
        return true;
    }
}