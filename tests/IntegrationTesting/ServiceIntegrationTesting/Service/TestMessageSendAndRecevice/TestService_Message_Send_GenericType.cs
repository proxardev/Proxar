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
using ServiceIntegrationTesting.Models;

namespace ServiceIntegrationTesting;

public partial class TestService_Message
{
    [ServiceMethod(100)]
    [ServiceMethodExport]
    private async ZFTask GenericStructInt_SendReceiveSucc()
    {
        var input = new GenericStruct<int>(42, 3);
        var result = await this.GetTargetServiceProxy().GenericStructIntArgsReceive(input);
        result.Should().Be(input.Value * input.Count);
        TestResultHelper.SetFinishTest();
    }

    [ServiceMethod(101)]
    [ServiceMethodExport]
    private async ZFTask GenericStructString_SendReceiveSucc()
    {
        var input = new GenericStruct<string>("hello", 2);
        var result = await this.GetTargetServiceProxy().GenericStructStringArgsReceive(input);
        result.Should().Be(input.Value.Length * input.Count);
        TestResultHelper.SetFinishTest();
    }

    [ServiceMethod(102)]
    [ServiceMethodExport]
    private async ZFTask GenericStructPlayerScore_SendReceiveSucc()
    {
        var input = new GenericStruct<PlayerScore>(new PlayerScore { PlayerId = 1, Score = 100 }, 2);
        var result = await this.GetTargetServiceProxy().GenericStructPlayerScoreArgsReceive(input);
        result.Should().Be(input.Value.TotalScore() * input.Count);
        TestResultHelper.SetFinishTest();
    }

    [ServiceMethod(103)]
    [ServiceMethodExport]
    private async ZFTask GenericStructNestedGenericStruct_SendReceiveSucc()
    {
        var inner = new GenericStruct<int>(5, 3);
        var input = new GenericStruct<GenericStruct<int>>(inner, 2);
        var result = await this.GetTargetServiceProxy().GenericStructNestedGenericStructArgsReceive(input);
        result.Should().Be((input.Value.Value * input.Value.Count) * input.Count);
        TestResultHelper.SetFinishTest();
    }

    [ServiceMethod(104)]
    [ServiceMethodExport]
    private async ZFTask GenericStructStructWithClass_SendReceiveSucc()
    {
        var input = new GenericStruct<StructWithClass>(
            new StructWithClass(10, new SimpleClass { Id = 5 }), 3);
        var result = await this.GetTargetServiceProxy().GenericStructStructWithClassArgsReceive(input);
        result.Should().Be((input.Value.Value + (input.Value.Ref?.Id ?? 0)) * input.Count);
        TestResultHelper.SetFinishTest();
    }

    [ServiceMethod(105)]
    [ServiceMethodExport]
    private async ZFTask GenericClassInt_SendReceiveSucc()
    {
        var input = new GenericClass<int> { Data = 42, Id = 5 };
        var result = await this.GetTargetServiceProxy().GenericClassIntArgsReceive(input);
        result.Should().Be(input.Compute() + input.Data);
        TestResultHelper.SetFinishTest();
    }

    [ServiceMethod(106)]
    [ServiceMethodExport]
    private async ZFTask GenericClassListInt_SendReceiveSucc()
    {
        var input = new GenericClass<List<int>>
        {
            Data = new List<int> { 1, 2, 3 },
            Id = 2
        };
        var result = await this.GetTargetServiceProxy().GenericClassListIntArgsReceive(input);
        result.Should().Be(input.Compute() + input.Data.Sum());
        TestResultHelper.SetFinishTest();
    }

    [ServiceMethod(107)]
    [ServiceMethodExport]
    private async ZFTask GenericClassPoint3D_SendReceiveSucc()
    {
        var input = new GenericClass<Point3D>
        {
            Data = new Point3D(1, 2, 3),
            Id = 3
        };
        var result = await this.GetTargetServiceProxy().GenericClassPoint3DArgsReceive(input);
        result.Should().Be(input.Compute() + input.Data.Sum());
        TestResultHelper.SetFinishTest();
    }

    [ServiceMethod(108)]
    [ServiceMethodExport]
    private async ZFTask GenericClassStructWithClass_SendReceiveSucc()
    {
        var input = new GenericClass<StructWithClass>
        {
            Data = new StructWithClass(10, new SimpleClass { Id = 5 }),
            Id = 4
        };
        var result = await this.GetTargetServiceProxy().GenericClassStructWithClassArgsReceive(input);
        result.Should().Be(input.Compute() + input.Data.Value + (input.Data.Ref?.Id ?? 0));
        TestResultHelper.SetFinishTest();
    }

    [ServiceMethod(109)]
    [ServiceMethodExport]
    private async ZFTask GenericClassGenericClassInt_SendReceiveSucc()
    {
        var input = new GenericClass<GenericClass<int>>
        {
            Data = new GenericClass<int> { Data = 7, Id = 1 },
            Id = 5
        };
        var result = await this.GetTargetServiceProxy().GenericClassGenericClassIntArgsReceive(input);
        result.Should().Be(input.Compute() + input.Data.Compute() + input.Data.Data);
        TestResultHelper.SetFinishTest();
    }
}