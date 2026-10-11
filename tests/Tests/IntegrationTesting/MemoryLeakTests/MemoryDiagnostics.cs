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


using System.Runtime;

namespace MemoryLeakTests;

public static class MemoryDiagnostics
{
    /// <summary>
    /// 获取稳定的内存快照（多次采样取最小值，剔除GC抖动）
    /// </summary>
    public static long GetStableMemorySnapshot(int samples = 3, int delayMs = 0)
    {
        GCSettings.LargeObjectHeapCompactionMode = GCLargeObjectHeapCompactionMode.CompactOnce;
        //var snapshot = dotMemory.Check();
        for (int i = 0; i < samples; i++)
        {
            // 强制完全GC（2轮确保Finalizer对象也被清理）
            GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced, blocking: true, compacting: true);
            GC.WaitForPendingFinalizers();
            //GC.Collect(GC.MaxGeneration, GCCollectionMode.Aggressive, blocking: true, compacting: true);
            //GC.WaitForPendingFinalizers();
            if (delayMs != 0)
            {
                Thread.Sleep(delayMs);
            }

        }
        return GC.GetTotalMemory(true);
        //return GC.GetGCMemoryInfo().TotalCommittedBytes;
    }
}