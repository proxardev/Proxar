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
using Proxar.IdGenerator;
using Proxar.ServiceCore;
using Proxar.Tasks;
using Proxar.Timer;
using Proxar.Utilities;

namespace ServiceIntegrationTesting;


public static class RunTestHelper
{
    public static void Run(Action action)
    {
        action.Invoke();
        TestResultHelper.SetFinishTest();
    }
    public static async ZFTask Run(Func<ZFTask> action)
    {
        await action();
        TestResultHelper.SetFinishTest();
    }
}

public partial class TestService : ServiceBase
{
    private Int32IdGenerator ResponseIdGenerator = new Int32IdGenerator();


    [ServiceMethod(2)]
    private async ZFTask Call_Should_Response()
    {
        var targetServiceId = Service.CreateService<TestService>();
        var proxy = Service.GetServiceProxy<TestServiceProxy>(targetServiceId);
        for (var i = 0; i < 100; i++)
        {
            var res = await proxy.Response(i);
            var randomA = Random.Shared.Next(100000);
            var randomB = Random.Shared.Next(100000);

            var randomNumAddRes = await proxy.AdditinAndResponse(randomA, randomB);

            res.Should().Be(i);
            randomNumAddRes.Should().Be(randomA + randomB);
        }
        TestResultHelper.SetFinishTest();
    }

    [ServiceMethod(3)]
    private int Response(int data)
    {
        return data;
    }

    [ServiceMethod(4)]
    private int AdditinAndResponse(int a, int b)
    {
        return a + b;
    }


    [ServiceMethod(13)]
    private async ZFTask Call_MullCall_ReturnInOrder()
    {
        var testServiceId = Service.CreateService<TestService>();
        var testServiceProxy = Service.GetServiceProxy<TestServiceProxy>(testServiceId);
        var count = 10000;
        var taskList = RangeHelper.Range(count)
            .Select(x => testServiceProxy.ResponseId())
            .ToList();
        var resList = await Proxar.Tasks.ZFTaskHelper.WhenAll(taskList);
        resList.Should().NotBeNullOrEmpty();
        resList.Count.Should().Be(count);
        var resSet = resList.ToHashSet();
        resSet.Count.Should().Be(resList.Count);

        var first = resList[0];
        foreach (var item in resList.Skip(1))
        {
            first.Should().Be(item - 1);
            first = item;
        }
        TestResultHelper.SetFinishTest();
    }

    [ServiceMethod(14)]
    private int ResponseId()
    {
        return ResponseIdGenerator.NewId();
    }

    [ServiceMethod(31)]
    public async ZFTask Timer_CreateTimer_TimeoutExecMethodOnlyOneTimes()
    {
        var timerExec = false;
        var task = ZFTask.CreateZFTask();
        int times = 0;
        var action = () =>
        {
            times++;
            timerExec = true;
            task.SetResult();
        };
        TimerHelper.TimerCall(1, action);

        await task;

        await TimerHelper.Delay(100);
        times.Should().Be(1);

        timerExec.Should().BeTrue();

        TestResultHelper.SetFinishTest();

    }

    [ServiceMethod(32)]
    private async ZFTask Timer_Delay_DelaySucc()
    {
        var msTime = 500;
        var start = TimeHelper.GetMSSecond();
        await TimerHelper.Delay(msTime);
        var end = TimeHelper.GetMSSecond();
        var time = end - start;
        //time.Should().BeLessThanOrEqualTo(msTime + 100);
        time.Should().BeGreaterThanOrEqualTo(msTime);
        TestResultHelper.SetFinishTest();
    }

    [ServiceMethod(33)]
    private async ZFTask Timer_InternalCall_CallOverOneTimes()
    {
        await InternalCall_CallOverOneTimes();
        await InternalCall_CallOverOneTimes2();


        TestResultHelper.SetFinishTest();
    }

    private async ZFTask InternalCall_CallOverOneTimes()
    {
        int times = 0;
        var task = ZFTask.CreateZFTask();
        var timerId = 0L;
        int callTime = 5;
        var action = () =>
        {
            times++;
            if (times >= callTime)
            {
                task.SetResult();
                TimerHelper.CancelTimer(timerId);
            }
        };
        timerId = TimerHelper.IntervalTimerCall(10, action);
        await task;

        times.Should().Be(callTime);
    }


    private async ZFTask InternalCall_CallOverOneTimes2()
    {
        int times = 0;
        int allTimes = 5;
        var task = ZFTask.CreateZFTask();
        var timerId = 0L;
        var argsSum = 0;
        var randonArgs = RandomIntDataFunc();
        var action = (int args) =>
        {
            times++;
            argsSum += args;
            if (times >= allTimes)
            {
                TimerHelper.CancelTimer(timerId);
                task.SetResult();
            }
        };
        timerId = TimerHelper.IntervalTimerCall(10, action, randonArgs);

        TimerHelper.HasTimer(timerId).Should().BeTrue();
        await task;

        times.Should().Be(allTimes);
        argsSum.Should().Be(allTimes * randonArgs);
        TimerHelper.HasTimer(timerId).Should().BeFalse();
    }

    [ServiceMethod(34)]
    private async ZFTask Timer_CancelTimer_ShouldNotExecute()
    {
        var exec = false;
        var action = () =>
        {
            exec = true;
        };
        var timerId = TimerHelper.IntervalTimerCall(1, action);
        TimerHelper.HasTimer(timerId).Should().BeTrue();
        TimerHelper.CancelTimer(timerId);


        TimerHelper.HasTimer(timerId).Should().BeFalse();

        await TimerHelper.Delay(10);
        exec.Should().BeFalse();
        TimerHelper.HasTimer(timerId).Should().BeFalse();

        TestResultHelper.SetFinishTest();
    }

    [ServiceMethod(35)]
    private async ZFTask Timer_CancelAllTimer_AllTimerShouldStop(int count)
    {
        var timerList = RangeHelper.Range(count)
            .Select(
            x =>
            {
                var exec = false;
                var validExecFunc = () =>
                {
                    return exec;
                };
                var action = () =>
                {
                    exec = true;
                };
                var timerId = TimerHelper.IntervalTimerCall(1, action);

                return (validExecFunc, timerId);
            }
            )
            .ToList();

        timerList.Count.Should().Be(count);

        foreach (var (validExecFunc, timerId) in timerList)
        {
            validExecFunc().Should().BeFalse();
            TimerHelper.HasTimer(timerId).Should().BeTrue();
        }

        TimerHelper.CancelAllTimer();

        foreach (var (validExecFunc, timerId) in timerList)
        {
            validExecFunc().Should().BeFalse();
            TimerHelper.HasTimer(timerId).Should().BeFalse();
        }


        await TimerHelper.Delay(10);

        foreach (var (validExecFunc, timerId) in timerList)
        {
            validExecFunc().Should().BeFalse();
            TimerHelper.HasTimer(timerId).Should().BeFalse();
        }

        TestResultHelper.SetFinishTest();
    }



    public int RandomIntDataFunc()
    {
        return Random.Shared.Next();
    }

    public string RandomStringDataFunc()
    {
        return Random.Shared.Next().ToString();
    }


    [ServiceMethod(50)]
    private async ZFTask ActorSingleton_GetActorSingleton_ActorSingletonShouldBelongThisService(int createServiceCount, int iterHandlerCnt, int iterDelayMSTime)
    {
        var serviceProxys = RangeHelper.Range(createServiceCount)
            .Select(
                x =>
                {
                    var serviceId = Service.CreateService<TestService2>();
                    return Service.GetServiceProxy<TestService2Proxy>(serviceId);
                }
            )
            .ToList();
        var taskList = serviceProxys
            .Select(
                x =>
                {
                    return x.IterActorSingletonIdGenerattor_CheckThreadSafe(iterHandlerCnt, iterDelayMSTime);
                    //return Service.Result<Exception>().Call(x, TestService2_Proto.Pro_1_IterNewActorSingletonIdGenerattorIdAndCheckThreadSafe, iterHandlerCnt, iterDelayMSTime);
                }
            )
            .ToList();
        var resList = await Proxar.Tasks.ZFTaskHelper.WhenAll(taskList);

        foreach (var serviceId in serviceProxys)
        {
            serviceId.ExecuteServiceClose().Coroutine();
        }

        resList.Count.Should().Be(createServiceCount);
        foreach (var res in resList)
        {
            if (res != string.Empty)
            {
                var ex = new Exception(res);
                TestResultHelper.SetFinishTest(ex);
                return;
            }
        }

        TestResultHelper.SetFinishTest();
    }
}