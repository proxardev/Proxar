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


using TestShared.TestClass;
using Xunit.Abstractions;

namespace MemoryLeakTests
{
    public class MemoryLeak : TestClassBaseWithOutput
    {
        public MemoryLeak(ITestOutputHelper output) : base(output)
        {
        }

        [Theory]
        [InlineData(nameof(TestMemoryServiceProxy.GlobalTimer_CreateAndDestory_NotLeak), 1000)]
        [InlineData(nameof(TestMemoryServiceProxy.EntityTimer_CreateAndDestory_NotLeak), 1000)]
        [InlineData(nameof(TestMemoryServiceProxy.Service_CreateAndDestory_NotLeak), 10000)]
        [InlineData(nameof(TestMemoryServiceProxy.Service_CallMethod_NotLeak), 10000)]
        internal async Task CallTargetProtoMethod(string methodName, int args = int.MinValue, int args2 = int.MinValue,
            int args3 = int.MinValue,
            int args4 = int.MinValue
            )
        {
            await this.CallMethodByProxy<TestMemoryService, TestMemoryServiceProxy>(methodName, args, args2, args3, args4);
        }

        //[Theory]
        //[InlineData(nameof(TestMemoryServiceProxy.Service_CallMethod_NotLeak2), 10000)]
        //[DotMemoryUnit(FailIfRunWithoutSupport = false)]
        //internal async Task CallTargetProtoMethod2(string methodName, int args = int.MinValue, int args2 = int.MinValue,
        //    int args3 = int.MinValue,
        //    int args4 = int.MinValue
        //    )
        //{

        //    await CheckCreateTargetTestService<TestMemoryService>();

        //    var baseCheck = dotMemory.Check();
        //    await this.CallMethod<TestMemoryService, TestMemoryServiceProxy>(methodName, args, args2, args3, args4);
        //    var after = dotMemory.Check((check) =>
        //    {
        //        var size = check.GetTrafficFrom(baseCheck).AllocatedMemory.SizeInBytes;
        //        size.Should().BeLessThan(1024 * 1024 * 4);
        //    });
        //}
    }

}