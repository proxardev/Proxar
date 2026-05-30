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

public partial class ServiceBaseProxyExternal : IExternalProxy
{
    public long ServiceId { get; }

    public IMessageInvoker MessageInvoker => GetMessageInvoker();

    private IMessageInvoker messageInvoker { get; } = null!;

    public ServiceBaseProxyExternal(long serviceId)
    {
        this.ServiceId = serviceId;
    }

    public ServiceBaseProxyExternal(long serviceId, IMessageInvoker messageInvoker) : this(serviceId)
    {
        this.messageInvoker = messageInvoker;
    }

    private IMessageInvoker GetMessageInvoker()
    {
        if (messageInvoker == null)
        {
            return ExternalProxyConfig.MessageInvoker;
        }
        return messageInvoker;
    }


    public static IExternalProxy Create(long serviceId)
    {
        return new ServiceBaseProxyExternal(serviceId);
    }

    public static IExternalProxy Create(long serviceId, IMessageInvoker messageInvoker)
    {
        return new ServiceBaseProxyExternal(serviceId, messageInvoker);
    }
}