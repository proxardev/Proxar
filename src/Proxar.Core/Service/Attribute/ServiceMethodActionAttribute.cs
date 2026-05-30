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
public sealed class ServiceMethodActionAttribute : Attribute
{
    public bool RawArgsMethod { get; }
    public bool Queue0ArgsMethod { get; }


    public ServiceMethodActionAttribute(bool rawArgsMethod)
    {
        RawArgsMethod = rawArgsMethod;
    }

    internal ServiceMethodActionAttribute(bool rawArgsMethod, bool queue0ArgsMethod)
    {
        RawArgsMethod = rawArgsMethod;
        Queue0ArgsMethod = queue0ArgsMethod;
    }
}