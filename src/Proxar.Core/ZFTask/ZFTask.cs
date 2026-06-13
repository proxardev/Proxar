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


using Proxar.Tasks.Interfaces;
using System.Runtime.CompilerServices;

namespace Proxar.Tasks;



[AsyncMethodBuilder(typeof(AsyncZFTaskMethodBuilder))]
public class ZFTask : ZFTaskBase<ZFTask>, ICriticalNotifyCompletion, ITask
{
    public static Func<ZFTask> CreateZFTask { get; set; } = InternalZFTaskHelper.CreateZFTaskOwnedByService;
    public static Action<ZFTask>? OnResultConsumed { get; set; } = InternalZFTaskHelper.ReturnZFTaskToPool;

    private static ZFTask completedTask = CreateCompletedTask();

    private static ZFTask CreateCompletedTask()
    {
        var task = new ZFTask();
        task.state = ZFTaskState.Succeeded;
        task.Id = -1;
        return task;
    }

    public static ZFTask CompletedTask => completedTask;


    public static Action<Action>? NextFrameHander;


    public static ZFTask Yield()
    {
        var task = CreateZFTask();
        NextFrame(task);
        return task;
    }

    private static void NextFrame(ZFTask task)
    {
        ArgumentNullException.ThrowIfNull(NextFrameHander);
        NextFrameHander.Invoke(task.SetResult);
    }

    public ZFTask GetAwaiter()
    {
        return this;
    }

    public void GetResult()
    {
        this.CheckResult();
        if (this != CompletedTask)
        {
            OnResultConsumed?.Invoke(this);
        }
    }

    public void SetResult()
    {
        this.SetResultCore();
    }

    public override void OnRented()
    {
        this.Id = ZFTaskInt64IdGeneratorActorSingleton.Current
            .NewId();
    }

}