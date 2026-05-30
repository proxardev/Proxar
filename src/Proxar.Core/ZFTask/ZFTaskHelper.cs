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

namespace Proxar.Tasks;


public static class ZFTaskHelper
{
    public static async ZFTask<List<T>> WhenAll<T>(List<ZFTask<T>> taskList)
    {
        var resList = new List<T>();
        foreach (var task in taskList)
        {
            var res = await task;
            resList.Add(res);
        }
        return resList;
    }

    public static async ZFTask WhenAll2<T>(List<ZFTask<T>> taskList)
    {
        var resList = new List<T>();
        foreach (var task in taskList)
        {
            var res = await task;
            resList.Add(res);
        }
    }

    public static void SafeSetResult<T>(this List<ZFTask<T>> taskList, T value)
    {
        if (taskList == null)
        {
            return;
        }
        foreach (var task in taskList)
        {
            try
            {
                task.SetResult(value);
            }
            catch (Exception e)
            {
                ProxarLogger.Error(e);
            }
        }
    }

}