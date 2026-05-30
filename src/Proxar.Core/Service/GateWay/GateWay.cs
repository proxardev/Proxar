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

namespace Proxar.ServiceCore.GateWay;


public class GateWay : IGateMessageInvoker
{
    private Lazy<GateWayServiceProxy> gateWayServiceProxy { get; } = new Lazy<GateWayServiceProxy>(CreateGateWayServiceProxy, true);


    private static GateWayServiceProxy CreateGateWayServiceProxy()
    {
        var serviceId = Service.GetUniqueService<GateWayService>();
        var proxy = Service.GetServiceProxy<GateWayServiceProxy>(serviceId);
        return proxy;

    }

    public void AddChannel(IChannel channel)
    {
        gateWayServiceProxy.Value.Raw.AddChannel(channel);
    }

    public void RemoveChannel(IChannel channel)
    {
        gateWayServiceProxy.Value.Raw.RemoveChannel(channel);
    }

    public void ForwardServiceMessage2Client(long fromServiceId, long targetId, long proxyId, long rpcId, int proto, byte[] payloadBytes)
    {
        gateWayServiceProxy.Value.ForwardServiceMessage2Client(fromServiceId, targetId, proxyId, rpcId, proto, payloadBytes);
    }

    public void ReceiveChannelData(IChannel channel, byte[] data)
    {
        var x = ActorThreadScope.HostCluster;
        gateWayServiceProxy.Value.Raw.ReceiveChannelData(channel, data);
    }

    public void RemoveTarget2Channel(long targetId)
    {
        gateWayServiceProxy.Value.RemoveTarget2Channel(targetId);
    }

    public void SetTarget2Channel(long targetId, long channelId)
    {
        gateWayServiceProxy.Value.SetTarget2Channel(targetId, channelId);
    }

    public void SetTargetServiceMapping(long targetId, long type, long serviceId)
    {
        gateWayServiceProxy.Value.SetTargetServiceMapping(targetId, type, serviceId);
    }
}