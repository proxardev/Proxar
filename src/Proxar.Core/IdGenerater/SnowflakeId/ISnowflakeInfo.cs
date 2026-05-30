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
/// 雪花算法基础信息接口
/// </summary>
public interface ISnowflakeInfo
{
    /// <summary>
    /// 时间戳位宽
    /// </summary>
    int TimestampBits { get; }

    /// <summary>
    /// 机器ID位宽
    /// </summary>
    int WorkerIdBits { get; }

    /// <summary>
    /// 序列号位宽
    /// </summary>
    int SequenceBits { get; }

    /// <summary>
    /// 时间单位
    /// </summary>
    TimeUnit TimeUnit { get; }

    /// <summary>
    /// 最大时间戳值（默认实现：根据位宽计算）
    /// </summary>
    long MaxTimestamp => (1L << TimestampBits) - 1;

    /// <summary>
    /// 最大机器ID值（默认实现：根据位宽计算）
    /// </summary>
    long MaxWorkerId => (1L << WorkerIdBits) - 1;

    /// <summary>
    /// 最大序列号值（默认实现：根据位宽计算）
    /// </summary>
    long MaxSequence => (1L << SequenceBits) - 1;

    /// <summary>
    /// 机器ID移位位数（默认实现：等于序列号位宽）
    /// </summary>
    int WorkerIdShift => SequenceBits;

    /// <summary>
    /// 时间戳移位位数（默认实现：机器ID位宽 + 序列号位宽）
    /// </summary>
    int TimestampShift => WorkerIdBits + SequenceBits;

    /// <summary>
    /// 基础时间戳（UTC）
    /// </summary>
    long BaseTimestamp { get; }

    /// <summary>
    /// 校验接口（默认实现：检查位宽合法性）
    /// </summary>
    public void Validate()
    {
        if (TimestampBits <= 0)
            throw new ArgumentOutOfRangeException(nameof(TimestampBits), "时间戳位宽必须大于0");
        if (WorkerIdBits <= 0)
            throw new ArgumentOutOfRangeException(nameof(WorkerIdBits), "机器ID位宽必须大于0");
        if (SequenceBits <= 0)
            throw new ArgumentOutOfRangeException(nameof(SequenceBits), "序列号位宽必须大于0");
        if (TimestampBits + WorkerIdBits + SequenceBits >= 64)
            throw new ArgumentException("时间戳、机器ID、序列号位宽总和必须小于64");
    }

    long BuildId(long time, long workerId, long sequence)
    {
        return (time << TimestampShift)
               | (workerId << WorkerIdShift)
               | sequence;
    }

    SnowflakeIdStruct ParseId(long id)
    {

        return new SnowflakeIdStruct(id, this);
    }

}