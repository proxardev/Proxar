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


using Proxar.Core;
using Proxar.Threading;

namespace TestShared;

internal class GlobalTestResultSingleton :
    Singleton<GlobalTestResultSingleton>
{
    // 当service没找到结果委托，执行这个全局委托
    private Action<Exception?, string>? GlobalResultAction = null!;

    private SpinLockScope SpinLock = new SpinLockScope();


    public void AddAction(Action<Exception?, string> action)
    {
        SpinLock.Execute(AddAction2, action);
    }

    public void AddAction2(Action<Exception?, string> action)
    {
        GlobalResultAction += action;
    }
    public void RemoveAction(Action<Exception?, string> action)
    {
        SpinLock.Execute(RemoveAction2, action);
    }

    public void RemoveAction2(Action<Exception?, string> action)
    {
        GlobalResultAction -= action!;
    }

    public void ExeAction(Exception? exception, string info)
    {
        if (!IntergrationTestResultActorSingleton.Current.FailAfterExecGlobalResultAction)
        {
            return;
        }
        var action = () =>
        {
            this.ExeAction2(exception, info); ;
        };
        SpinLock.Execute(action);
    }
    public void ExeAction2(Exception? exception, string info)
    {
        GlobalResultAction?.Invoke(exception, info);
    }


}