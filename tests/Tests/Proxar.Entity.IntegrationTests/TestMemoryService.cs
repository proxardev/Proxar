using Proxar.ServiceCore;
using Proxar.Tasks;
using Proxar.Timer;
using Proxar.Timer.Interfaces;
using Proxar.Utilities;
using ServiceIntegrationTesting;

namespace MemoryLeakTests;

internal partial class TestMemoryService : ServiceBase
{

    private void CreateEntity<T>(int entityCnt, List<T> entityWith1MBs)
        where T : EntityBase, new()
    {
        var root = EntityRoot.GetRoot();
        for (int i = 0; i < entityCnt; i++)
        {
            var entity = root.AddChild<T>();
            entityWith1MBs.Add(entity);
        }
    }

    private void CreateAndDesoryEntity<T>(int entityCnt)
        where T : EntityBase, new()
    {
        var root = EntityRoot.GetRoot();
        for (int i = 0; i < entityCnt; i++)
        {
            var entity = root.AddChild<T>();
            entity.Dispose();
        }
        for (int i = 0; i < entityCnt; i++)
        {
            var entity = root.AddComponent<T>();
            entity.Dispose();
        }
        var round = entityCnt / 100;
        for (int i = 0; i < round; i++)
        {
            var entityTestChainChild = root.AddChild<T>();
            var entity = entityTestChainChild;
            for (int j = 0; j < 100; j++)
            {
                entity = entity.AddChild<T>();

            }
            entityTestChainChild.Dispose();
        }




    }

    [ServiceMethod]
    private async ZFTask Entity_CreateOnly_Leak(int entityCnt)
    {
        var entityWith1MBs = new List<EntityWith1MB>();
        var leakInfo = await MemoryLeakTestHelper.AssertMemoryLeak(
            async () =>
            {
                CreateEntity(entityCnt, entityWith1MBs);
                await ZFTask.CompletedTask;
            },
            maxAllowedIncreaseBytes: 1024 * 1024 * entityCnt,
            expectLeak: true
            );
        foreach (var entity in entityWith1MBs)
        {
            entity.Dispose();
        }
        TestResultHelper.SetFinishTest(info: $"�ڴ�й©�� {leakInfo}");
    }


    [ServiceMethod]
    private async ZFTask Entity_CreateAndDestory_NotLeak(int entityCnt)
    {
        var leakInfo = await MemoryLeakTestHelper.AssertMemoryLeak(
            async () =>
            {
                CreateAndDesoryEntity<EntityWith1MB>(entityCnt);
                await ZFTask.CompletedTask;
            }
            );
        TestResultHelper.SetFinishTest(info: $"�ڴ�й©�� {leakInfo}");
    }


    [ServiceMethod]
    private async ZFTask TimerEntity_CreateAndDestory_NotLeak(int entityCnt)
    {
        var leakInfo = await MemoryLeakTestHelper.AssertMemoryLeak(
            async () =>
            {
                CreateAndDesoryEntity<TimerEntityWith1MB>(entityCnt);
                await ZFTask.CompletedTask;
            }
            );
        TestResultHelper.SetFinishTest(info: $"�ڴ�й©�� {leakInfo}");
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="timerObject"></param>
    /// <param name="cnt"></param>
    /// <returns></returns>
    private async ZFTask<string> Timer_CreateAndDestory_NotLeak(ITimerObject timerObject, int cnt)
    {
        var singleCnt = cnt / MemoryLeakTestHelper.TestRoundCnt;
        var time = 1L;
        Func<ZFTask> createAndDestoryAction = async () =>
        {
            for (int i = 0; i < singleCnt; i++)
            {

                var data = new List<int>(1024 * 1024 / 4);
                var action = () => { var cnt = data.Count; };
                timerObject.TimerCall(time, action);
                var timerId = timerObject.IntervalTimerCall(time, action);
                timerObject.CancelTimer(timerId);


                var action2 = (int i) => { var cnt = data.Count; };
                timerObject.TimerCall(time, action2, i);
                timerId = timerObject.IntervalTimerCall(time, action2, i);
                timerObject.CancelTimer(timerId);
            }
            await timerObject.Delay(time + 1);
            await ZFTask.CompletedTask;
        };

        var leakInfo = await MemoryLeakTestHelper.AssertMemoryLeak(
            createAndDestoryAction
            );
        return leakInfo;

    }

    [ServiceMethod]
    private async ZFTask GlobalTimer_CreateAndDestory_NotLeak(int cnt)
    {
        var leakInfo = await Timer_CreateAndDestory_NotLeak(
            TimerActorSingleton.Current,
            cnt
            );
        TestResultHelper.SetFinishTest(info: $"�ڴ�й©�� {leakInfo}");

    }

    [ServiceMethod]
    private async ZFTask EntityTimer_CreateAndDestory_NotLeak(int cnt)
    {
        var entity = EntityRoot.GetRoot().AddChild<TimerEntityWith1MB>();
        var leakInfo = await Timer_CreateAndDestory_NotLeak(
            entity,
            cnt
            );
        TestResultHelper.SetFinishTest(info: $"�ڴ�й©�� {leakInfo}");

    }

    [ServiceMethod]
    private async ZFTask Service_CreateAndDestory_NotLeak(int cnt)
    {
        var singleCnt = cnt / MemoryLeakTestHelper.TestRoundCnt;
        Func<ZFTask> createAndDestoryAction = async () =>
        {
            var testMemoryServiceProxys = new List<TestMemoryServiceProxy>();
            for (int i = 0; i < singleCnt; i++)
            {
                var serviceId = Service.CreateService<TestMemoryService>();
                var testMemoryServiceProxy = Service.GetServiceProxy<TestMemoryServiceProxy>(serviceId);
                testMemoryServiceProxys.Add(testMemoryServiceProxy);
            }
            var tasks = testMemoryServiceProxys
                .Select(proxy => proxy.Create1MBData())
                .ToList();

            await TaskHelper.WhenAll(tasks);

            tasks = testMemoryServiceProxys
                .Select(proxy => proxy.ExecuteServiceClose())
                .ToList();
            await TaskHelper.WhenAll(tasks);
            await ZFTask.CompletedTask;
        };

        var leakInfo = await MemoryLeakTestHelper.AssertMemoryLeak(
            createAndDestoryAction
            );
        TestResultHelper.SetFinishTest(info: $"�ڴ�й©�� {leakInfo}");
    }

    private List<int> testMemoryLeakData = null!;

    [ServiceMethod]
    private int Create1MBData()
    {
        testMemoryLeakData = new List<int>(1024 * 1024 / 4);
        return 1;
    }


    [ServiceMethod]
    private async ZFTask Service_CallMethod_NotLeak(int cnt)
    {
        var datas = new List<int>(1024 * 1024 / 4);
        for (int i = 0; i < datas.Capacity; i++)
        {
            datas.Add(i);
        }

        var testMemoryServiceProxys = RangeHelper.Range(10)
             .Select(
                 x =>
                 {
                     var serviceId = Service.CreateService<TestMemoryService>();
                     return Service.GetServiceProxy<TestMemoryServiceProxy>(serviceId);
                 }
             )
             .ToList();


        var singleCnt = cnt / MemoryLeakTestHelper.TestRoundCnt;
        Func<ZFTask> callMethod = async () =>
        {
            var randomDatas = new List<int>(datas);
            var tasks = RangeHelper.Range(singleCnt)
                .Select(_ => RandomHelper.RandomList(testMemoryServiceProxys).CallMethodTest(randomDatas))
                .ToList();

            await TaskHelper.WhenAll(tasks);
        };

        var leakInfo = await MemoryLeakTestHelper.AssertMemoryLeak(
            callMethod
            );
        foreach (var testMemoryServiceProxy in testMemoryServiceProxys)
        {
            await testMemoryServiceProxy.ExecuteServiceClose();
        }
        TestResultHelper.SetFinishTest(info: $"�ڴ�й©�� {leakInfo}");
    }

    [ServiceMethod]
    private async ZFTask Service_CallMethod_NotLeak2(int cnt)
    {
        var datas = new List<int>(1024 * 1024 / 4);
        for (int i = 0; i < datas.Capacity; i++)
        {
            datas.Add(i);
        }

        var testMemoryServiceProxys = RangeHelper.Range(10)
             .Select(
                 x =>
                 {
                     var serviceId = Service.CreateService<TestMemoryService>();
                     return Service.GetServiceProxy<TestMemoryServiceProxy>(serviceId);
                 }
             )
             .ToList();


        var singleCnt = cnt / MemoryLeakTestHelper.TestRoundCnt;
        for (int i = 0; i < 10; i++)
        {
            var randomDatas = new List<int>(datas);
            var tasks = RangeHelper.Range(singleCnt)
                .Select(_ => RandomHelper.RandomList(testMemoryServiceProxys).CallMethodTest(randomDatas))
                .ToList();

            await TaskHelper.WhenAll(tasks);
        }
        ;

        foreach (var testMemoryServiceProxy in testMemoryServiceProxys)
        {
            await testMemoryServiceProxy.ExecuteServiceClose();
        }
        TestResultHelper.SetFinishTest();
    }

    [ServiceMethod]
    private int CallMethodTest(List<int> datas)
    {
        return 0;
    }


    [ServiceMethod]
    private async ZFTask Service_SendMethod_NotLeak(int cnt)
    {
        var datas = new List<int>(1024 * 1024 / 4);
        for (int i = 0; i < datas.Capacity; i++)
        {
            datas.Add(i);
        }

        var serviceId = Service.CreateService<TestMemoryService>();
        var testMemoryServiceProxy = Service.GetServiceProxy<TestMemoryServiceProxy>(serviceId);


        var singleCnt = cnt / MemoryLeakTestHelper.TestRoundCnt;
        Func<ZFTask> callMethod = async () =>
        {
            var randomDatas = new List<int>(datas);
            for (int i = 0; i < singleCnt; i++)
            {
                testMemoryServiceProxy.SendMethodTest(randomDatas);
            }

            await testMemoryServiceProxy.ExecuteEchoConfirm();
        };
        await testMemoryServiceProxy.ExecuteServiceClose();

        var leakInfo = await MemoryLeakTestHelper.AssertMemoryLeak(
            callMethod
            );
        TestResultHelper.SetFinishTest(info: $"�ڴ�й©�� {leakInfo}");
    }

    [ServiceMethod]
    private void SendMethodTest(List<int> datas)
    {
    }
}