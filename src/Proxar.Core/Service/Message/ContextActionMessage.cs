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


namespace Proxar.ServiceCore.Message;


internal class ContextActionMessage : NativeServiceMessage
{
    private object? state;
    private SendOrPostCallback? sendOrPostCallback1;
    private Action? action;


    public ContextActionMessage(SendOrPostCallback sendOrPostCallback, object? state)
    {
        this.sendOrPostCallback1 = sendOrPostCallback;
        this.state = state;
        this.action = null;
    }

    public ContextActionMessage(Action action)
    {
        this.action = action;
    }

    public void Init()
    {
        this.SetHeadData(0, 0, 0, ProtoBase.ContextAction);
    }

    public void Execute()
    {
        this.action?.Invoke();
        this.sendOrPostCallback1?.Invoke(this.state);
    }
}