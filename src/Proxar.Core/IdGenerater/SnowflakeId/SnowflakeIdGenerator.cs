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


using Proxar.Utilities;
namespace Proxar.IdGenerator.SnowflakeId;

/// <summary>
/// 鍙厤缃寲闆姳ID绠楁硶鐢熸垚鍣?
/// </summary>
public class SnowflakeIdGenerator : IIdGenerator<long>
{
    private readonly ISnowflakeInfo _info;
    private readonly long _workerId;

    private long _state = 0; // 楂?timestampBits 浣嶏細鏃堕棿鎴筹紱浣?sequenceBits 浣嶏細搴忓垪鍙?

    /// <summary>
    /// 鏋勯€犻洩鑺盜D鐢熸垚鍣?
    /// </summary>
    /// <param name="workerId">鏈哄櫒ID</param>
    /// <param name="snowflakeInfo">闆姳绠楁硶閰嶇疆淇℃伅</param>
    public SnowflakeIdGenerator(long workerId, ISnowflakeInfo snowflakeInfo)
    {
        _info = snowflakeInfo ?? throw new ArgumentNullException(nameof(snowflakeInfo));

        if (workerId < 0 || workerId > _info.MaxWorkerId)
            throw new ArgumentOutOfRangeException(
                nameof(workerId),
                $"鏈哄櫒ID蹇呴』鍦?0 ~ {_info.MaxWorkerId} 涔嬮棿");

        _workerId = workerId;
    }

    /// <summary>
    /// 鐢熸垚涓嬩竴涓敮涓€ID
    /// </summary>
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