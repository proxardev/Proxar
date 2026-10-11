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


using Proxar.Tasks;
namespace MemoryLeakTests;

/// <summary>
/// 内存泄漏测试类
/// </summary>
public static class MemoryLeakTestHelper
{

    public const int TestRoundCnt = 10;

    /// <summary>
    /// 内存泄漏检测：执行N轮，看内存是否持续上涨
    /// </summary>
    /// <param name="action">每轮执行的操作</param>
    /// <param name="rounds">测试轮次</param>
    /// <param name="maxAllowedIncreaseBytes">最大允许增长（默认 8MB）</param>
    public static async ZFTask<string> AssertMemoryLeak(
        Func<ZFTask> action,
        int rounds = TestRoundCnt,
        int maxAllowedIncreaseBytes = 50 * 1024 * 1024,
        bool expectLeak = false)
    {
        // 预热
        for (int i = 0; i < 1; i++)
        {
            await action();
        }


        long baseMem = MemoryDiagnostics.GetStableMemorySnapshot();
        long lastMem = baseMem;
        long totalDelta = 0;

        for (int round = 0; round < rounds; round++)
        {
            await action();

            long currentMem = MemoryDiagnostics.GetStableMemorySnapshot();
            totalDelta = currentMem - baseMem;

            if (totalDelta > maxAllowedIncreaseBytes)
            {
                for (int i = 0; i < 5; i++)
                {
                    currentMem = MemoryDiagnostics.GetStableMemorySnapshot(delayMs: 50);
                    totalDelta = currentMem - baseMem;
                    if (totalDelta < maxAllowedIncreaseBytes)
                    {
                        break;
                    }
                }
            }

            // 正常逻辑：必须 **不泄漏**
            if (!expectLeak)
            {
                if (totalDelta > maxAllowedIncreaseBytes)
                {
                    Assert.Fail(
                        $"\n内存泄漏（不允许）{round}轮\n" +
                        $"初始内存：{baseMem / 1024} KB\n" +
                        $"当前内存：{currentMem / 1024} KB\n" +
                        $"增长：{totalDelta / 1024} KB\n" +
                        $"阈值：{maxAllowedIncreaseBytes / 1024} KB");
                }
            }

            // 对照逻辑：必须 **泄漏**
            else
            {
                if (totalDelta > maxAllowedIncreaseBytes)
                {
                    // 泄漏成功,不用继续跑
                    return totalDelta.ToMemoryString();
                }
            }

            lastMem = currentMem;
        }

        // 如果是 期望泄漏，但跑完所有轮次都没泄漏 → 失败
        if (expectLeak)
        {
            Assert.Fail(
                $"\n测试无效：期望泄漏，但未检测到泄漏\n" +
                $"初始内存：{baseMem / 1024} KB\n"
                + $"最终内存：{lastMem / 1024} KB\n"
                + $"增长：{(lastMem - baseMem) / 1024} KB");
        }
        return totalDelta.ToMemoryString();
    }

    public static string ToMemoryString(this long self)
    {
        return $"{self / 1024.0 / 1024.0} MB";
    }
}