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


using Proxar.ServiceCore;
using Proxar.Tasks;
using Proxar.Timer;
using Proxar.Timer.Interfaces;
using Proxar.Utilities;
using ServiceIntegrationTesting;

namespace MemoryLeakTests;

internal partial class TestMemoryService : ServiceBase
{

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

    [ServiceMethod(10)]
    private async ZFTask GlobalTimer_CreateAndDestory_NotLeak(int cnt)
    {
        var leakInfo = await Timer_CreateAndDestory_NotLeak(
            TimerActorSingleton.Current,
            cnt
            );
        TestResultHelper.SetFinishTest(info: $"内存泄漏量 {leakInfo}");

    }

    [ServiceMethod(11)]
    private async ZFTask EntityTimer_CreateAndDestory_NotLeak(int cnt)
    {
        var entity = new TimerEntityWith1MB();
        var leakInfo = await Timer_CreateAndDestory_NotLeak(
            entity,
            cnt
            );
        TestResultHelper.SetFinishTest(info: $"内存泄漏量 {leakInfo}");

    }

    [ServiceMethod(100)]
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

            await Proxar.Tasks.ZFTaskHelper.WhenAll(tasks);

            tasks = testMemoryServiceProxys
                .Select(proxy => proxy.ExecuteServiceClose())
                .ToList();
            await Proxar.Tasks.ZFTaskHelper.WhenAll(tasks);
            await ZFTask.CompletedTask;
        };

        var leakInfo = await MemoryLeakTestHelper.AssertMemoryLeak(
            createAndDestoryAction
            );
        TestResultHelper.SetFinishTest(info: $"内存泄漏量 {leakInfo}");
    }

    private List<int> testMemoryLeakData = null!;

    [ServiceMethod(101)]
    private int Create1MBData()
    {
        testMemoryLeakData = new List<int>(1024 * 1024 / 4);
        return 1;
    }


    [ServiceMethod(110)]
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

            await Proxar.Tasks.ZFTaskHelper.WhenAll(tasks);
        };

        var leakInfo = await MemoryLeakTestHelper.AssertMemoryLeak(
            callMethod
            );
        foreach (var testMemoryServiceProxy in testMemoryServiceProxys)
        {
            await testMemoryServiceProxy.ExecuteServiceClose();
        }
        TestResultHelper.SetFinishTest(info: $"内存泄漏量 {leakInfo}");
    }

    [ServiceMethod(120)]
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

            await Proxar.Tasks.ZFTaskHelper.WhenAll(tasks);
        }
        ;

        foreach (var testMemoryServiceProxy in testMemoryServiceProxys)
        {
            await testMemoryServiceProxy.ExecuteServiceClose();
        }
        TestResultHelper.SetFinishTest();
    }

    [ServiceMethod(111)]
    private int CallMethodTest(List<int> datas)
    {
        return 0;
    }


    [ServiceMethod(112)]
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
        TestResultHelper.SetFinishTest(info: $"内存泄漏量 {leakInfo}");
    }

    [ServiceMethod(113)]
    private void SendMethodTest(List<int> datas)
    {
    }
}