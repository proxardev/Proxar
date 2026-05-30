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
using TestShared;

namespace ServiceIntegrationTesting;

public partial class TestService_Create : ServiceBase
{
    private Dictionary<int, bool> waitSendCallBackData = new Dictionary<int, bool>();

    [ServiceMethod(1)]
    private async ZFTask CreateAndDestoryService_Succ(int count)
    {
        var serviceIdList = RangeHelper.Range(count)
            .Select(
                x =>
                {
                    return Service.CreateService<TestService>();
                }
            )
            .ToList();
        var serviceProxys = serviceIdList
            .Select(tarServiceId => Service.GetServiceProxy<TestServiceProxy>(tarServiceId))
            .ToList();
        var callTaskList = serviceProxys
            .Select(
                proxy =>
                {
                    return proxy.AdditinAndResponse(1, 54);
                }
            )
            .ToList();

        TestLogHelper.TestDebugLog($"CreateAndDestory wait {count} {callTaskList.Count}");
        var callResList = await Proxar.Tasks.ZFTaskHelper.WhenAll(callTaskList);
        TestLogHelper.TestDebugLog($"CreateAndDestory wait succ {count}");
        callResList.Count.Should().Be(count);
        callResList.ToHashSet().Count.Should().Be(1);


        _ = serviceProxys
            .Select(
                x =>
                {
                    x.ExecuteServiceClose();
                    return 0;
                }
            )
            .ToList();

        TestLogHelper.TestDebugLog($"CreateAndDestory start delay {count}");
        await TimerHelper.Delay(1000);
        TestLogHelper.TestDebugLog($"CreateAndDestory wait delay succ {count}");

        for (; ; )
        {
            var wait = false;
            var set = serviceIdList.ToHashSet();
            foreach (var serviceId in serviceIdList)
            {
                var service = ServiceManager.Instance.GetService(serviceId);
                if (service != null)
                {
                    var x = ServiceManager.Instance;
                    wait = true;
                    break;
                }
            }
            if (wait)
            {
                await TimerHelper.Delay(1000);
            }
            else
            {
                break;
            }
        }

        var succServiceSet = serviceIdList.ToHashSet();
        foreach (var proxy in serviceProxys)
        {
            Exception? e = null;
            try
            {
                await proxy.AdditinAndResponse(1, 54);
            }
            catch (Exception ex)
            {
                e = ex;
                succServiceSet.Remove(proxy.GetServiceId());
            }
            e.Should().NotBeNull();
        }

        succServiceSet.Count.Should().Be(0);
        TestResultHelper.SetFinishTest();

    }

    [ServiceMethod(2)]
    private async ZFTask ParallelCreateService_Succ(int parallel, int cnt)
    {
        parallel.Should().BeGreaterThanOrEqualTo(10);
        cnt.Should().BeGreaterThanOrEqualTo(100);

        var serviceIdList = RangeHelper.Range(parallel)
            .Select(
                x =>
                {
                    return Service.CreateService<TestService_Create>();
                }
            )
            .ToList();
        var callTaskList = serviceIdList
            .Select(
                x =>
                {
                    return Service.GetServiceProxy<TestService_CreateProxy>(x).CreateService(cnt);
                }
            )
            .ToList();

        var res = await Proxar.Tasks.ZFTaskHelper.WhenAll(callTaskList);
        var allServiceIdList = res.SelectMany(x => x).ToList();
        allServiceIdList.Should().NotBeNull();
        allServiceIdList.Count.Should().Be(parallel * cnt);
        allServiceIdList.ToHashSet().Count.Should().Be(parallel * cnt);

        TestResultHelper.SetFinishTest();

    }

    [ServiceMethod(3)]
    private List<long> CreateService(int cnt)
    {
        var serviceIdList = new List<long>();
        try
        {
            foreach (var _ in RangeHelper.Range(cnt))
            {
                var serviceId = Service.CreateService<TestService>();
                serviceIdList.Add(serviceId);

            }
        }
        catch (Exception)
        {

        }
        return serviceIdList;
    }

    [ServiceMethod(11)]
    private async ZFTask MulCallAndSend_CallSendSucc(int count)
    {
        var serviceIdList = RangeHelper.Range(count)
            .Select(
                x =>
                {
                    return Service.CreateService<HelpTest_Service>();
                }
            )
            .ToList();

        var idGenerator = new Int32IdGenerator();

        var testService_CreateProxy = Service.GetServiceProxy<TestService_CreateProxy>(GetServiceId());
        var idList = serviceIdList
            .Select(
                x =>
                {
                    var id = idGenerator.NewId();
                    var proxy = Service.GetServiceProxy<HelpTest_ServiceProxy>(x);
                    proxy.Raw.Ping(testService_CreateProxy.Pong, id);
                    return id;
                }
            )
            .ToList();

        var callTaskList = serviceIdList
            .Select(
                x =>
                {
                    var proxy = Service.GetServiceProxy<HelpTest_ServiceProxy>(x);
                    return proxy.Add(1, 54);
                }
            )
            .ToList();
        var callResList = await Proxar.Tasks.ZFTaskHelper.WhenAll(callTaskList);
        callResList.Count.Should().Be(count);
        callResList.ToHashSet().Count.Should().Be(1);
        callResList.First().Should().Be(55);

        var callbackSuccIdList = waitSendCallBackData.Keys.ToList();
        callbackSuccIdList.Sort();
        idList.Sort();
        callbackSuccIdList.Should().BeEqualTo(idList);
        waitSendCallBackData.Keys.ToHashSet().SetEquals(idList.ToHashSet()).Should().BeTrue();
        waitSendCallBackData.Clear();

        TestResultHelper.SetFinishTest();

    }

    [ServiceMethod(13)]
    private void Pong(int seq)
    {
        waitSendCallBackData[seq] = true;
    }


}