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


using Proxar.Logging;

namespace Proxar.Utilities;


internal static class ProfileHelper
{

    public static long TestActionCostTime<T>(Action<T> func, T args, string info)
    {
        var startTime = TimeHelper.GetMSSecond();
        func(args);
        var endTime = TimeHelper.GetMSSecond();

        var costTime = endTime - startTime;

        ProxarLogger.Console($"{info} action cost time test :{costTime}");
        return costTime;
    }

    public static long TestActionCostTime<T>(Action<T> func, T args, int count, string info)
    {
        var startTime = TimeHelper.GetMSSecond();
        func(args);
        var endTime = TimeHelper.GetMSSecond();

        var costTime = endTime - startTime;

        ProxarLogger.Console($"{info} action {count} call cost time test :{costTime}");
        return costTime;
    }
}