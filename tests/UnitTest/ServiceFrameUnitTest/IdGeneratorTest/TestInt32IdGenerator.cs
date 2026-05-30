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


using Proxar.IdGenerator;

namespace FrameUnitTest.IdGeneratorTest;

public class TestInt32IdGenerator : IdGeneratorTestClass
{


    [Theory]
    [InlineData(typeof(Int32IdGenerator), 1, 1000)]
    [InlineData(typeof(Int32IdGenerator), 1, 100000)]
    [InlineData(typeof(Int32IdSafeGenerator), 1, 100000)]
    [InlineData(typeof(Int32IdSafeGenerator), 1000, 100000)]
    public async Task Int32Id_SingleAndParallelCreate_ProducesUniqueId(Type type, int threadCnt, int iter)
    {
        await IdConcurrentGenerationProducesUniqueId<int>(type, threadCnt, iter);
    }
}

