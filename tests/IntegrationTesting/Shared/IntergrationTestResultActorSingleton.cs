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


using Proxar.ActorSingletonCore;

namespace TestShared;

internal class IntergrationTestResultActorSingleton :
    ActorSingleton<IntergrationTestResultActorSingleton>
{
    internal Action<Exception?, string> TestResultAction { get; set; } = null!;
    internal bool FailAfterExecGlobalResultAction { get; set; } = true;

    internal long TargetServiceId { get; set; }
    internal long Id { get; set; }

    public override void Dispose()
    {
        throw new NotImplementedException();
    }
}