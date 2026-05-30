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
using Proxar.ServiceCore;
using Proxar.Tasks;
using Proxar.Utilities;

namespace ServiceIntegrationTesting;

public partial class TestService_Message
{
    /// <summary>
    /// 200: List<int> → IEnumerable<int>
    /// </summary>
    [ServiceMethod(200)]
    [ServiceMethodExport]
    private async ZFTask SendIntList_ToEnumerable_Success()
    {
        var proxy = GetTargetServiceProxy();
        int length = 12;
        List<int> list = RangeHelper.Range(length).ToList();

        int result = await proxy.ReceiveEnumerable_ReturnLength_200(list);
        result.Should().Be(length);
        TestResultHelper.SetFinishTest();
    }

    /// <summary>
    /// 201: List<int> → IReadOnlyList<int>
    /// </summary>
    [ServiceMethod(201)]
    [ServiceMethodExport]
    private async ZFTask SendIntList_ToReadOnlyList_Success()
    {
        var proxy = GetTargetServiceProxy();
        int length = 12;
        List<int> list = RangeHelper.Range(length).ToList();

        int result = await proxy.ReceiveReadOnlyList_ReturnLength_201(list);
        result.Should().Be(length);
        TestResultHelper.SetFinishTest();
    }

    /// <summary>
    /// 202: List<int> → IReadOnlyCollection<int>
    /// </summary>
    [ServiceMethod(202)]
    [ServiceMethodExport]
    private async ZFTask SendIntList_ToReadOnlyCollection_Success()
    {
        var proxy = GetTargetServiceProxy();
        int length = 12;
        List<int> list = RangeHelper.Range(length).ToList();

        int result = await proxy.ReceiveReadOnlyCollection_ReturnLength_202(list);
        result.Should().Be(length);
        TestResultHelper.SetFinishTest();
    }

    /// <summary>
    /// 203: int[] → IEnumerable<int>
    /// </summary>
    [ServiceMethod(203)]
    [ServiceMethodExport]
    private async ZFTask SendIntArray_ToEnumerable_Success()
    {
        var proxy = GetTargetServiceProxy();
        int length = 12;
        int[] array = RangeHelper.Range(length).ToArray();

        int result = await proxy.ReceiveEnumerable_ReturnLength_203(array);
        result.Should().Be(length);
        TestResultHelper.SetFinishTest();
    }

    /// <summary>
    /// 204: int[] → IReadOnlyList<int>
    /// </summary>
    [ServiceMethod(204)]
    [ServiceMethodExport]
    private async ZFTask SendIntArray_ToReadOnlyList_Success()
    {
        var proxy = GetTargetServiceProxy();
        int length = 12;
        int[] array = RangeHelper.Range(length).ToArray();

        int result = await proxy.ReceiveReadOnlyList_ReturnLength_204(array);
        result.Should().Be(length);
        TestResultHelper.SetFinishTest();
    }

    /// <summary>
    /// 205: int[] → IReadOnlyCollection<int>
    /// </summary>
    [ServiceMethod(205)]
    [ServiceMethodExport]
    private async ZFTask SendIntArray_ToReadOnlyCollection_Success()
    {
        var proxy = GetTargetServiceProxy();
        int length = 12;
        int[] array = RangeHelper.Range(length).ToArray();

        int result = await proxy.ReceiveReadOnlyCollection_ReturnLength_205(array);
        result.Should().Be(length);
        TestResultHelper.SetFinishTest();
    }

    /// <summary>
    /// 206: HashSet<int> → IEnumerable<int>
    /// </summary>
    [ServiceMethod(206)]
    [ServiceMethodExport]
    private async ZFTask SendIntHashSet_ToEnumerable_Success()
    {
        var proxy = GetTargetServiceProxy();
        int length = 12;
        HashSet<int> hashSet = new HashSet<int>(RangeHelper.Range(length));

        int result = await proxy.ReceiveEnumerable_ReturnLength_206(hashSet);
        result.Should().Be(length);
        TestResultHelper.SetFinishTest();
    }

    /// <summary>
    /// 207: HashSet<int> → IReadOnlyCollection<int>
    /// </summary>
    [ServiceMethod(207)]
    [ServiceMethodExport]
    private async ZFTask SendIntHashSet_ToReadOnlyCollection_Success()
    {
        var proxy = GetTargetServiceProxy();
        int length = 12;
        HashSet<int> hashSet = new HashSet<int>(RangeHelper.Range(length));

        int result = await proxy.ReceiveReadOnlyCollection_ReturnLength_207(hashSet);
        result.Should().Be(length);
        TestResultHelper.SetFinishTest();
    }

    /// <summary>
    /// 208: Dictionary<int, int> → IReadOnlyDictionary<int, int>
    /// </summary>
    [ServiceMethod(208)]
    [ServiceMethodExport]
    private async ZFTask SendIntDictionary_ToReadOnlyDictionary_Success()
    {
        var proxy = GetTargetServiceProxy();
        int count = 5;
        Dictionary<int, int> dict = new Dictionary<int, int>();
        for (int i = 0; i < count; i++) dict.Add(i, i * 10);

        int result = await proxy.ReceiveReadOnlyDictionary_ReturnCount_208(dict);
        result.Should().Be(count);
        TestResultHelper.SetFinishTest();
    }

    /// <summary>
    /// 209: List<PlayerScore> → IEnumerable<PlayerScore>
    /// </summary>
    [ServiceMethod(209)]
    [ServiceMethodExport]
    private async ZFTask SendPlayerScoreList_ToEnumerable_Success()
    {
        var proxy = GetTargetServiceProxy();
        int length = 5;
        var list = RangeHelper.Range(length)
            .Select(x => new PlayerScore { PlayerId = x, Score = 10 })
            .ToList();

        int total = await proxy.ReceiveEnumerable_ReturnTotalScore_209(list);
        total.Should().Be(50);
        TestResultHelper.SetFinishTest();
    }

    /// <summary>
    /// 210: List<PlayerScore> → IReadOnlyList<PlayerScore>
    /// </summary>
    [ServiceMethod(210)]
    [ServiceMethodExport]
    private async ZFTask SendPlayerScoreList_ToReadOnlyList_Success()
    {
        var proxy = GetTargetServiceProxy();
        var list = new List<PlayerScore>
    {
        new() { PlayerId = 1, Score = 20 },
        new() { PlayerId = 2, Score = 30 }
    };

        int total = await proxy.ReceiveReadOnlyList_ReturnTotalScore_210(list);
        total.Should().Be(50);
        TestResultHelper.SetFinishTest();
    }

    /// <summary>
    /// 211: List<PlayerScore> → IReadOnlyCollection<PlayerScore>
    /// </summary>
    [ServiceMethod(211)]
    [ServiceMethodExport]
    private async ZFTask SendPlayerScoreList_ToReadOnlyCollection_Success()
    {
        var proxy = GetTargetServiceProxy();
        var list = new List<PlayerScore>
    {
        new() { PlayerId = 9, Score = 100 },
        new() { PlayerId = 8, Score = 200 }
    };

        int total = await proxy.ReceiveReadOnlyCollection_ReturnTotalScore_211(list);
        total.Should().Be(300);
        TestResultHelper.SetFinishTest();
    }

    /// <summary>
    /// 212: PlayerScore[] → IEnumerable<PlayerScore>
    /// </summary>
    [ServiceMethod(212)]
    [ServiceMethodExport]
    private async ZFTask SendPlayerScoreArray_ToEnumerable_Success()
    {
        var proxy = GetTargetServiceProxy();
        var array = new[]
        {
        new PlayerScore { PlayerId = 1, Score = 5 },
        new PlayerScore { PlayerId = 2, Score = 5 }
    };

        int total = await proxy.ReceiveEnumerable_ReturnTotalScore_212(array);
        total.Should().Be(10);
        TestResultHelper.SetFinishTest();
    }

    /// <summary>
    /// 213: PlayerScore[] → IReadOnlyList<PlayerScore>
    /// </summary>
    [ServiceMethod(213)]
    [ServiceMethodExport]
    private async ZFTask SendPlayerScoreArray_ToReadOnlyList_Success()
    {
        var proxy = GetTargetServiceProxy();
        var array = new[]
        {
        new PlayerScore { PlayerId = 5, Score = 10 }
    };

        int total = await proxy.ReceiveReadOnlyList_ReturnTotalScore_213(array);
        total.Should().Be(10);
        TestResultHelper.SetFinishTest();
    }

    /// <summary>
    /// 214: PlayerScore[] → IReadOnlyCollection<PlayerScore>
    /// </summary>
    [ServiceMethod(214)]
    [ServiceMethodExport]
    private async ZFTask SendPlayerScoreArray_ToReadOnlyCollection_Success()
    {
        var proxy = GetTargetServiceProxy();
        var array = new[]
        {
        new PlayerScore { PlayerId = 1, Score = 7 },
        new PlayerScore { PlayerId = 2, Score = 13 }
    };

        int total = await proxy.ReceiveReadOnlyCollection_ReturnTotalScore_214(array);
        total.Should().Be(20);
        TestResultHelper.SetFinishTest();
    }

    /// <summary>
    /// 215: HashSet<PlayerScore> → IEnumerable<PlayerScore>
    /// </summary>
    [ServiceMethod(215)]
    [ServiceMethodExport]
    private async ZFTask SendPlayerScoreHashSet_ToEnumerable_Success()
    {
        var proxy = GetTargetServiceProxy();
        var hashSet = new HashSet<PlayerScore>
    {
        new() { PlayerId = 1, Score = 10 },
        new() { PlayerId = 2, Score = 20 }
    };

        int total = await proxy.ReceiveEnumerable_ReturnTotalScore_215(hashSet);
        total.Should().Be(30);
        TestResultHelper.SetFinishTest();
    }

    /// <summary>
    /// 216: HashSet<PlayerScore> → IReadOnlyCollection<PlayerScore>
    /// 【已修复：独立完整用例】
    /// </summary>
    [ServiceMethod(216)]
    [ServiceMethodExport]
    private async ZFTask SendPlayerScoreHashSet_ToReadOnlyCollection_Success()
    {
        var proxy = GetTargetServiceProxy();
        var hashSet = new HashSet<PlayerScore>
    {
        new() { PlayerId = 1, Score = 10 },
        new() { PlayerId = 2, Score = 20 }
    };

        int total = await proxy.ReceiveReadOnlyCollection_ReturnTotalScore_216(hashSet);
        total.Should().Be(30);
        TestResultHelper.SetFinishTest();
    }

    /// <summary>
    /// 217: Dictionary<int, PlayerScore> → IReadOnlyDictionary<int, PlayerScore>
    /// </summary>
    [ServiceMethod(217)]
    [ServiceMethodExport]
    private async ZFTask SendIntKeyPlayerScoreDict_ToReadOnlyDictionary_Success()
    {
        var proxy = GetTargetServiceProxy();
        var dict = new Dictionary<int, PlayerScore>
        {
            [1] = new() { PlayerId = 1, Score = 10 },
            [2] = new() { PlayerId = 2, Score = 20 }
        };

        int total = await proxy.ReceiveIntKeyDict_ReturnTotalScore_217(dict);
        total.Should().Be(30);
        TestResultHelper.SetFinishTest();
    }

    /// <summary>
    /// 218: Dictionary<PlayerScore, PlayerScore> → IReadOnlyDictionary<PlayerScore, PlayerScore>
    /// Key 是 Class
    /// </summary>
    [ServiceMethod(218)]
    [ServiceMethodExport]
    private async ZFTask SendPlayerScoreKeyDict_ToReadOnlyDictionary_Success()
    {
        var proxy = GetTargetServiceProxy();
        var key1 = new PlayerScore { PlayerId = 1, Score = 100 };
        var key2 = new PlayerScore { PlayerId = 2, Score = 200 };

        var dict = new Dictionary<PlayerScore, PlayerScore>
        {
            [key1] = key1,
            [key2] = key2
        };

        int sum = await proxy.ReceiveClassKeyDict_ReturnSumValue_218(dict);
        sum.Should().Be(300);
        TestResultHelper.SetFinishTest();
    }
}