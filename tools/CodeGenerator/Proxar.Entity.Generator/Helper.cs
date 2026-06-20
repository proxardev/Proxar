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

#pragma warning disable CS1591

namespace ZFSourceGenerator
{
    public static class Helper
    {

        public static string MakeGenericStatement(List<string> argsTemplateList)
        {
            if (argsTemplateList.Count == 0)
            {
                return "";
            }
            var statement = "<"
                + string.Join(", ", argsTemplateList)
                + ">";
            return statement;
        }

        public static string MakeRealArgsStatement(List<string> argsTemplateList, List<string> argsList)
        {
            if (argsTemplateList.Count == 0)
            {
                return string.Empty;
            }
            var realArgsList = new List<string>();
            for (int i = 0; i < argsTemplateList.Count; i++)
            {
                realArgsList.Add($"{argsTemplateList[i]} {argsList[i]},");
            }
            var res = string.Join(" ", realArgsList);
            return res;
        }
    }
}