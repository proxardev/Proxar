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
    private List<IHostCluster> hostClusters = new List<IHostCluster>();
    public IHostCluster DefaultHostCluster { get; private set; } = null!;
    private List<Action> buildActions = new List<Action>();
    private IIdGenerator<int> hostClusterIdGenerator = new Int32IdGenerator();

    private List<Action<IHostCluster>> defaultHostClusterConfigureActions = new List<Action<IHostCluster>>();

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


        buildActions.Add(CreateDefaultHostCluster);
        this.ConfigureDefaultHostCluster(hostClusters =>
            {
                if (hostClusters.Flag == null || hostClusters.Flag == "")
                {
                    hostClusters.Flag = "Server";
                }
            }
        );

        foreach (var action in buildActions)
        {
            action.Invoke();
        }

        ActorThreadScope.ThreadHostClusterSet = DefaultHostCluster;
    }

    /// <summary>
    /// 创建额外的主机集群，默认主机集群会在Build方法中自动创建，如果需要创建额外的主机集群，可以调用此方法，并在action中配置主机集群的服务和功能。
    /// </summary>
    /// <param name="action"></param>
    /// <returns></returns>
    public AppHostBuilder CreateHostCluster(Action<IHostCluster> action)
    {
        var action2 = () => CreateHostCluster2(action);
        buildActions.Add(action2);
        return this;
    }

    private void CreateHostCluster2(Action<IHostCluster> action)
    {
        IHostCluster hostCluster = new DefaultHostCluster(hostClusterIdGenerator.NewId());
        hostClusters.Add(hostCluster);
        action.Invoke(hostCluster);
        hostCluster.ClusterExecute(() =>
        {
            StartHelper.CreateMainService(hostCluster.GetHostClusterStartActions());
        });
    }

    /// <summary>
    /// 默认启动器
    /// </summary>
    private void CreateDefaultHostCluster()
    {
        IHostCluster hostCluster = new DefaultHostCluster(hostClusterIdGenerator.NewId());
        DefaultHostCluster = hostCluster;
        hostClusters.Add(hostCluster);
        foreach (var action in defaultHostClusterConfigureActions)
        {
            action.Invoke(hostCluster);
        }
        hostCluster.ClusterExecute(() =>
        {
            StartHelper.DefaultStartUp(hostCluster.GetHostClusterStartActions(), mainThreadAction);
        });
    }

    public AppHostBuilder ConfigureDefaultHostCluster(Action<IHostCluster> action)
    {
        defaultHostClusterConfigureActions.Add(action);
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