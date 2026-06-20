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
/// 表示一个已解析的雪花算法 ID，提供对时间戳、工作节点 ID 和序列号的只读访问。
/// </summary>
public readonly struct SnowflakeIdStruct
{
    private readonly long _id;
    private readonly ISnowflakeInfo _info;

    /// <summary>
    /// 使用原始 ID 值和信息提供者初始化结构体。信息提供者会在首次访问时延迟调用，并缓存结果。
    /// </summary>
    /// <param name="id">雪花算法生成的原始 64 位 ID。</param>
    /// <param name="infoProvider">用于获取 <see cref="ISnowflakeInfo"/> 配置的工厂委托。</param>
    public SnowflakeIdStruct(long id, Func<ISnowflakeInfo> infoProvider)
        : this(infoProvider)
    {
        _id = id;
    }

    /// <summary>
    /// 使用原始 ID 值和已有的 <see cref="ISnowflakeInfo"/> 实例初始化结构体。
    /// </summary>
    /// <param name="id">雪花算法生成的原始 64 位 ID。</param>
    /// <param name="snowflakeInfo">
    /// 定义雪花算法位宽、基准时间戳等参数的 <see cref="ISnowflakeInfo"/> 配置，
    /// 用于解析 <paramref name="id"/> 中的时间戳、工作节点 ID 和序列号。
    /// </param>
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

    /// <summary>
    /// 获取原始雪花 ID 值。
    /// </summary>
    public long Id => _id;

    /// <summary>
    /// 获取从 ID 中解析出的时间戳部分（相对于 <see cref="ISnowflakeInfo.BaseTimestamp"/> 的偏移量）。
    /// </summary>
    public long Timestamp => (_id >> _info.TimestampShift) & _info.MaxTimestamp;

    /// <summary>
    /// 获取从 ID 中解析出的工作节点 ID。
    /// </summary>
    public long WorkerId => (_id >> _info.WorkerIdShift) & _info.MaxWorkerId;

    /// <summary>
    /// 获取从 ID 中解析出的序列号。
    /// </summary>
    public long Sequence => _id & _info.MaxSequence;

    /// <inheritdoc/>
    public override string ToString() =>
        $"SnowflakeId[Id={_id}, unit {_info.TimeUnit},Time={Timestamp}, Worker={WorkerId}, Seq={Sequence}]";
}