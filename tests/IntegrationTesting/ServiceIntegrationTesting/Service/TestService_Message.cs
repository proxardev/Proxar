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
using ServiceIntegrationTesting.TestMessageSendAndRecevice;
using System.Diagnostics.CodeAnalysis;


namespace ServiceIntegrationTesting;


public partial class TestService_Message : ServiceBase
{
    private long TargetTestSercviceId = 0;
    private Int32IdGenerator testSendMethodIdGenerator = new Int32IdGenerator();
    private Dictionary<int, ZFTask<int>> testSendMethodResultMap = new Dictionary<int, ZFTask<int>>();

    [AllowNull]
    private ITestService_ReceiveMessage proxy;

    private long GetTargetService()
    {
        return TargetTestSercviceId;
    }

    [ServiceMethod(650)]
    [ServiceMethodAction(true)]
    private void SetTargetProxy(TestService_CustomReceiveMessageProxy proxy)
    {
        this.proxy = proxy;
    }

    [ServiceMethod(651)]
    [ServiceMethodAction(true)]
    private void SetTargetProxy2(TestService_CustomReceiveMessageExternalProxy proxy)
    {
        this.proxy = proxy;
    }

    internal ITestService_ReceiveMessage GetTargetServiceProxy()
    {
        if (this.proxy != null)
        {
            return this.proxy;
        }
        return Service.GetServiceProxy<TestService_ReceiveMessageProxy>(this.GetTargetService());
    }

    [ServiceMethod(1)]
    private async ZFTask SimpleArgs_SendReceiveSucc()
    {
        var proxy = this.GetTargetServiceProxy();
        var res = await proxy.SimpleArgsReceive(
            (byte)1, (short)2, (ushort)3, 4, 5U, 6L, 7UL, 8f, 9d);

        var call3_NeedSerialize = Service.IsNeedSerialize<string>()
                                || Service.IsNeedSerialize<decimal>();

        res.Should().NotBeNull();

        var (data1, data2, data3, data4, data5, data6, data7, data8, data9) = res;
        data1.Should().Be(1);
        data2.Should().Be(2);
        data3.Should().Be(3);
        data4.Should().Be(4);
        data5.Should().Be(5);
        data6.Should().Be(6);
        data7.Should().Be(7);
        data8.Should().Be(8);
        data9.Should().Be(9);

        TestResultHelper.SetFinishTest();
    }

    [ServiceMethod(2)]
    private async ZFTask SimpleArgs2_SendReceiveSucc()
    {
        var proxy = this.GetTargetServiceProxy();
        var res = await proxy.SimpleArgsReceive2("1", 2m);

        res.Item1.Should().Be("1");
        res.Item2.Should().Be(2);
        TestResultHelper.SetFinishTest();
    }

    [ServiceMethod(897)]
    private async ZFTask Call_ExceptionOnRemote_CatchException()
    {
        try
        {
            await this.GetTargetServiceProxy().ThrowException();
        }
        catch (Exception e)
        {
            e.Message.Should().NotBeNull("Dispatch_897_ThrowException");
            TestResultHelper.SetFinishTest();
            return;
        }
        throw new Exception("测试失败");
    }

    private (int, ZFTask<int>) GetSendTestResultTask()
    {
        var idx = this.testSendMethodIdGenerator.NewId();
        var task = ZFTask<int>.CreateZFTask();
        testSendMethodResultMap[idx] = task;
        return (idx, task);
    }

    [ServiceMethod(899)]
    private void ReceiceMsgIdxResult(int msgIdx, int result)
    {
        var task = this.testSendMethodResultMap[msgIdx];
        this.testSendMethodResultMap.Remove(msgIdx);
        task.SetResult(result);
    }

    [ServiceMethod(601)]
    private void SetTargetTestService(long serviceId)
    {
        TargetTestSercviceId = serviceId;
    }
}