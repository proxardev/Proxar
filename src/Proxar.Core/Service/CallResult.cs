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
/// 用于指示 RPC 调用返回类型的标记结构体。作为 <see cref="Service.Result{TRet}"/> 的返回类型，用于发起可等待的远程调用。
/// </summary>
/// <typeparam name="TRet">期望的调用返回类型。</typeparam>
/// <remarks>
/// 框架、生成代码使用，外部不要直接使用
/// </remarks>
public struct CallResult<TRet> { }

public static partial class Service
{
    /// <summary>
    /// 获取一个 <see cref="CallResult{TRet}"/> 标记实例，用于发起对远程服务的可等待调用（RPC）。
    /// </summary>
    /// <typeparam name="TRet">期望的调用返回类型。</typeparam>
    /// <returns>一个默认的 <see cref="CallResult{TRet}"/> 实例。</returns>
    /// <remarks>
    /// 框架、生成代码使用，外部不要直接使用
    /// </remarks>
    public static CallResult<TRet> Result<TRet>() => default;
}