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
/// 定义服务的运行状态。
/// </summary>
/// <remarks>
/// 服务状态包括：未启动、正在运行、等待关闭和已关闭。
/// 用于定义追踪服务的生命周期阶段。
/// </remarks>
public enum ServiceStatue
{
    /// <summary>
    /// 服务尚未启动或处于未知状态。
    /// </summary>
    None = 0,

    /// <summary>
    /// 服务正在正常运行中。
    /// </summary>
    Running = 1,

    /// <summary>
    /// 服务已收到关闭信号，正在执行清理或等待当前任务完成。
    /// </summary>
    WaitClose = 3,

    /// <summary>
    /// 服务已完全停止，资源已释放。
    /// </summary>
    Close = 4
}