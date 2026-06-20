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


namespace Proxar.Tasks;


/// <summary>
/// 定义 ZFTask 的执行状态。
/// </summary>
/// <remarks>
/// 任务状态包括：等待执行、成功完成、发生故障、被取消以及重置状态。
/// 用于追踪异步操作或后台任务的生命周期。
/// </remarks>
public enum ZFTaskState
{
    /// <summary>
    /// 任务已创建，等待调度或执行。
    /// </summary>
    Pending = 0,

    /// <summary>
    /// 任务已成功完成，未发生任何异常。
    /// </summary>
    Succeeded = 1,

    /// <summary>
    /// 任务执行过程中发生未处理的异常，导致失败。
    /// </summary>
    Faulted = 2,

    /// <summary>
    /// 任务在完成前被显式取消。
    /// </summary>
    Canceled = 3,

    /// <summary>
    /// 任务被重置为初始状态，可用于重新执行。
    /// </summary>
    Reset = 4
}