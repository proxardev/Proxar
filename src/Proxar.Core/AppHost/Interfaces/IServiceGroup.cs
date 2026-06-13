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


using Proxar.ServiceCore;
using Proxar.ServiceCore.Interfaces;
using Proxar.Tasks;

namespace Proxar.AppHost.Interfaces;

public interface IServiceGroup
{
    int GroupId { get; }

    IMessageInvoker Invoker { get; set; }

    IInternalMessageInvoker InternalInvoker { get; set; }

    IGateMessageInvoker GateMessageInvoker { get; set; }

    IMessageInvoker ExternalProxyInvoker { get; set; }

    string Flag { get; set; }


    void AddServiceGroupStartAction(Func<long, ZFTask> func);

    List<Func<long, ZFTask>> GetServiceGroupStartActions();

    void ServiceGroupExecute(Action action)
    {
        var group = ActorThreadScope.ServiceGroup;
        try
        {
            ActorThreadScope.ThreadServiceGroup = this;
            action.Invoke();
        }
        catch (Exception)
        {
            ActorThreadScope.ThreadServiceGroup = group;
            throw;
        }
        finally
        {
            ActorThreadScope.ThreadServiceGroup = group;
        }
    }
}