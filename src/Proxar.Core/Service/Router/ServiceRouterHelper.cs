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


using Proxar.Network;
using Proxar.ServiceCore.Interfaces;
namespace Proxar.ServiceCore.Router;


internal static class ServiceRouterHelper
{

    internal static long GetWorkerId(long serviceId)
    {
        var idStruct = ServiceConfig.SnowflakeInfo.ParseId(serviceId);
        return idStruct.WorkerId;
    }

    internal static void SendNet(long serviceId, IServiceMessage serviceMessage)
    {
        var workerId = GetWorkerId(serviceId);
        var clusterBusServiceId = Service.GetUniqueService<ClusterBus>();
        var serviceProxy = Service.GetServiceProxy<ClusterBusProxy>(clusterBusServiceId);
        var netMessage = serviceMessage as INetMessage;
        serviceProxy.Raw.Send(workerId, netMessage!);
    }



}