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