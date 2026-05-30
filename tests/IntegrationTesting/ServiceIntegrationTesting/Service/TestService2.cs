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
using Proxar.ActorSingletonCore;
using Proxar.IdGenerator;
using Proxar.ServiceCore;
using Proxar.Tasks;

namespace ServiceIntegrationTesting;

internal partial class TestService2 : ServiceBase
{
    public class TestIdGeneratorActorSingleton : IdGeneratorActorSingleton<TestIdGeneratorActorSingleton, long, Int64IdGenerator>
    {

    }

    [ServiceMethod(1)]
    private async ZFTask<string> IterActorSingletonIdGenerattor_CheckThreadSafe(int count, int delayMSTime)
    {
        try
        {

            var idList = new List<long>();
            var ownerServiceIdSet = new HashSet<long>();
            for (int i = 0; i < count; i++)
            {
                ownerServiceIdSet.Add(TestIdGeneratorActorSingleton
                    .Current.OwnerServiceId);
                var id = TestIdGeneratorActorSingleton.Current.NewId();
                idList.Add(id);
                await Service.NextFrame();
            }
            ownerServiceIdSet.First().Should().Be(this.GetServiceId());
            ownerServiceIdSet.Count.Should().Be(1);
            var min = idList.Min();
            var max = idList.Max();

            idList.First().Should().Be(min);
            idList.Last().Should().Be(max);
            idList.Count.Should().Be(idList.ToHashSet().Count);
            idList.Count.Should().Be(count);
            var front = idList.First() - 1;
            foreach (var id in idList)
            {
                front++;
                id.Should().Be(front);
            }
        }
        catch (Exception e)
        {
            return e.ToString();
        }

        return string.Empty;
    }

}