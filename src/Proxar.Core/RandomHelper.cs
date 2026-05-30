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


namespace Proxar.Utilities;


public static class RandomHelper
{

    public static int Random(int range)
    {
        return System.Random.Shared.Next(range);
    }

    public static int Random()
    {
        return System.Random.Shared.Next();
    }

    /// <summary>
    /// 打乱整数列表（原地修改，使用 Fisher–Yates 洗牌算法）
    /// </summary>
    /// <param name="list">要打乱的列表</param>
    public static void ShuffleList<T>(List<T> list)
    {
        if (list == null || list.Count <= 1)
            return;


        for (int i = list.Count - 1; i > 0; i--)
        {
            int randomIndex = Random(i + 1);

            (list[i], list[randomIndex]) = (list[randomIndex], list[i]);
        }
    }

    public static T RandomList<T>(List<T> values)
    {
        return values[Random(values.Count)];
    }
}