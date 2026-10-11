using Proxar.Entity;

namespace MemoryLeakTests;

internal class TimerEntityWith1MB : TimerEntityBase
{
    public List<int> data = new List<int>(1024 * 1024 / 4);
}