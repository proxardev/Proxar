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
using Proxar.IdGenerator.Interfaces;
using Proxar.ServiceCore;

namespace Proxar.AppHost;

/// <summary>
/// 构建器，用于配置并启动整个 Proxar 服务应用。
/// 继承自 <see cref="Singleton{AppHostBuilder}"/>，在整个应用域中唯一。
/// 通过 <see cref="Build"/> 方法触发最终的构建与启动流程。
/// </summary>
public partial class AppHostBuilder : Singleton<AppHostBuilder>
{
    private List<IServiceGroup> serviceGroups = new List<IServiceGroup>();

    /// <summary>
    /// 获取默认的服务组。该服务组在 <see cref="Build"/> 时自动创建。
    /// </summary>
    public IServiceGroup DefaultServiceGroup { get; private set; } = null!;

    private List<Action> buildActions = new List<Action>();
    private IIdGenerator<int> serviceGroupIdGenerator = new Int32IdGenerator();
    private List<Action<IServiceGroup>> defaultServiceGroupConfigureActions = new List<Action<IServiceGroup>>();
    private Action mainThreadAction = StartHelper.MainThreadLoop;
    private string[] commandLineArgs = null!;
    private Action? appOptionsBuildAction;
    private bool alreadyBuild = false;


    /// <summary>
    /// 执行构建与启动流程。该方法只能调用一次，重复调用会被忽略。
    /// </summary>
    public void Build()
    {
        if (alreadyBuild)
        {
            return;
        }
        alreadyBuild = true;

        if (appOptionsBuildAction == null)
        {
            this.ConfigureAppOptions<AppOptions>();
        }
        appOptionsBuildAction!.Invoke();

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
    /// 注册一个额外的服务组。
    /// 调用此方法可在同一进程中创建逻辑隔离的服务集群（例如用于集成测试的客户端组）。
    /// </summary>
    /// <param name="action">用于配置服务组通信组件与启动逻辑的回调。</param>
    /// <returns>当前构建器实例，支持链式调用。</returns>
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
    /// 创建默认服务组，并将主线程进入该服务组的消息循环。此方法由 <see cref="Build"/> 自动调用，不需要手动执行。
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

    /// <summary>
    /// 配置默认服务组。在 <see cref="Build"/> 过程中，所有注册的配置回调会依次作用于默认服务组。
    /// </summary>
    /// <param name="action">用于配置默认服务组的回调，通常用于注册启动回调，执行自定义设置或启动流程。</param>
    /// <returns>当前构建器实例，支持链式调用。</returns>
    public AppHostBuilder ConfigureDefaultServiceGroup(Action<IServiceGroup> action)
    {
        defaultServiceGroupConfigureActions.Add(action);
        return this;
    }

    /// <summary>
    /// 设置自定义的主线程入口操作。默认使用 <see cref="StartHelper.MainThreadLoop"/>。
    /// 在 <see cref="Build"/> 执行时，该操作会在默认服务组的上下文中被调用。
    /// </summary>
    /// <param name="action">要作为主线程入口的操作。</param>
    /// <returns>当前构建器实例，支持链式调用。</returns>
    public AppHostBuilder ConfigureMainThreadAction(Action action)
    {
        mainThreadAction = action;
        return this;
    }

    /// <summary>
    /// 设置命令行参数。这些参数会在 <see cref="Build"/> 中被解析并应用于 <see cref="AppOptions"/>。
    /// </summary>
    /// <param name="args">命令行参数数组。</param>
    /// <returns>当前构建器实例，支持链式调用。</returns>
    public AppHostBuilder SetCommandLineArgs(string[] args)
    {
        commandLineArgs = args;
        return this;
    }

    /// <summary>
    /// 配置 <see cref="AppOptions"/>，允许业务层使用自定义配置类型。
    /// </summary>
    /// <typeparam name="T">自定义的配置类型，必须继承 <see cref="AppOptions"/>。</typeparam>
    /// <returns>当前构建器实例，支持链式调用。</returns>
    public AppHostBuilder ConfigureAppOptions<T>() where T : AppOptions, new()
    {
        var builder = new AppOptionsBuilder<T>();

        // 存储加载逻辑，在 Build 时执行
        appOptionsBuildAction = () =>
        {
            builder.Load(commandLineArgs);
            ProxarHost.Instance.SetAppOptions(builder.Options);
        };
        return this;
    }

}