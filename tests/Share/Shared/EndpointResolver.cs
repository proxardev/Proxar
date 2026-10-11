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
using Proxar.Network.Interfaces;
using System.Collections.Concurrent;
namespace TestShared;

public class EndpointResolver : DefaultEndpointResolver, IEndpointResolver
{
    private ConcurrentDictionary<long, (string, int)> cache = new();


    (string Ip, int Port) IEndpointResolver.Resolve(long workerId)
    {
        if (cache.TryGetValue(workerId, out var ret))
        {
            return ret;
        }
        var (ip, port) = GetIpPortDefault(workerId);
        port = TestContextHelper.NewPort(port);
        ret = (ip, port);
        cache.TryAdd(workerId, ret);
        return ret;
    }

}