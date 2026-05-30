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

public readonly struct SnowflakeIdStruct
{
    private readonly long _id;
    private readonly ISnowflakeInfo _info;

    /// <summary>
    /// 通过ID解析组件
    /// </summary>
    public SnowflakeIdStruct(long id, Func<ISnowflakeInfo> infoProvider)
        : this(infoProvider)
    {
        _id = id;
    }

    /// <summary>
    /// 通过ID解析组件
    /// </summary>
    public SnowflakeIdStruct(long id, ISnowflakeInfo snowflakeInfo)
    {
        _info = snowflakeInfo;
        _id = id;
    }

    private SnowflakeIdStruct(Func<ISnowflakeInfo> infoProvider)
    {
        _id = 0;
        _info = infoProvider() ?? throw new ArgumentNullException(nameof(infoProvider));
    }

    public long Id => _id;
    public long Timestamp => (_id >> _info.TimestampShift) & _info.MaxTimestamp;
    public long WorkerId => (_id >> _info.WorkerIdShift) & _info.MaxWorkerId;
    public long Sequence => _id & _info.MaxSequence;

    public override string ToString() =>
        $"SnowflakeId[Id={_id}, unit {_info.TimeUnit},Time={Timestamp}, Worker={WorkerId}, Seq={Sequence}]";
}