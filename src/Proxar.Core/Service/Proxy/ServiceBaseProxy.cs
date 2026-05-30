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

public partial class ServiceBaseProxy : IServiceProxy
{
    public long ServiceId { get; }

    public IMessageInvoker MessageInvoker => throw new NotImplementedException();

    private IMessageInvoker messageInvoker { get; } = null!;

    public ServiceBaseProxy(long serviceId)
    {
        this.ServiceId = serviceId;
    }

    public ServiceBaseProxy(long serviceId, IMessageInvoker messageInvoker) : this(serviceId)
    {
        this.messageInvoker = messageInvoker;
    }

    public long GetServiceId()
    {
        return this.ServiceId;
    }

    public static IServiceProxy Create(long serviceId)
    {
        return new ServiceBaseProxy(serviceId);
    }

    public static IServiceProxy Create(long serviceId, IMessageInvoker messageInvoker)
    {
        throw new NotImplementedException();
    }
}