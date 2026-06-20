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
using Proxar.ActorSingletonCore;
using Proxar.ServiceCore;
using Proxar.Tasks;
using Proxar.Timer;
using Proxar.Timer.Interfaces;
using Proxar.Utilities;

namespace ServiceIntegrationTesting;

internal class TimerClassAbstract : ActorSingleton<TimerClassAbstract>
{
    public ITimerObject TimerObject { get; set; } = null!;
    private int mode = 0;

    public void InitTestTimerObject(int mode)
    {
        this.mode = mode;
        if (mode == 0)
        {
            TimerObject = TimerActorSingleton.Current;
        }
        else if (mode == 1)
        {
            TimerObject = new TestTimerEntity();
        }
    }


    public static ITimerObject GetTimerObject()
    {
        return TimerClassAbstract.Current.TimerObject;
    }

    public int GetMode()
    {
        return this.mode;
    }

    public override void Dispose()
    {
    }
}

public class TestTimerEntity : TimerBase
{

}


internal partial class TestService_Timer : ServiceBase
{

    [ServiceMethod(800)]
    private void InitTimerClassAbs(int mode)
    {
        TimerClassAbstract.Current.InitTestTimerObject(mode);
    }


    private void CheckTimerCallBackCost(long start, int delayMilliSecond, int ratio = 90)
    {
        var end = TimeHelper.GetMSSecond();
        var cost = end - start;
        var diff = Math.Abs(delayMilliSecond * ratio / 100.0);
        cost.Should().BeGreaterThanOrEqualTo(delayMilliSecond);
    }

    [ServiceMethod(1)]
    private async ZFTask Delay_ExecuteAfterDelay(int delayMilliSecond)
    {
        var start = TimeHelper.GetMSSecond();

        await TimerClassAbstract.GetTimerObject().Delay(delayMilliSecond);

        CheckTimerCallBackCost(start, delayMilliSecond);

        TestResultHelper.SetFinishTest();
    }

    [ServiceMethod(2)]
    private async ZFTask MullDelay_ExecuteAfterDelay()
    {
        var start = TimeHelper.GetMSSecond();
        var list = new List<int>();

        await TimerClassAbstract.GetTimerObject().Delay(30);
        list.Add(1);
        CheckTimerCallBackCost(start, 30);


        await TimerClassAbstract.GetTimerObject().Delay(20);
        list.Add(2);
        CheckTimerCallBackCost(start, 50);


        await TimerClassAbstract.GetTimerObject().Delay(10);
        list.Add(3);
        CheckTimerCallBackCost(start, 60);

        list.Should().Equal(1, 2, 3);

        TestResultHelper.SetFinishTest();
    }

    [ServiceMethod(3)]
    private async ZFTask LoopDelay_ExecuteAfterDelay(int loop)
    {
        var start = TimeHelper.GetMSSecond();
        var delayTimes = 0;
        for (int i = 0; i < loop; i++)
        {
            await TimerClassAbstract.GetTimerObject().Delay(10);
            delayTimes++;
        }

        CheckTimerCallBackCost(start, 10 * loop);

        delayTimes.Should().Be(loop);

        TestResultHelper.SetFinishTest();
    }

    [ServiceMethod(11)]
    private async ZFTask TimerCallBack_CallBackSucc(int delayMilliSecond, int timerCnt = 1)
    {
        var start = TimeHelper.GetMSSecond();
        var task = ZFTask.CreateZFTask();
        var execute = false;
        int execCnt = 0;
        var action = () =>
        {
            execCnt++;
            if (execCnt == timerCnt)
            {
                task.SetResult();
            }
            execute = true;
        };
        var timerIdList = RangeHelper.Range(timerCnt)
            .Select(
            _ =>
            {
                return TimerClassAbstract.GetTimerObject().TimerCall(delayMilliSecond, action);
            }
            )
            .ToList();

        foreach (var timerId in timerIdList)
        {
            TimerClassAbstract.GetTimerObject().HasTimer(timerId).Should().BeTrue();
        }


        await task;
        timerIdList.Count.Should().Be(timerCnt);
        execCnt.Should().Be(timerCnt);
        execute.Should().BeTrue();

        CheckTimerCallBackCost(start, delayMilliSecond);

        foreach (var timerId in timerIdList)
        {
            TimerClassAbstract.GetTimerObject().HasTimer(timerId).Should().BeFalse();
        }

        TestResultHelper.SetFinishTest();
    }

    [ServiceMethod(12)]
    private async ZFTask TimerGenericCallBack_CallBackSucc(int delayMilliSecond, int timerCnt = 1)
    {
        var start = TimeHelper.GetMSSecond();
        var task = ZFTask.CreateZFTask();
        var execute = false;
        int execCnt = 0;
        var dataSum = 0;
        var action = (int x) =>
        {
            dataSum += x;
            execCnt++;
            if (execCnt == timerCnt)
            {
                task.SetResult();
            }
            execute = true;
        };
        var willDataSum = 0;
        var timerIdList = RangeHelper.Range(timerCnt)
            .Select(
            _ =>
            {
                var data = RandomHelper.Random(10000);
                willDataSum += data;
                return TimerClassAbstract.GetTimerObject().TimerCall(delayMilliSecond, action, data);
            }
            )
            .ToList();

        foreach (var timerId in timerIdList)
        {
            TimerClassAbstract.GetTimerObject().HasTimer(timerId).Should().BeTrue();
        }

        await task;
        timerIdList.Count.Should().Be(timerCnt);
        execCnt.Should().Be(timerCnt);
        execute.Should().BeTrue();
        dataSum.Should().Be(willDataSum);

        CheckTimerCallBackCost(start, delayMilliSecond);

        foreach (var timerId in timerIdList)
        {
            TimerClassAbstract.GetTimerObject().HasTimer(timerId).Should().BeFalse();
        }

        TestResultHelper.SetFinishTest();
    }

    [ServiceMethod(13)]
    private async ZFTask TimerCallBack_CallBackOnlyOne()
    {
        var start = TimeHelper.GetMSSecond();
        var task = ZFTask.CreateZFTask();
        var execute = false;
        var execCnt = 0;
        var action = () =>
        {
            execCnt++;
            task.SetResult();
            execute = true;
        };

        var timerId = TimerClassAbstract.GetTimerObject().TimerCall(1, action);
        TimerClassAbstract.GetTimerObject().HasTimer(timerId).Should().BeTrue();

        await TimerClassAbstract.GetTimerObject().Delay(100);
        timerId.Should().NotBe(0);
        execute.Should().BeTrue();
        execCnt.Should().Be(1);

        TimerClassAbstract.GetTimerObject().HasTimer(timerId).Should().BeFalse();

        TestResultHelper.SetFinishTest();
    }

    [ServiceMethod(14)]
    private async ZFTask TimerGenericCallBack_CallBackOnlyOne()
    {
        var start = TimeHelper.GetMSSecond();
        var task = ZFTask.CreateZFTask();
        var execute = false;
        var execCnt = 0;
        var execSumRes = 0;
        var action = (int x) =>
        {
            execSumRes += x;
            execCnt++;
            task.SetResult();
            execute = true;
        };

        var data = RandomHelper.Random(10000);
        var timerId = TimerClassAbstract.GetTimerObject().TimerCall(1, action, data);
        TimerClassAbstract.GetTimerObject().HasTimer(timerId).Should().BeTrue();

        await TimerClassAbstract.GetTimerObject().Delay(100);
        timerId.Should().NotBe(0);
        execute.Should().BeTrue();
        execCnt.Should().Be(1);
        execSumRes.Should().Be(data);

        TimerClassAbstract.GetTimerObject().HasTimer(timerId).Should().BeFalse();

        TestResultHelper.SetFinishTest();
    }

    private async ZFTask IntervalTimerCall_CallBackSucc(int delayMilliSecond, int timerCnt = 1)
    {
        var start = TimeHelper.GetMSSecond();
        var task = ZFTask.CreateZFTask();
        var execute = false;
        int execCnt = 0;
        var action = () =>
        {
            execCnt++;
            if (execCnt == timerCnt)
            {
                task.SetResult();
            }
            execute = true;
        };
        var timerId = TimerClassAbstract.GetTimerObject().IntervalTimerCall(delayMilliSecond, action);

        TimerClassAbstract.GetTimerObject().HasTimer(timerId).Should().BeTrue();


        await task;
        TimerClassAbstract.GetTimerObject().HasTimer(timerId).Should().BeTrue();
        execCnt.Should().Be(timerCnt);
        execute.Should().BeTrue();

        CheckTimerCallBackCost(start, delayMilliSecond);


        TimerClassAbstract.GetTimerObject().CancelTimer(timerId);
        TimerClassAbstract.GetTimerObject().HasTimer(timerId).Should().BeFalse();
    }

    [ServiceMethod(21)]
    private async ZFTask IntervalTimerCall_CallBackNumberTimesAndCancelFinal(int delayMilliSecond, int timerCnt = 1)
    {
        await IntervalTimerCall_CallBackSucc(delayMilliSecond, timerCnt);
        TestResultHelper.SetFinishTest();
    }

    private async ZFTask GenericIntervalTimerCall_CallBackSucc<T>(T data, int delayMilliSecond, int timerCnt = 1)
    {
        var start = TimeHelper.GetMSSecond();
        var task = ZFTask.CreateZFTask();
        var execute = false;
        int execCnt = 0;
        var action = (T x) =>
        {
            execCnt++;
            if (execCnt == timerCnt)
            {
                task.SetResult();
            }
            execute = true;
        };
        var timerId = TimerClassAbstract.GetTimerObject().IntervalTimerCall(delayMilliSecond, action, data);

        TimerClassAbstract.GetTimerObject().HasTimer(timerId).Should().BeTrue();


        await task;
        TimerClassAbstract.GetTimerObject().HasTimer(timerId).Should().BeTrue();
        execCnt.Should().Be(timerCnt);
        execute.Should().BeTrue();

        CheckTimerCallBackCost(start, delayMilliSecond);


        TimerClassAbstract.GetTimerObject().CancelTimer(timerId);
        TimerClassAbstract.GetTimerObject().HasTimer(timerId).Should().BeFalse();
    }

    [ServiceMethod(22)]
    private async ZFTask GenericIntervalTimerCall_CallBackNumberTimesAndCancelFinal(int delayMilliSecond, int timerCnt = 1)
    {
        await GenericIntervalTimerCall_CallBackSucc(RandomHelper.Random(1000), delayMilliSecond, timerCnt);
        await GenericIntervalTimerCall_CallBackSucc(RandomHelper.Random(1000).ToString(), delayMilliSecond, timerCnt);
        await GenericIntervalTimerCall_CallBackSucc(this, delayMilliSecond, timerCnt);
        TestResultHelper.SetFinishTest();
    }

    private async ZFTask Test_CancelTimerCall_TimerStop(int delayMilliSecond, int timerCnt, Func<long, Action, long> createTimerFunc)
    {
        var exec = false;
        var action = () =>
        {
            exec = true;
            throw new InvalidOperationException();
        };

        var timerIdList = RangeHelper.Range(timerCnt)
            .Select(
                _ =>
                {
                    return createTimerFunc(delayMilliSecond, action);
                }
            )
            .ToList();
        timerIdList.Count.Should().Be(timerCnt);
        foreach (var timerId in timerIdList)
        {
            TimerClassAbstract.GetTimerObject().HasTimer(timerId).Should().BeTrue();
        }

        foreach (var timerId in timerIdList)
        {
            TimerClassAbstract.GetTimerObject().CancelTimer(timerId);
        }

        foreach (var timerId in timerIdList)
        {
            TimerClassAbstract.GetTimerObject().HasTimer(timerId).Should().BeFalse();
        }


        await TimerClassAbstract.GetTimerObject().Delay(delayMilliSecond * 2);

        foreach (var timerId in timerIdList)
        {
            TimerClassAbstract.GetTimerObject().HasTimer(timerId).Should().BeFalse();
        }

        exec.Should().BeFalse();
        timerCnt.Should().NotBe(0);

    }


    [ServiceMethod(31)]
    private async ZFTask CancelTimerCall_TimerStop(int delayMilliSecond, int timerCnt)
    {
        await Test_CancelTimerCall_TimerStop(delayMilliSecond, timerCnt, TimerClassAbstract.GetTimerObject().TimerCall);

        TestResultHelper.SetFinishTest();

    }

    private async ZFTask TestCancelGenericTimerCall<T>(int delayMilliSecond, int timerCnt, T data,
        Func<long, Action<T>, T, long> createTimerFunc)
    {

        var exec = false;
        var action = (T x) =>
        {
            exec = true;
            throw new InvalidOperationException();
        };

        var timerIdList = RangeHelper.Range(timerCnt)
            .Select(
                _ =>
                {
                    return createTimerFunc(delayMilliSecond, action, data);
                }
            )
            .ToList();
        timerIdList.Count.Should().Be(timerCnt);
        foreach (var timerId in timerIdList)
        {
            TimerClassAbstract.GetTimerObject().HasTimer(timerId).Should().BeTrue();
        }

        foreach (var timerId in timerIdList)
        {
            TimerClassAbstract.GetTimerObject().CancelTimer(timerId);
        }

        foreach (var timerId in timerIdList)
        {
            TimerClassAbstract.GetTimerObject().HasTimer(timerId).Should().BeFalse();
        }


        await TimerClassAbstract.GetTimerObject().Delay(delayMilliSecond * 2);

        foreach (var timerId in timerIdList)
        {
            TimerClassAbstract.GetTimerObject().HasTimer(timerId).Should().BeFalse();
        }

        exec.Should().BeFalse();
        timerCnt.Should().NotBe(0);
    }

    [ServiceMethod(32)]
    private async ZFTask CancelGenericTimerCall_TimerStop(int delayMilliSecond, int timerCnt)
    {
        var timerObject = TimerClassAbstract.GetTimerObject();
        await TestCancelGenericTimerCall(delayMilliSecond, timerCnt, RandomHelper.Random(),
            timerObject.TimerCall);
        await TestCancelGenericTimerCall(delayMilliSecond, timerCnt, RandomHelper.Random().ToString(),
            timerObject.TimerCall);
        await TestCancelGenericTimerCall(delayMilliSecond, timerCnt, this,
            timerObject.TimerCall);

        TestResultHelper.SetFinishTest();
    }

    [ServiceMethod(33)]
    private async ZFTask CancelIntervalTimerCall_TimerStop(int delayMilliSecond, int timerCnt)
    {
        var timerObject = TimerClassAbstract.GetTimerObject();
        await Test_CancelTimerCall_TimerStop(delayMilliSecond, timerCnt, timerObject.IntervalTimerCall);

        TestResultHelper.SetFinishTest();

    }

    [ServiceMethod(34)]
    private async ZFTask CancelGenericIntervalTimerCall_TimerStop(int delayMilliSecond, int timerCnt)
    {
        var timerObject = TimerClassAbstract.GetTimerObject();
        await TestCancelGenericTimerCall(delayMilliSecond, timerCnt, RandomHelper.Random(),
            timerObject.IntervalTimerCall);
        await TestCancelGenericTimerCall(delayMilliSecond, timerCnt, RandomHelper.Random().ToString(),
            timerObject.IntervalTimerCall);
        await TestCancelGenericTimerCall(delayMilliSecond, timerCnt, this,
            timerObject.IntervalTimerCall);

        TestResultHelper.SetFinishTest();

    }

    private void TestFunc<T>(T data)
    {
        throw new InvalidOperationException();
    }

    [ServiceMethod(41)]
    private async ZFTask CancelAllTimer_AllTimerStop(int delayMilliSecond, int timerCnt)
    {
        var exec = false;
        var action = () =>
        {
            exec = true;
            throw new InvalidOperationException();
        };

        var timerIdList = new List<long>();
        foreach (var _ in Enumerable.Range(0, timerCnt))
        {
            timerIdList.AddRange(
                TimerClassAbstract.GetTimerObject().TimerCall(delayMilliSecond, action),
                TimerClassAbstract.GetTimerObject().IntervalTimerCall(delayMilliSecond, action),
                TimerClassAbstract.GetTimerObject().TimerCall(delayMilliSecond, TestFunc, 90),
                TimerClassAbstract.GetTimerObject().IntervalTimerCall(delayMilliSecond, TestFunc, 90)
                );


        }

        ZFTask delayTask = null!;
        Func<ZFTask> delayFunc = async () =>
        {
            delayTask = TimerClassAbstract.GetTimerObject().Delay(delayMilliSecond);
            await delayTask;
            exec = true;
            throw new InvalidOperationException();
        };

        delayFunc().Coroutine();
        var delayTask2 = TimerClassAbstract.GetTimerObject().Delay(delayMilliSecond);

        timerIdList.Count.Should().Be(timerCnt * 4);

        foreach (var timerId in timerIdList)
        {
            TimerClassAbstract.GetTimerObject().HasTimer(timerId).Should().BeTrue();
        }


        delayTask.Should().NotBeNull();
        delayTask.IsCompleted.Should().BeFalse();
        delayTask2.IsCompleted.Should().BeFalse();


        TimerClassAbstract.GetTimerObject().CancelAllTimer();
        await TimerClassAbstract.GetTimerObject().Delay((int)Math.Min(10 * 1000, (long)delayMilliSecond * 5));

        delayTask2.IsCompleted.Should().BeFalse();
        foreach (var timerId in timerIdList)
        {
            TimerClassAbstract.GetTimerObject().HasTimer(timerId).Should().BeFalse();
        }
        exec.Should().BeFalse();
        TestResultHelper.SetFinishTest();
    }

    [ServiceMethod(51)]
    private void LongTimed_SussTimed(long delayMinute)
    {
        var action = () =>
        {
        };
        var timerObject = TimerClassAbstract.GetTimerObject();
        var timerIdList = new List<long>()
        {
            timerObject.TimerCall(delayMinute*60*1000, action),
            timerObject.IntervalTimerCall(delayMinute*60*1000, action)
        };

        timerIdList.Count.Should().Be(2);
        foreach (var timerId in timerIdList)
        {
            timerObject.HasTimer(timerId).Should().BeTrue();
        }

        foreach (var timerId in timerIdList)
        {
            timerObject.CancelTimer(timerId);
        }
        CheckHaveTimer(timerIdList, false);
        TestResultHelper.SetFinishTest();
    }

    [ServiceMethod(61)]
    private async ZFTask MulServiceCreateTimer_Succ(int serviceCnt, int timerCallCnt, long delayMilliSecond)
    {
        var serviceIdList = RangeHelper.Range(serviceCnt)
            .Select(
                x =>
                {
                    return Service.CreateService<TestService_Timer>();
                }
            )
            .ToList();
        var serviceProxys = serviceIdList
            .Select(x => Service.GetServiceProxy<TestService_TimerProxy>(x))
            .ToList();

        var mode = TimerClassAbstract.Current.GetMode();
        foreach (var serviceId in serviceProxys)
        {
            serviceId.InitTimerClassAbs(mode);
        }
        var taskList = new List<ZFTask<string>>();
        foreach (var serviceId in serviceProxys)
        {
            var res = serviceId.CreateMulTimerAndWaitExec(timerCallCnt, delayMilliSecond);
            taskList.Add(res);
        }

        var resList = await ZFTaskHelper.WhenAll(taskList);
        var errList = resList
            .Where(x => x != "");
        errList.Any().Should().BeFalse();

        Service.CloseService(serviceIdList);

        TestResultHelper.SetFinishTest();
    }

    public void CheckHaveTimer(List<long> timerIdList, bool state, ITimerObject? timerObject = null)
    {
        if (timerObject == null)
        {
            timerObject = TimerClassAbstract.GetTimerObject();
        }
        foreach (var timerId in timerIdList)
        {
            timerObject.HasTimer(timerId).Should().Be(state);
        }
    }

    public void CancelTimerIdList(List<long> timerIdList)
    {
        var timerObject = TimerClassAbstract.GetTimerObject();
        foreach (var timerId in timerIdList)
        {
            timerObject.CancelTimer(timerId);
        }
    }

    [ServiceMethod(62)]
    private async ZFTask<string> CreateMulTimerAndWaitExec(int timerCallCnt, long delayMillSecond)
    {
        try
        {
            var exec = false;
            var action = () =>
            {
                exec = true;
            };
            var timerIdList = new List<long>();
            var timerObject = TimerClassAbstract.GetTimerObject();
            foreach (var _ in RangeHelper.Range(timerCallCnt))
            {
                timerIdList.AddRange(
                    timerObject.TimerCall(delayMillSecond, action),
                    timerObject.IntervalTimerCall(delayMillSecond, action)
                    );
            }
            timerIdList.Count.Should().Be(timerCallCnt * 2);
            CheckHaveTimer(timerIdList, true);

            await timerObject.Delay(delayMillSecond + 100);
            exec.Should().BeTrue();

            CancelTimerIdList(timerIdList);

            CheckHaveTimer(timerIdList, false);

            return "";
        }
        catch (Exception e)
        {
            return e.ToString();
        }
    }

    [ServiceMethod(71)]
    private async ZFTask DestoryTimerEntity_TimerEndLifeTime(int timerCnt)
    {
        var exec = false;
        var action = () =>
        {
            exec = true;
            throw new InvalidOperationException();
        };
        var entity = new TimerBase();
        var timerIdList = RangeHelper.Range(timerCnt)
            .Select(
                x =>
                {
                    return entity.TimerCall(10, action);
                }
            )
            .ToList();
        var timerId = entity.TimerCall(10, action);
        CheckHaveTimer(timerIdList, true, entity);
        entity.Dispose();
        CheckHaveTimer(timerIdList, false, entity);
        await TimerActorSingleton.Current.Delay(100);
        exec.Should().BeFalse();
        entity.HasTimer(timerId).Should().BeFalse();

        TestResultHelper.SetFinishTest();

    }

    [ServiceMethod(72)]
    private async ZFTask RecursionDestoryTimerEntity_TimerEndLifeTime(int recursionCnt, int timerCnt)
    {
        var exec = false;
        var action = () =>
        {
            exec = true;
            throw new InvalidOperationException();
        };
        var allEntitys = new List<(TestTimerEntity, long)>();

        for (int i = 0; i < recursionCnt; i++)
        {
            var timerEntity = new TestTimerEntity();
            var timerId = timerEntity.TimerCall(10, action);
            allEntitys.Add((timerEntity, timerId));

        }
        foreach (var (timerEntity, timerId) in allEntitys)
        {
            timerEntity.HasTimer(timerId).Should().BeTrue();
            timerEntity.Dispose();
        }


        foreach (var (timerEntity2, timerId) in allEntitys)
        {
            timerEntity2.HasTimer(timerId).Should().BeFalse();
        }


        await TimerActorSingleton.Current.Delay(100);
        exec.Should().BeFalse();

        TestResultHelper.SetFinishTest();

    }

}