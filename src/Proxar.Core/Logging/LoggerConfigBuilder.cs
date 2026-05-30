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


using Microsoft.Extensions.Logging;
using Proxar.Logging;

namespace Proxar.Network;

public class LoggerConfigBuilder
{
    private ILoggerFactory loggerFactory = null!;
    private Action<Exception, string> errorAction = null!;

    public LoggerConfigBuilder SetLoggerFactory(ILoggerFactory loggerFactory)
    {
        this.loggerFactory = loggerFactory;
        return this;
    }

    public LoggerConfigBuilder SetErrorAction(Action<Exception, string> action)
    {
        this.errorAction = action;
        return this;
    }

    internal void Apply()
    {
        ProxarLogger.SetLoggerFactory(loggerFactory);
        ProxarLogger.SetErrorAction(errorAction);
    }
}