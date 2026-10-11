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


using System.Diagnostics;
namespace Proxar.ServiceCore.Dispatch;

internal class StatRecord
{
    internal long StatPark;
    [Conditional("DEBUG")]
    internal void AddStatPark() => StatPark++;

    internal long StatSpurious;
    [Conditional("DEBUG")]
    internal void AddStatSpurious() => StatSpurious++;

    internal long StatLoop;
    [Conditional("DEBUG")]
    internal void AddStatLoop() => StatLoop++;

    internal long StatPopHit;
    [Conditional("DEBUG")]
    internal void AddStatPopHit() => StatPopHit++;

    internal long StatPopMiss;
    [Conditional("DEBUG")]
    internal void AddStatPopMiss() => StatPopMiss++;

    internal long StatTakeService;
    [Conditional("DEBUG")]
    internal void AddStatTakeService() => StatTakeService++;

    internal long StatMsg;
    [Conditional("DEBUG")]
    internal void AddStatMsg() => StatMsg++;

    internal long StatDrop;
    [Conditional("DEBUG")]
    internal void AddStatDrop() => StatDrop++;

    internal long StatEmpty;
    [Conditional("DEBUG")]
    internal void AddStatEmpty() => StatEmpty++;

    internal long StatSpinHit;   // [FIX-A] 自旋期间直接接到活（零系统调用）
    [Conditional("DEBUG")]
    internal void AddStatSpinHit() => StatSpinHit++;

    internal long StatSpinMiss;  // [FIX-A] 自旋没接到活
    internal void AddStatSpinMiss() => StatSpinMiss++;


    [Conditional("DEBUG")]
    internal void Record()
    {

    }
}
