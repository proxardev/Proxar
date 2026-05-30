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
/// 雪花算法基础信息实现类
/// </summary>
public class SnowflakeInfo : ISnowflakeInfo
{
    private readonly int _timestampBits;
    private readonly int _workerIdBits;
    private readonly int _sequenceBits;
    private readonly TimeUnit _timeUnit;
    private readonly long _baseTimestamp;

    /// <summary>
    /// 构造雪花算法基础信息
    /// </summary>
    /// <param name="timestampBits">时间戳位宽</param>
    /// <param name="workerIdBits">机器ID位宽</param>
    /// <param name="sequenceBits">序列号位宽</param>
    /// <param name="timeUnit">时间单位</param>
    /// <param name="baseTimestamp">基础时间戳 UTC时间</param>
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
    public int TimestampBits => _timestampBits;
    public int WorkerIdBits => _workerIdBits;
    public int SequenceBits => _sequenceBits;
    public TimeUnit TimeUnit => _timeUnit;
    public long BaseTimestamp => _baseTimestamp;
    #endregion
}