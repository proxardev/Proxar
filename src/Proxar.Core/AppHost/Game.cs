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
using Proxar.IdGenerator;
using Proxar.ServiceCore;
using Proxar.ServiceCore.Interfaces;

namespace Proxar.AppHost;

public class Game : Singleton<Game>
{
    public IIdGenerator<long> IdGenerator2 { get; set; } = new Int64IdSafeGenerator();

    public IGateMessageInvoker GateMessageInvoker => ActorThreadScope.ServiceGroup.GateMessageInvoker;

    public IIdGenerator<long> SnowflakeIdGenerator { get; set; } = null!;

    public AppOptions AppOptions { get; private set; } = null!;

    internal void GameInit()
    {
        FrameInit.Init();
    }


    internal void ConfigureAppOptions(string[] args)
    {
        AppOptions = AppOptionsLoader.Load<AppOptions>(args);
    }


}