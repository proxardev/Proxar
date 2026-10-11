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



/// <summary>
/// 标记一个服务类为知名服务，并指定其在集群中的唯一序列号。
/// 序列号在同一 WorkerId 范围内必须唯一。
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public class KnownServiceAttribute : Attribute
{
    /// <summary>
    /// 知名服务的唯一序列号
    /// </summary>
    public int Sequence { get; }

    public KnownServiceAttribute()
    {
    }

    public KnownServiceAttribute(int sequence)
    {
        Sequence = sequence;
    }
}