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


using Proxar.IdGenerator.SnowflakeId;
using Proxar.Utilities;

namespace Proxar.ServiceCore;

internal static class ServiceConfig
{
    internal static int ServiceSnowflakeIdBaseYear { get; set; } = 2026;

    internal static int ThreadCount { get; set; } = 10;

    private static ISnowflakeInfo snowflakeInfo { get; set; } = null!;

    private static ISnowflakeInfo TryGetSnowflakeInfo()
    {
        if (snowflakeInfo == null)
        {
            snowflakeInfo = NewSnowflakeInfo();
        }
        return snowflakeInfo;
    }

    private static ISnowflakeInfo NewSnowflakeInfo()
    {
        var second = TimeHelper.GetSecondByStr($"{ServiceSnowflakeIdBaseYear}-01-01 00:00:00");
        snowflakeInfo = new SnowflakeInfo(32, 14, 17, TimeUnit.Seconds, second);
        return snowflakeInfo;
    }

    public static ISnowflakeInfo SnowflakeInfo => TryGetSnowflakeInfo();


}