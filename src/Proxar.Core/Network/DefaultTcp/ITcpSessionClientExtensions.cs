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


using Proxar.Network.Interfaces;
using TouchSocket.Core;
using TouchSocket.Sockets;

namespace Proxar.Network;

public static class ITcpSessionClientExtensions
{
    private static readonly DependencyProperty<IChannel> ChannelProperty = new DependencyProperty<IChannel>(nameof(ChannelProperty), null!);

    public static IChannel GetChannel(this ITcpClient client)
    {
        return client.GetValue(ChannelProperty);
    }

    internal static void SetChannel(this ITcpClient client, IChannel value)
    {
        client.SetValue(ChannelProperty, value);
    }

    public static IChannel GetChannel(this ITcpSession client)
    {
        return client.GetValue(ChannelProperty);
    }

    public static T GetChannel<T>(this ITcpSession client)
        where T : class, IChannel
    {
        var channel = client.GetValue(ChannelProperty);
        var tarTypeChannel = (channel as T)!;
        return tarTypeChannel;
    }

    internal static void SetChannel(this ITcpSession client, IChannel value)
    {
        client.SetValue(ChannelProperty, value);
    }
}