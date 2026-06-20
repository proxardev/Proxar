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
/// 雪花算法基础配置信息接口，提供ID生成所需的位宽、时间单位和掩码，并提供ID构建与解析的默认实现。
/// </summary>
public interface ISnowflakeInfo
{
    /// <summary>
    /// 时间戳部分的位宽。
    /// </summary>
    int TimestampBits { get; }

    /// <summary>
    /// 工作节点ID部分的位宽。
    /// </summary>
    int WorkerIdBits { get; }

    /// <summary>
    /// 序列号部分的位宽。
    /// </summary>
    int SequenceBits { get; }

    /// <summary>
    /// 时间戳的时间单位。
    /// </summary>
    TimeUnit TimeUnit { get; }

    /// <summary>
    /// 根据位宽计算的最大时间戳掩码。
    /// </summary>
    long MaxTimestamp => (1L << TimestampBits) - 1;

    /// <summary>
    /// 根据位宽计算的最大工作节点ID掩码。
    /// </summary>
    long MaxWorkerId => (1L << WorkerIdBits) - 1;

    /// <summary>
    /// 根据位宽计算的最大序列号掩码。
    /// </summary>
    long MaxSequence => (1L << SequenceBits) - 1;

    /// <summary>
    /// 工作节点ID在雪花ID中的移位位数，默认等于序列号位宽。
    /// </summary>
    int WorkerIdShift => SequenceBits;

    /// <summary>
    /// 时间戳在雪花ID中的移位位数，默认等于工作节点ID位宽与序列号位宽之和。
    /// </summary>
    int TimestampShift => WorkerIdBits + SequenceBits;

    /// <summary>
    /// 基准时间戳（UTC），用于计算相对时间偏移。
    /// </summary>
    long BaseTimestamp { get; }

    /// <summary>
    /// 校验配置的有效性，确保位宽之和小于64且各部分位宽为正数。
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException">某部分位宽小于或等于0。</exception>
    /// <exception cref="ArgumentException">总位宽大于或等于64。</exception>
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

    /// <summary>
    /// 根据时间戳、工作节点ID和序列号构建一个雪花算法ID。
    /// </summary>
    /// <param name="time">相对时间偏移量。</param>
    /// <param name="workerId">工作节点ID。</param>
    /// <param name="sequence">序列号。</param>
    /// <returns>组合后的雪花ID。</returns>
    long BuildId(long time, long workerId, long sequence)
    {
        return (time << TimestampShift)
               | (workerId << WorkerIdShift)
               | sequence;
    }

    /// <summary>
    /// 解析雪花ID为可读的结构体形式。
    /// </summary>
    /// <param name="id">雪花算法生成的原始ID。</param>
    /// <returns>包含时间戳、工作节点ID和序列号的 <see cref="SnowflakeIdStruct"/>。</returns>
    SnowflakeIdStruct ParseId(long id)
    {
        return new SnowflakeIdStruct(id, this);
    }
}