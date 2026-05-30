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
using Proxar.Tasks;
namespace Proxar.Network;



public static class NetWorkV2Helper
{
    public static async ZFTask Configure(OnChannelConnectedCallback? onClientConnectedCallback)
    {
        var ip = Game.Instance.AppOptions.Ip;
        var port = Game.Instance.AppOptions.Port;
        if (ip.Count() != 0 && port != 0)
        {
            await TcpSocketManager.Instance.InitializeInternalNetworkSocketService(ip, port);
        }
        var clientIp = Game.Instance.AppOptions.ClientIp;
        var clientPort = Game.Instance.AppOptions.ClientPort;
        if (clientIp.Count() != 0 && clientPort != 0)
        {
            await TcpSocketManager.Instance.InitializeExternalNetworkSocketService(clientIp, clientPort, onClientConnectedCallback);
        }
    }
}
