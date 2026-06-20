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
/// 用于配置服务的参数的构建器，支持链式调用。
/// </summary>
/// <remarks>
/// 通过此类可以设置服务的雪花 ID 基准年份、线程数等配置项。
/// 最终配置会写入 <see cref="ServiceConfig"/> 静态类中。
/// </remarks>
public class ServiceConfigBuilder
{
    /// <summary>
    /// 设置服务雪花 ID 生成器的基准年份。
    /// </summary>
    /// <param name="year">基准年份（例如 2024），用于计算时间戳偏移量。</param>
    /// <returns>当前 <see cref="ServiceConfigBuilder"/> 实例。</returns>
    /// <remarks>
    /// 该值将赋给 <see cref="ServiceConfig.ServiceSnowflakeIdBaseYear"/>，影响全局 ID 生成逻辑。
    /// 默认情况下可能使用固定值，调用此方法可覆盖。
    /// </remarks>
    public ServiceConfigBuilder UseServiceSnowflakeIdBaseYear(int year)
    {
        ServiceConfig.ServiceSnowflakeIdBaseYear = year;
        return this;
    }

    /// <summary>
    /// 设置服务使用的线程池线程数量。
    /// </summary>
    /// <param name="threadCount">线程数量，必须大于 0。</param>
    /// <returns>当前 <see cref="ServiceConfigBuilder"/> 实例。</returns>
    /// <remarks>
    /// 该值将赋给 <see cref="ServiceConfig.ThreadCount"/>，影响服务并发处理能力。
    /// 如果设置不合理（如过大或过小），可能导致性能问题。
    /// </remarks>
    public ServiceConfigBuilder UseThreadCount(int threadCount)
    {
        ServiceConfig.ThreadCount = threadCount;
        return this;
    }
}