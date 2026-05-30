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

namespace Proxar.ServiceCore.Interfaces;


public interface IGateMessageInvoker
{


    void AddChannel(IChannel channel);
    void RemoveChannel(IChannel channel);

    void SetTarget2Channel(long targetId, long channelId);

    void RemoveTarget2Channel(long targetId);

    void SetTargetServiceMapping(long targetId, long type, long serviceId);

    void ReceiveChannelData(IChannel channel, byte[] data);

    void ForwardServiceMessage2Client(long fromServiceId, long targetId, long proxyId, long rpcId, int proto, byte[] payloadBytes);




}