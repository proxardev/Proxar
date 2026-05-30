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
    // 30: Point2D - 纯值简单结构体
    [ServiceMethod(30)]
    [ServiceMethodExport]
    private async ZFTask Point2D_SendReceiveSucc()
    {
        var input = new Point2D(3, 4);
        var result = await GetTargetServiceProxy().Point2DArgsReceive(input);
        result.Should().Be(input.Sum() * 2);
        TestResultHelper.SetFinishTest();
    }

    // 31: Point3D - 纯值复杂结构体（含方法）
    [ServiceMethod(31)]
    [ServiceMethodExport]
    private async ZFTask Point3D_SendReceiveSucc()
    {
        var input = new Point3D(1, 2, 3);
        var result = await GetTargetServiceProxy().Point3DArgsReceive(input);
        result.Item1.Should().Be(input.Sum() + 10);
        result.Item2.X.Should().Be(input.X * 2);
        result.Item2.Y.Should().Be(input.Y * 2);
        result.Item2.Z.Should().Be(input.Z * 2);
        TestResultHelper.SetFinishTest();
    }

    // 32: LabeledValue - 含string字段（必须序列化）
    [ServiceMethod(32)]
    private async ZFTask LabeledValue_SendReceiveSucc()
    {
        var input = new LabeledValue(100, "Score");
        var result = await GetTargetServiceProxy().LabeledValueArgsReceive(input);
        result.Should().Be($"{input.Label}:{input.Value + 50}");
        TestResultHelper.SetFinishTest();
    }

    // 33: StructWithClass - 含类字段（必须序列化）
    [ServiceMethod(33)]
    [ServiceMethodExport]
    private async ZFTask StructWithClass_SendReceiveSucc()
    {
        var input = new StructWithClass(10, new SimpleClass { Id = 5 });
        var result = await GetTargetServiceProxy().StructWithClassArgsReceive(input);
        result.Should().Be(input.Value + (input.Ref?.Compute() ?? 0));
        TestResultHelper.SetFinishTest();
    }
}