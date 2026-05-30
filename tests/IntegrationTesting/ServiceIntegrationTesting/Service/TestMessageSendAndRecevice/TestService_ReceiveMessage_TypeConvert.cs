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


using Proxar.ServiceCore;

namespace ServiceIntegrationTesting;

public partial class TestService_ReceiveMessage
{
    [ServiceMethod(200)]
    [ServiceMethodExport]
    private int ReceiveEnumerable_ReturnLength_200(IEnumerable<int> value) => value.Count();

    [ServiceMethod(201)]
    [ServiceMethodExport]
    private int ReceiveReadOnlyList_ReturnLength_201(IReadOnlyList<int> value) => value.Count;

    [ServiceMethod(202)]
    [ServiceMethodExport]
    private int ReceiveReadOnlyCollection_ReturnLength_202(IReadOnlyCollection<int> value) => value.Count;

    [ServiceMethod(203)]
    [ServiceMethodExport]
    private int ReceiveEnumerable_ReturnLength_203(IEnumerable<int> value) => value.Count();

    [ServiceMethod(204)]
    [ServiceMethodExport]
    private int ReceiveReadOnlyList_ReturnLength_204(IReadOnlyList<int> value) => value.Count;

    [ServiceMethod(205)]
    [ServiceMethodExport]
    private int ReceiveReadOnlyCollection_ReturnLength_205(IReadOnlyCollection<int> value) => value.Count;

    [ServiceMethod(206)]
    [ServiceMethodExport]
    private int ReceiveEnumerable_ReturnLength_206(IEnumerable<int> value) => value.Count();

    [ServiceMethod(207)]
    [ServiceMethodExport]
    private int ReceiveReadOnlyCollection_ReturnLength_207(IReadOnlyCollection<int> value) => value.Count;

    [ServiceMethod(208)]
    [ServiceMethodExport]
    private int ReceiveReadOnlyDictionary_ReturnCount_208(IReadOnlyDictionary<int, int> value) => value.Count;

    [ServiceMethod(209)]
    [ServiceMethodExport]
    private int ReceiveEnumerable_ReturnTotalScore_209(IEnumerable<PlayerScore> value)
        => value.Sum(x => x.Score);

    [ServiceMethod(210)]
    [ServiceMethodExport]
    private int ReceiveReadOnlyList_ReturnTotalScore_210(IReadOnlyList<PlayerScore> value)
        => value.Sum(x => x.Score);

    [ServiceMethod(211)]
    [ServiceMethodExport]
    private int ReceiveReadOnlyCollection_ReturnTotalScore_211(IReadOnlyCollection<PlayerScore> value)
        => value.Sum(x => x.Score);

    [ServiceMethod(212)]
    [ServiceMethodExport]
    private int ReceiveEnumerable_ReturnTotalScore_212(IEnumerable<PlayerScore> value)
        => value.Sum(x => x.Score);

    [ServiceMethod(213)]
    [ServiceMethodExport]
    private int ReceiveReadOnlyList_ReturnTotalScore_213(IReadOnlyList<PlayerScore> value)
        => value.Sum(x => x.Score);

    [ServiceMethod(214)]
    [ServiceMethodExport]
    private int ReceiveReadOnlyCollection_ReturnTotalScore_214(IReadOnlyCollection<PlayerScore> value)
        => value.Sum(x => x.Score);

    [ServiceMethod(215)]
    [ServiceMethodExport]
    private int ReceiveEnumerable_ReturnTotalScore_215(IEnumerable<PlayerScore> value)
        => value.Sum(x => x.Score);

    [ServiceMethod(216)]
    [ServiceMethodExport]
    private int ReceiveReadOnlyCollection_ReturnTotalScore_216(IReadOnlyCollection<PlayerScore> value)
        => value.Sum(x => x.Score);

    [ServiceMethod(217)]
    [ServiceMethodExport]
    private int ReceiveIntKeyDict_ReturnTotalScore_217(IReadOnlyDictionary<int, PlayerScore> value)
        => value.Values.Sum(x => x.Score);

    [ServiceMethod(218)]
    [ServiceMethodExport]
    private int ReceiveClassKeyDict_ReturnSumValue_218(IReadOnlyDictionary<PlayerScore, PlayerScore> value)
        => value.Values.Sum(x => x.Score);
}