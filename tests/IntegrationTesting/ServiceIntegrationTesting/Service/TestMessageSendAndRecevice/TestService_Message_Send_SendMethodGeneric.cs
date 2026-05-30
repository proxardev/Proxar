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
using Proxar.ServiceCore;
using Proxar.Tasks;

namespace ServiceIntegrationTesting;

public partial class TestService_Message
{
    [ServiceMethod(170)]
    [ServiceMethodExport]
    private async ZFTask Send0DataArgs_SendMethodAndReceiveSucc()
    {
        var (idx, task) = GetSendTestResultTask();
        this.GetTargetServiceProxy().Send0DataArgs(this.GetServiceId(), idx);
        var result = await task;
        result.Should().Be(100);
        TestResultHelper.SetFinishTest();
    }

    [ServiceMethod(171)]
    [ServiceMethodExport]
    private async ZFTask Send1DataArg_SendMethodAndReceiveSucc()
    {
        var (idx, task) = GetSendTestResultTask();
        this.GetTargetServiceProxy().Send1DataArg(this.GetServiceId(), idx, 7);
        var result = await task;
        result.Should().Be(21);
        TestResultHelper.SetFinishTest();
    }

    [ServiceMethod(172)]
    [ServiceMethodExport]
    private async ZFTask Send2DataArgs_SendMethodAndReceiveSucc()
    {
        var (idx, task) = GetSendTestResultTask();
        this.GetTargetServiceProxy().Send2DataArgs(this.GetServiceId(), idx, 5, 10);
        var result = await task;
        result.Should().Be(45);
        TestResultHelper.SetFinishTest();
    }

    [ServiceMethod(173)]
    [ServiceMethodExport]
    private async ZFTask Send3DataArgs_SendMethodAndReceiveSucc()
    {
        var (idx, task) = GetSendTestResultTask();
        this.GetTargetServiceProxy().Send3DataArgs(this.GetServiceId(), idx, 2, 3, 4);
        var result = await task;
        result.Should().Be(24);
        TestResultHelper.SetFinishTest();
    }

    [ServiceMethod(174)]
    [ServiceMethodExport]
    private async ZFTask Send4DataArgs_SendMethodAndReceiveSucc()
    {
        var (idx, task) = GetSendTestResultTask();
        this.GetTargetServiceProxy().Send4DataArgs(this.GetServiceId(), idx, 5, 10, 15, 20);
        var result = await task;
        result.Should().Be(100);
        TestResultHelper.SetFinishTest();
    }

    [ServiceMethod(175)]
    [ServiceMethodExport]
    private async ZFTask Send5DataArgs_SendMethodAndReceiveSucc()
    {
        var (idx, task) = GetSendTestResultTask();
        this.GetTargetServiceProxy().Send5DataArgs(this.GetServiceId(), idx, 1, 2, 3, 4, 5);
        var result = await task;
        result.Should().Be(45);
        TestResultHelper.SetFinishTest();
    }

    [ServiceMethod(176)]
    [ServiceMethodExport]
    private async ZFTask Send6DataArgs_SendMethodAndReceiveSucc()
    {
        var (idx, task) = GetSendTestResultTask();
        this.GetTargetServiceProxy().Send6DataArgs(this.GetServiceId(), idx, 5, 10, 15, 20, 25, 30);
        var result = await task;
        result.Should().Be(305);
        TestResultHelper.SetFinishTest();
    }

    [ServiceMethod(177)]
    [ServiceMethodExport]
    private async ZFTask Send7DataArgs_SendMethodAndReceiveSucc()
    {
        var (idx, task) = GetSendTestResultTask();
        this.GetTargetServiceProxy().Send7DataArgs(this.GetServiceId(), idx, 10, 20, 30, 40, 50, 60, 70);
        var result = await task;
        result.Should().Be(280);
        TestResultHelper.SetFinishTest();
    }
}