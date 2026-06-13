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


using Proxar.AppHost.Interfaces;
using Proxar.ServiceCore.GateWay;
using Proxar.ServiceCore.Interfaces;
using Proxar.ServiceCore.Router;
using Proxar.Tasks;

namespace Proxar.AppHost;

public class DefaultServiceGroup : IServiceGroup
{
    public int GroupId { get; }

    public IMessageInvoker Invoker { get; set; }

    public IInternalMessageInvoker InternalInvoker { get; set; }
    public IGateMessageInvoker GateMessageInvoker { get; set; }
    public IMessageInvoker ExternalProxyInvoker { get; set; }
    public string Flag { get; set; } = string.Empty;

    private readonly List<Func<long, ZFTask>> startActions = new List<Func<long, ZFTask>>();


    internal DefaultServiceGroup(int clusterId)
    {
        GroupId = clusterId;
        Invoker = new ServiceRouter();
        InternalInvoker = new ServiceInternalRouter();

        GateMessageInvoker = new GateWay();

        ExternalProxyInvoker = new Server2ExternalProxyMessageInvoker();
    }

    public void AddServiceGroupStartAction(Func<long, ZFTask> func)
    {
        startActions.Add(func);
    }

    public List<Func<long, ZFTask>> GetServiceGroupStartActions()
    {
        return startActions;
    }
}