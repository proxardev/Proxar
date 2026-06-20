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


namespace Proxar.IdGenerator.SnowflakeId;
/// <summary>
/// <see cref="ISnowflakeInfo"/> 的标准实现，用于描述雪花算法的位宽分配、时间单位和基准时间戳。
/// </summary>
/// <remarks>
/// 在构造时，会通过 <see cref="ISnowflakeInfo.Validate"/> 接口默认实现校验配置的合法性，
/// 确保各部分位宽之和不超过63位，且各部分位宽均为正数。
/// </remarks>
public class SnowflakeInfo : ISnowflakeInfo
{
    private readonly int _timestampBits;
    private readonly int _workerIdBits;
    private readonly int _sequenceBits;
    private readonly TimeUnit _timeUnit;
    private readonly long _baseTimestamp;

    /// <summary>
    /// 使用指定的位宽、时间单位和基准时间戳初始化 <see cref="SnowflakeInfo"/> 的新实例。
    /// </summary>
    /// <param name="timestampBits">分配给时间戳部分的位宽。</param>
    /// <param name="workerIdBits">分配给工作节点 ID 部分的位宽。</param>
    /// <param name="sequenceBits">分配给序列号部分的位宽。</param>
    /// <param name="timeUnit">时间戳的时间单位（秒或毫秒）。</param>
    /// <param name="baseTimestamp">
    /// 基准 UTC 时间戳值，用于计算相对时间偏移量。
    /// 通常设置为系统上线日期（如 <c>new DateTimeOffset(2025, 1, 1, 0, 0, 0, TimeSpan.Zero).ToUnixTimeMilliseconds()</c>）。
    /// </param>
    /// <exception cref="ArgumentOutOfRangeException">任一位宽参数小于或等于零。</exception>
    /// <exception cref="ArgumentException">总位宽大于或等于64。</exception>
    public SnowflakeInfo(
        int timestampBits,
        int workerIdBits,
        int sequenceBits,
        TimeUnit timeUnit,
        long baseTimestamp)
    {
        _timestampBits = timestampBits;
        _workerIdBits = workerIdBits;
        _sequenceBits = sequenceBits;
        _timeUnit = timeUnit;
        _baseTimestamp = baseTimestamp;

        // 调用接口默认实现的校验逻辑
        (this as ISnowflakeInfo).Validate();
    }

    #region ISnowflakeInfo 接口实现（仅实现无默认值的成员）

    /// <inheritdoc/>
    public int TimestampBits => _timestampBits;

    /// <inheritdoc/>
    public int WorkerIdBits => _workerIdBits;

    /// <inheritdoc/>
    public int SequenceBits => _sequenceBits;

    /// <inheritdoc/>
    public TimeUnit TimeUnit => _timeUnit;

    /// <inheritdoc/>
    public long BaseTimestamp => _baseTimestamp;

    #endregion
}