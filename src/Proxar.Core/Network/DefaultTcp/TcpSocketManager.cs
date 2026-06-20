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


using Proxar.Core;
using Proxar.Network.Interfaces;
using Proxar.Tasks;
namespace Proxar.Network;


public class TcpSocketManager : Singleton<TcpSocketManager>
{
    internal TcpServerListener InternalNetworkTcpServiceManager { get; private set; } = null!;
    internal TcpServerListener ExternalNetworkdTcpServiceManager { get; private set; } = null!;


    public async ZFTask InitializeInternalNetworkSocketService(string ip, int port)
    {
        InternalNetworkTcpServiceManager = new TcpServerListener();
        InternalNetworkTcpServiceManager.OnConnected = OnInternalClusterChannelConnected;
        await InternalNetworkTcpServiceManager.StartAsync(ip, port);
    }

    private static void OnInternalClusterChannelConnected(IChannel channel)
    {
        TcpChannelHelper.BindInternalChannelEvents(channel);
    }

    public async ZFTask InitializeExternalNetworkSocketService(string ip, int port, OnChannelConnectedCallback? onChannelConnectedCallback)
    {
        ExternalNetworkdTcpServiceManager = new TcpServerListener();
        ExternalNetworkdTcpServiceManager.OnConnected = onChannelConnectedCallback;
        await ExternalNetworkdTcpServiceManager.StartAsync(ip, port);
    }
}