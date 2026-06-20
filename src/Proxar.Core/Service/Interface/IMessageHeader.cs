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


namespace Proxar.ServiceCore.Interfaces;


/// <summary>
/// 定义消息头的通用接口，提供对头部数据的只读访问。
/// </summary>
public interface IMessageHeader
{
    /// <summary>
    /// 获取消息头数据的只读跨度。
    /// </summary>
    /// <returns>包含头部数据的 <see cref="ReadOnlySpan{Int64}"/>。</returns>
    public ReadOnlySpan<long> GetHeaderReadOnlySpan();
}