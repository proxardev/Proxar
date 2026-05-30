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


using Proxar.Tasks;
using TouchSocket.Core;
using TouchSocket.Sockets;

namespace Proxar.Network;


public class TcpServerListener
{
    private TcpService tcpService { get; set; } = null!;

    public OnChannelConnectedCallback? OnConnected { get; set; } = null!;

    public TcpServerListener()
    {
        tcpService = new TcpService();
        tcpService.Connected = (client, e) =>
        {
            var channel = TouchSocketChannel.FromSession(client);
            OnConnected?.Invoke(channel);
            return Task.CompletedTask;
        };
    }


    public void Stop()
    {
        tcpService.SafeDispose();
    }


    public async ZFTask StartAsync(string ip, int port)
    {
        var config = new TouchSocketConfig()
            .SetListenIPHosts(new IPHost($"{ip}:{port}"));

        config.SetTcpDataHandlingAdapter(() =>
            {
                return new FixedHeaderPackageAdapter()
                {
                    FixedHeaderType = FixedHeaderType.Int
                };
            }
            )
            .ConfigurePlugins(a =>
            {
                a.Add<TcpServiceReceivedPlugin>();
            });

        // 启动服务器
        await tcpService.SetupAsync(config);
        await tcpService.StartAsync();
    }

    public async Task CloseServiceAsync()
    {
        if (tcpService == null)
        {
            return;
        }
        await tcpService.StopAsync();
    }

}

