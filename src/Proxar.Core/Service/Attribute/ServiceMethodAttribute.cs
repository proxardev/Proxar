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


namespace Proxar.ServiceCore;


[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
public sealed class ServiceMethodAttribute : Attribute
{
    public int Proto { get; }

    /// <summary>
    /// If true, the caller will not automatically handle the return value, and the method itself is responsible for sending the response back to the caller.
    /// </summary>
    public bool SelfHandleReturn { get; }


    public ServiceMethodAttribute(int proto)
    {
        Proto = proto;
    }

    public ServiceMethodAttribute(int proto, bool selfHandleReturn)
    {
        Proto = proto;
        SelfHandleReturn = selfHandleReturn;
    }
}