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
using Proxar.IdGenerator.Interfaces;
using Proxar.ServiceCore;
using Proxar.ServiceCore.Interfaces;

namespace Proxar.AppHost;



/// <summary>
/// 程序的全局根对象，作为 <see cref="Singleton{ProxarHost}"/> 提供对整个应用宿主、配置和基础设施的入口访问。
/// </summary>
public class ProxarHost : Singleton<ProxarHost>
{
    /// <summary>
    /// 获取当前线程所在服务组的网关消息调用器。
    /// </summary>
    public IGateMessageInvoker GateMessageInvoker => ActorThreadScope.ServiceGroup.GateMessageInvoker;

    /// <summary>
    /// 获取或设置全局的雪花算法 ID 生成器。
    /// </summary>
    public IIdGenerator<long> SnowflakeIdGenerator { get; set; } = null!;

    /// <summary>
    /// 获取当前应用的配置选项，在宿主启动时由命令行参数和配置文件填充。
    /// </summary>
    public AppOptions AppOptions { get; private set; } = null!;


    internal void GameInit()
    {
        FrameInit.Init();
    }

    internal void SetAppOptions(AppOptions options)
    {
        AppOptions = options ?? throw new ArgumentNullException(nameof(options));
    }
}