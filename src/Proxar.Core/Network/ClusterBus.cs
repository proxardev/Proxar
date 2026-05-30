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


using Proxar.Core.Extensions;
using Proxar.ServiceCore;
using Proxar.Tasks;
namespace Proxar.Network;


internal partial class ClusterBus : ServiceBase
{
    private Dictionary<long, IChannel> channels = new Dictionary<long, IChannel>();
    private Dictionary<long, List<ZFTask>> waitCreate = new Dictionary<long, List<ZFTask>>();

    private ZFTask TryGetOrCreateChannel(long workerId)
    {
        if (channels.ContainsKey(workerId))
        {
            return ZFTask.CompletedTask;
        }
        var create = !waitCreate.ContainsKey(workerId);
        var task = new ZFTask();
        if (create)
        {
            waitCreate[workerId] = new List<ZFTask>();
        }
        waitCreate[workerId].Add(task);
        if (create)
        {
            TryConnect(workerId).Coroutine();
        }
        return task;
    }

    private async ZFTask TryConnect(long workerId)
    {
        var (ip, port) = ChannelConfig.EndpointResolver.Resolve(workerId);
        var channel = await ChannelConfig.ChannelFactory.CreateChannel(ip, port);
        channels[workerId] = channel;
        var tasks = waitCreate!.Get(workerId);
        if (tasks != null)
        {
            foreach (var task in tasks)
            {
                task.SetResult();
            }
        }
    }

    [ServiceMethod(1)]
    [ServiceMethodAction(true, true)]
    private async ZFTask Send(long workerId, INetMessage message)
    {
        await TryGetOrCreateChannel(workerId);
        var channel = channels[workerId];
        channel.Send(message);
    }

}