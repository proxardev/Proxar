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


using FluentAssertions;
using Proxar.IdGenerator;

namespace FrameUnitTest.IdGeneratorTest;

public class IdGeneratorTestClass
{

    internal static async Task IdConcurrentGenerationProducesUniqueId<T>(Type type, int threadCnt, int iter)
        where T : struct, IConvertible
    {
        var idGenerator = Activator.CreateInstance(type!) as IIdGenerator<T>;
        var start = idGenerator!.NewId();

        var action = () =>
        {
            var tmpIdList = new List<T>();
            for (int j = 0; j < iter; j++)
            {
                var id = idGenerator.NewId();
                tmpIdList.Add(id);
            }
            return tmpIdList;
        };
        var funcList = Enumerable.Range(0, threadCnt)
            .Select(i => action)
            .ToList();
        var taskList = funcList
            .Select(action => Task.Run(action))
            .ToList();
        var idList = await Task.WhenAll(taskList);
        var allIdSet = idList
            .SelectMany(id => id)
            .ToHashSet();

        var maxId = allIdSet.Max().ToInt64(null);
        var count = (long)allIdSet.Count;

        var expectedMax = (long)threadCnt * iter + start.ToInt64(null);
        var expectedCount = (long)threadCnt * iter;

        maxId.Should().Be(expectedMax);
        count.Should().Be(expectedCount);
    }


}

