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


using Proxar.IdGenerator.Interfaces;
using Proxar.Utilities;
namespace Proxar.IdGenerator.SnowflakeId;



/// <summary>
/// 基于雪花算法的 ID 生成器，实现 <see cref="IIdGenerator{Int64}"/>，可在分布式系统中生成趋势递增的唯一标识符。
/// </summary>
public class SnowflakeIdGenerator : IIdGenerator<long>
{
    private readonly ISnowflakeInfo _info;
    private readonly long _workerId;

    // 高 timestampBits 位：时间戳；低 sequenceBits 位：序列号
    private long _state = 0;

    /// <summary>
    /// 初始化雪花 ID 生成器实例。
    /// </summary>
    /// <param name="workerId">工作节点 ID，必须在 0 到 <see cref="ISnowflakeInfo.MaxWorkerId"/> 之间。</param>
    /// <param name="snowflakeInfo">雪花算法的配置信息，包含位宽分配和时间单位等。</param>
    /// <exception cref="ArgumentNullException"><paramref name="snowflakeInfo"/> 为 null。</exception>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="workerId"/> 超出有效范围。</exception>
    public SnowflakeIdGenerator(long workerId, ISnowflakeInfo snowflakeInfo)
    {
        _info = snowflakeInfo ?? throw new ArgumentNullException(nameof(snowflakeInfo));

        if (workerId < 0 || workerId > _info.MaxWorkerId)
            throw new ArgumentOutOfRangeException(
                nameof(workerId),
                $"工作节点 ID 必须在 0 到 {_info.MaxWorkerId} 之间");

        _workerId = workerId;
    }

    /// <summary>
    /// 生成下一个唯一 ID。
    /// </summary>
    /// <returns>一个趋势递增的全局唯一标识符。</returns>
    public long NewId()
    {
        while (true)
        {
            long state = Interlocked.Read(ref _state);
            long lastTs = state >> _info.SequenceBits;
            long seq = state & _info.MaxSequence;

            long curTs = GetCurrentTimestamp();

            long newTs = lastTs;
            long newSeq = seq;

            if (curTs > lastTs)
            {
                newTs = curTs;
                newSeq = 0;
            }
            else
            {
                newSeq = seq + 1;
                if (newSeq > _info.MaxSequence)
                {
                    newTs = lastTs + 1;
                    newSeq = 0;
                }
            }

            long newState = (newTs << _info.SequenceBits) | newSeq;

            if (Interlocked.CompareExchange(ref _state, newState, state) != state)
            {
                continue;
            }
            return _info.BuildId(newTs, _workerId, newSeq);
        }
    }

    private long GetCurrentTimestamp()
    {
        long currentMs = TimeHelper.GetMSSecond();

        return _info.TimeUnit == TimeUnit.Seconds
            ? currentMs / 1000 - _info.BaseTimestamp
            : currentMs - _info.BaseTimestamp;
    }
}