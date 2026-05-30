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


using Proxar.ServiceSynchronizationContext;
namespace Proxar.Tasks;

internal static class InternalZFTaskHelper
{

    internal static ZFTask CreateZFTaskOwnedByService()
    {
        var task = new ZFTask();
        task.Id = ZFTaskInt64IdGeneratorActorSingleton.Current
            .NewId();
        return task;
        //var task = ActorObjectPoolSingleton<ZFTask>.Current.Rent();
        //SynchronizationContextHelper
        //    .GetSynchronization<ActorSynchronizationContext>()!
        //    .RegisterPenddingZFTask(task);
        //return task;
    }

    internal static void ReturnZFTaskToPool(ZFTask task)
    {
        SynchronizationContextHelper
            .GetSynchronization<ActorSynchronizationContext>()?
            .UnRegisterPenddingZFTask(task);

        //task.ReturnToPool();
    }

    internal static ZFTask<T> CreateZFTaskOwnedByService<T>()
    {
        var task = new ZFTask<T>();
        task.Id = ZFTaskInt64IdGeneratorActorSingleton.Current
            .NewId();
        SynchronizationContextHelper
            .GetSynchronization<ActorSynchronizationContext>()!
            .RegisterPenddingZFTask(task);
        return task;
    }

    internal static void ReturnZFTaskToPool<T>(ZFTask<T> task)
    {
        SynchronizationContextHelper
            .GetSynchronization<ActorSynchronizationContext>()!
            .UnRegisterPenddingZFTask(task);

        // 归还到缓存池
        //task.ReturnToPool();
    }
}