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


using HWT;
namespace Proxar.Timer;

internal class TimerTaskObject : BaseObject, TimerTask
{
    private long id;
    private Action? action;
    private Action<long>? onInvoke;
    private bool cancel = false;
    private bool clearInfo = true;

    public TimerTaskObject(long id, Action action, Action<long> onInvoke,
        bool runClearInfo = true) : base()
    {
        this.id = id;
        this.action = action;
        this.onInvoke = onInvoke;
        this.clearInfo = runClearInfo;
    }

    public void Run(HWT.Timeout timeout)
    {
        var synchronizationContext = this.GetSynchronization();
        if (synchronizationContext == null)
        {
            this.Run2();
            return;
        }
        synchronizationContext.Post(this.Run2);
    }

    public void Run2()
    {
        if (cancel)
        {
            return;
        }
        var action = this.action;
        var onInvoke = this.onInvoke;
        if (this.clearInfo)
        {
            this.action = null;
            this.onInvoke = null;
        }
        var id = this.id;
        onInvoke?.Invoke(id);
        if (action == null)
        {
            return;
        }
        action.Invoke();
    }

    public void Cancel()
    {
        this.cancel = true;
        this.action = null;
        this.onInvoke = null;
    }

    public bool IsCancel()
    {
        return this.cancel;
    }
}