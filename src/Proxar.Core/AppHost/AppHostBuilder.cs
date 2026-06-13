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


using Proxar.AppHost.Interfaces;
using Proxar.Core;
using Proxar.IdGenerator;
using Proxar.ServiceCore;

namespace Proxar.AppHost;

public partial class AppHostBuilder : Singleton<AppHostBuilder>
{
    private List<IServiceGroup> serviceGroups = new List<IServiceGroup>();
    public IServiceGroup DefaultServiceGroup { get; private set; } = null!;
    private List<Action> buildActions = new List<Action>();
    private IIdGenerator<int> serviceGroupIdGenerator = new Int32IdGenerator();

    private List<Action<IServiceGroup>> defaultServiceGroupConfigureActions = new List<Action<IServiceGroup>>();

    private Action mainThreadAction = StartHelper.MainThreadLoop;

    private string[] commandLineArgs = null!;

    private bool alreadyBuild = false;


    public void Build()
    {
        if (alreadyBuild)
        {
            return;
        }
        alreadyBuild = true;

        Game.Instance.ConfigureAppOptions(commandLineArgs);


        buildActions.Add(CreateDefaultServiceGroup);
        this.ConfigureDefaultServiceGroup(serviceGroup =>
            {
                if (serviceGroup.Flag == null || serviceGroup.Flag == "")
                {
                    serviceGroup.Flag = "Server";
                }
            }
        );

        foreach (var action in buildActions)
        {
            action.Invoke();
        }

        ActorThreadScope.ThreadServiceGroup = DefaultServiceGroup;
    }

    /// <summary>
    /// 创建额外的主机集群，默认主机集群会在Build方法中自动创建，如果需要创建额外的主机集群，可以调用此方法，并在action中配置主机集群的服务和功能。
    /// </summary>
    /// <param name="action"></param>
    /// <returns></returns>
    public AppHostBuilder CreateServiceGroup(Action<IServiceGroup> action)
    {
        var action2 = () => CreateServiceGroup2(action);
        buildActions.Add(action2);
        return this;
    }

    private void CreateServiceGroup2(Action<IServiceGroup> action)
    {
        IServiceGroup serviceGroup = new DefaultServiceGroup(serviceGroupIdGenerator.NewId());
        serviceGroups.Add(serviceGroup);
        action.Invoke(serviceGroup);
        serviceGroup.ServiceGroupExecute(() =>
        {
            var serviceGroup2 = ActorThreadScope.ServiceGroup;
            ServiceManager.Instance.RegisterServiceGroup(serviceGroup2);
            StartHelper.CreateMainService(serviceGroup2.GetServiceGroupStartActions());
        });
    }

    /// <summary>
    /// 默认启动器
    /// </summary>
    private void CreateDefaultServiceGroup()
    {
        IServiceGroup serviceGroup = new DefaultServiceGroup(serviceGroupIdGenerator.NewId());
        DefaultServiceGroup = serviceGroup;
        serviceGroups.Add(serviceGroup);
        foreach (var action in defaultServiceGroupConfigureActions)
        {
            action.Invoke(serviceGroup);
        }
        serviceGroup.ServiceGroupExecute(() =>
        {
            StartHelper.DefaultStartUp(serviceGroup.GetServiceGroupStartActions(), mainThreadAction);
        });
    }

    public AppHostBuilder ConfigureDefaultServiceGroup(Action<IServiceGroup> action)
    {
        defaultServiceGroupConfigureActions.Add(action);
        return this;
    }

    public AppHostBuilder ConfigureMainThreadAction(Action action)
    {
        mainThreadAction = action;
        return this;
    }

    public AppHostBuilder SetCommandLineArgs(string[] args)
    {
        commandLineArgs = args;
        return this;
    }
}