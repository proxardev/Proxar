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
using Proxar.ServiceCore.Interfaces;

namespace Proxar.ServiceCore.GateWay;

/// <summary>
/// 服务器向外部代理发送消息的消息调用器。
/// proxyId 是外部服务的代理Id，targetId是外部client等的唯一标识
/// </summary>
public class Server2ExternalProxyMessageInvoker : IMessageInvoker
{
    public void Send(long proxyId, long targetId, IServiceMessage serviceMessage)
    {
        var rpcId = serviceMessage.GetSeq();
        var proto = serviceMessage.GetProto();
        var payload = serviceMessage.GetPayloadReadOnlyMemory();

        var fromServiceId = serviceMessage.GetFromServiceId();

        var x = ActorThreadScope.ServiceGroup;
        Game.Instance.GateMessageInvoker.ForwardServiceMessage2Client(fromServiceId, targetId, proxyId, rpcId, proto, payload.ToArray());
        serviceMessage.Dispose();
    }

    public void SendLocal(long ProxyId, long targetId, IServiceMessage serviceMessage)
    {
        throw new NotImplementedException();
    }
}