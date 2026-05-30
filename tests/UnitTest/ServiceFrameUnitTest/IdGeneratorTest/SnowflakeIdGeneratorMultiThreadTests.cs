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


using FluentAssertions;
using Proxar.IdGenerator.SnowflakeId;
using Proxar.Utilities;
using System.Collections.Concurrent;

namespace FrameUnitTest.IdGeneratorTest;

public class SnowflakeIdGeneratorMultiThreadTests
{
    /// <summary>
    /// 多线程环境下ID生成的正确性验证（支持配置线程数、位宽、每线程ID量）
    /// </summary>
    /// <param name="workerId">机器ID（极少修改，放最前面）</param>
    /// <param name="threadCount">并发线程数</param>
    /// <param name="timestampBits">时间戳位宽</param>
    /// <param name="workerIdBits">机器ID位宽</param>
    /// <param name="sequenceBits">序列号位宽</param>
    /// <param name="baseYear">基础年份</param>
    /// <param name="idsPerThread">每个线程生产的ID数量（可配置）</param>
    [Theory]
    [InlineData(1, 10, 39, 15, 9, 2024, 1000000)]
    [InlineData(1, 10, 39, 22, 2, 2024, 1000000)]
    [InlineData(1, 10, 39, 23, 1, 2024, 1000000)]
    [InlineData(5, 20, 41, 9, 13, 2020, 1000000)]
    [InlineData(3, 50, 38, 11, 14, 2025, 1000000)]
    public async Task MultiThreadedGeneration_ShouldProduceUniqueAndValidIds(
        long workerId,
        int threadCount,
        int timestampBits,
        int workerIdBits,
        int sequenceBits,
        int baseYear,
        int idsPerThread)
    {

        var baseTime = TimeHelper.GetSecondByStr($"{baseYear}-01-01 00:00:00");
        ISnowflakeInfo snowflakeInfo = new SnowflakeInfo(timestampBits, workerIdBits, sequenceBits, TimeUnit.Seconds, baseTime);
        // Arrange
        var generator = new SnowflakeIdGenerator(workerId, snowflakeInfo);

        long maxSequence = snowflakeInfo.MaxSequence;
        long maxWorkerId = snowflakeInfo.MaxWorkerId;
        int workerIdShift = snowflakeInfo.WorkerIdShift;
        int timestampShift = snowflakeInfo.TimestampShift;

        // 启动多线程生成ID（每线程生产 idsPerThread 个）
        var tasks = Enumerable.Range(0, threadCount)
              .Select(_ => Task.Run(() =>
              {
                  var localIds = new List<long>(idsPerThread);
                  for (int i = 0; i < idsPerThread; i++)
                  {
                      localIds.Add(generator.NewId());
                  }

                  // 验证ID各部分符合位配置
                  foreach (var id in localIds)
                  {
                      // 提取时间戳、机器ID、序列号
                      var snowflakeIdStruct = snowflakeInfo.ParseId(id);

                      // 验证机器ID正确
                      snowflakeIdStruct.WorkerId.Should().Be(workerId, "ID中的机器ID必须与配置的workerId完全一致");

                      // 验证序列号在合法范围
                      snowflakeIdStruct.Sequence.Should().BeInRange(0, maxSequence, "序列号不能超过配置的位宽上限");

                      // 验证时间戳非负
                      snowflakeIdStruct.Timestamp.Should().BeGreaterThanOrEqualTo(0, "时间戳必须基于baseYear非负偏移");
                  }
                  return localIds;
              }));

        var results = await Task.WhenAll(tasks);

        var ids = new List<long>(threadCount * idsPerThread);
        foreach (var localIds in results)
        {
            ids.AddRange(localIds);  // 直接内存复制，无枚举器开销
        }
        //HasDuplicates(ids).Should().BeFalse();
        HasNoDuplicatesParallel(ids).Should().BeTrue();

        // 总ID数量
        ids.Should().HaveCount(threadCount * idsPerThread, "所有线程生成的ID总数必须等于 线程数 × 每线程ID量");

        ids.Count.Should().Be(threadCount * idsPerThread);
        // 无重复ID
        //ids.Should().OnlyHaveUniqueItems("多线程并发下必须保证ID全局唯一");


    }

    public static bool HasNoDuplicatesParallel<T>(IReadOnlyList<T> list) where T : struct, IEquatable<T>
    {
        // 取消令牌：发现重复 → 立即终止所有并行线程
        using var cts = new CancellationTokenSource();
        // 线程安全集合（模拟HashSet，值用byte最小内存）
        var uniqueSet = new ConcurrentDictionary<T, byte>();
        // 标记是否发现重复
        bool hasDuplicate = false;

        try
        {
            // 并行遍历：自动分配CPU多核，最大化利用率
            Parallel.ForEach(list,
                new ParallelOptions
                {
                    CancellationToken = cts.Token,  // 绑定取消令牌
                    MaxDegreeOfParallelism = Environment.ProcessorCount // 用满CPU核心
                },
                item =>
                {
                    // 尝试添加元素：失败=已存在 → 发现重复
                    if (!uniqueSet.TryAdd(item, 0))
                    {
                        hasDuplicate = true;
                        cts.Cancel(); // 立即取消所有线程
                    }
                });
        }
        catch (OperationCanceledException)
        {
            // 取消异常是预期行为，直接忽略
        }

        // 无重复返回true，有重复返回false
        return !hasDuplicate;
    }
}