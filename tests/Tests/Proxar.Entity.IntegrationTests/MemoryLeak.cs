using TestShared.TestClass;
using Xunit.Abstractions;

namespace MemoryLeakTests
{
    public class MemoryLeak : TestClassBaseWithOutput
    {
        public MemoryLeak(ITestOutputHelper output) : base(output)
        {
        }

        [Theory]
        [InlineData(nameof(TestMemoryServiceProxy.Entity_CreateOnly_Leak), 1000)]
        [InlineData(nameof(TestMemoryServiceProxy.Entity_CreateAndDestory_NotLeak), 1000)]
        [InlineData(nameof(TestMemoryServiceProxy.TimerEntity_CreateAndDestory_NotLeak), 1000)]
        [InlineData(nameof(TestMemoryServiceProxy.GlobalTimer_CreateAndDestory_NotLeak), 1000)]
        [InlineData(nameof(TestMemoryServiceProxy.EntityTimer_CreateAndDestory_NotLeak), 1000)]
        [InlineData(nameof(TestMemoryServiceProxy.Service_CreateAndDestory_NotLeak), 10000)]
        [InlineData(nameof(TestMemoryServiceProxy.Service_CallMethod_NotLeak), 10000)]
        internal async Task CallTargetProtoMethod(string methodName, int args = int.MinValue, int args2 = int.MinValue,
            int args3 = int.MinValue,
            int args4 = int.MinValue
            )
        {
            await this.CallMethodByProxy<TestMemoryService, TestMemoryServiceProxy>(methodName, args, args2, args3, args4);
        }

        //[Theory]
        //[InlineData(nameof(TestMemoryServiceProxy.Service_CallMethod_NotLeak2), 10000)]
        //[DotMemoryUnit(FailIfRunWithoutSupport = false)]
        //internal async Task CallTargetProtoMethod2(string methodName, int args = int.MinValue, int args2 = int.MinValue,
        //    int args3 = int.MinValue,
        //    int args4 = int.MinValue
        //    )
        //{

        //    await CheckCreateTargetTestService<TestMemoryService>();

        //    var baseCheck = dotMemory.Check();
        //    await this.CallMethod<TestMemoryService, TestMemoryServiceProxy>(methodName, args, args2, args3, args4);
        //    var after = dotMemory.Check((check) =>
        //    {
        //        var size = check.GetTrafficFrom(baseCheck).AllocatedMemory.SizeInBytes;
        //        size.Should().BeLessThan(1024 * 1024 * 4);
        //    });
        //}
    }

}