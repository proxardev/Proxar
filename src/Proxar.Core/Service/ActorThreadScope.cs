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


using Proxar.ActorSingletonCore.Interfaces;
using Proxar.AppHost;
using Proxar.AppHost.Interfaces;
using Proxar.ServiceSynchronizationContext;
using Proxar.Utilities;

namespace Proxar.ServiceCore;


internal static class ActorThreadScope
{


    [ThreadStatic]
    public static List<IActorSingleton?> ActorSingletonList = null!;

    [ThreadStatic]
    public static int ExpireAt = 0;

    [ThreadStatic]
    public static ServiceBase Service = null!;

    [ThreadStatic]
    private static IHostCluster hostCluster = null!;


    public static IHostCluster HostCluster => hostCluster ?? GetHostCluster();

    private static IHostCluster GetHostCluster()
    {
        return AppHostBuilder.Instance.DefaultHostCluster;
    }

    internal static IHostCluster ThreadHostClusterSet
    {
        set
        {
            hostCluster = value;
        }
    }


    [ThreadStatic]
    public static long OperateServiceId = 0;


    public const int CacheLiveTime = 60 * 5;
    public const int CheckReleaseMilliTime = 1000 * 60 * 5;


    public static void BindActor()
    {
        var actorSynchronizationContext = SynchronizationContextHelper
            .GetSynchronization<ActorSynchronizationContext>()!;
        ActorSingletonList = actorSynchronizationContext.ActorSingletonList;
        Service = actorSynchronizationContext.GetService();

        ExpireAt = TimeHelper.GetSecond() + CacheLiveTime;

        OperateServiceId = Service.GetServiceId();
        hostCluster = Service.HostCluster;

    }

    public static void UnbindActor()
    {
        ActorSingletonList = null!;
        Service = null!;
        ExpireAt = 0;
        OperateServiceId = 0;
    }

}