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


using Proxar.IdGenerator.Interfaces;
using Proxar.IdGenerator.SnowflakeId;
using Proxar.Logging;
using Proxar.ServiceCore;
using Proxar.ServiceCore.GateWay;
using Proxar.Tasks;

namespace Proxar.AppHost;


internal static class StartHelper
{

    private static async ZFTask CheckConfigDefault(long serviceId)
    {
        ProxarHost.Instance.SnowflakeIdGenerator = CreateSnowflakeIdGenerator();
        await ZFTask.CompletedTask;
    }

    private static IIdGenerator<long> CreateSnowflakeIdGenerator()
    {
        var generator = new SnowflakeIdGenerator(ProxarHost.Instance.AppOptions.WorkerId, ServiceConfig.SnowflakeInfo);
        return generator;
    }

    private static void CreateHelperService()
    {
        Service.CreateUniqueService<GateWayService>();
        Service.CreateUniqueService<LauncherService>();
    }

    private static void EnsureServerConfig()
    {
        ArgumentNullException.ThrowIfNull(ZFTaskConfig.UnhandledExceptionHandler);
    }

    private static void InitConfig()
    {
        string currentDirectory = Environment.CurrentDirectory;
        ProxarLogger.Info($"current work dir: {currentDirectory}");
        ProxarLogger.Console($"current work dir: {currentDirectory}");

        if (ZFTaskConfig.UnhandledExceptionHandler == null)
        {
            ZFTaskConfig.UnhandledExceptionHandler = (exception) =>
            {
                ProxarLogger.Error(exception);
            };
        }

        ZFTask.NextFrameHander = Service.NextFrame;

        ProxarHost.Instance.GameInit();
    }

    internal static long CreateMainService(List<Func<long, ZFTask>> funcs)
    {
        var serviceGroup = ActorThreadScope.ServiceGroup;
        ServiceManager.Instance.RegisterServiceGroup(serviceGroup);
        var boot = new ServiceBootstrapper(funcs);
        var serviceId = Service.CreateService<MainService>(boot);
        CreateHelperService();
        return serviceId;
    }

    internal static void DefaultStartUp(List<Func<long, ZFTask>> funcs, Action? mainThreadLoop = null)
    {
        funcs.Add(CheckConfigDefault);
        var serviceGroup = ActorThreadScope.ServiceGroup;
        ServiceManager.Instance.RegisterServiceGroup(serviceGroup);

        string currentDirectory = Environment.CurrentDirectory;
        ProxarLogger.Info($"cur start dir: {currentDirectory}");
        ProxarLogger.Console($"cur start dir: {currentDirectory}");

        InitConfig();
        EnsureServerConfig();

        var servierId = CreateMainService(funcs);

        if (mainThreadLoop == null)
        {
            mainThreadLoop = MainThreadLoop;
        }
        mainThreadLoop.Invoke();
    }

    internal static void MainThreadLoop()
    {
        while (true)
        {
            Thread.Sleep(1);
        }

    }
}