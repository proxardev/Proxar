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
using Proxar.ServiceCore;

namespace Proxar.AppHost;

public partial class AppHostBuilder
{
    public AppHostBuilder ConfigureChannel(Action<ChannelConfigBuilder> action)
    {
        var builder = new ChannelConfigBuilder();
        action.Invoke(builder);
        builder.Apply();
        return this;
    }

    public AppHostBuilder ConfigureService(Action<ServiceConfigBuilder> action)
    {
        var builder = new ServiceConfigBuilder();
        action.Invoke(builder);
        return this;
    }

    public AppHostBuilder ConfigureLogger(Action<LoggerConfigBuilder> action)
    {
        var builder = new LoggerConfigBuilder();
        action.Invoke(builder);
        builder.Apply();
        return this;
    }
}