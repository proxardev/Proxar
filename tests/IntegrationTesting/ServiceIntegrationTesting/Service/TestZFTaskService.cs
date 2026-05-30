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
using Proxar.Timer;
using Proxar.Utilities;
using TestShared;
namespace ServiceIntegrationTesting;

internal partial class TestService_ZFTask : ServiceBase
{

    // 同步完成场景，即await的时候，已经完成
    [ServiceMethod(1)]
    private async ZFTask ZFTask_ValueTaskSyncReturn_CompletesImmediately()
    {
        async ZFTask<int> GetNumber()
        {
            await ZFTask.CompletedTask;
            return 42;
        }

        var task = GetNumber();
        task.IsCompleted.Should().BeTrue();
        var res = await task;
        res.Should().Be(42);
        TestResultHelper.SetFinishTest();

    }

    [ServiceMethod(2)]
    public void ZFTask_VoidTaskSyncReturn_CompletesImmediately()
    {
#pragma warning disable CS1998 // 异步方法缺少 "await" 运算符，将以同步方式运行
        async ZFTask DoWork() { }
#pragma warning restore CS1998 // 异步方法缺少 "await" 运算符，将以同步方式运行

        var task = DoWork();
        task.IsCompleted.Should().BeTrue();
        TestResultHelper.SetFinishTest();
    }

    [ServiceMethod(3)]
    private async ZFTask ZFTask_NestedCallSyncReturn_OuterReceivesInnerTask()
    {
#pragma warning disable CS1998 // 异步方法缺少 "await" 运算符，将以同步方式运行
        async ZFTask<int> Inner() => 42;
#pragma warning restore CS1998 // 异步方法缺少 "await" 运算符，将以同步方式运行
        ZFTask<int> Outer() => Inner();

        var task = Outer();
        (await task).Should().Be(42);
        TestResultHelper.SetFinishTest();
    }


    // 异步完成的场景
    // 不直接await和直接await

    [ServiceMethod(11)]
    private async ZFTask ZFTask_ValueTypeAsyncReturn_WaitMomentComplete()
    {
        async ZFTask<int> GetNumber()
        {
            await Service.NextFrame();
            return 42;
        }

        var task = GetNumber();
        var task2 = GetNumber();



        task.IsCompleted.Should().BeFalse();
        task2.IsCompleted.Should().BeFalse();

        await task2;

        await TimerHelper.Delay(1);
        task.IsCompleted.Should().BeTrue();

        (await task).Should().Be(42);
        TestResultHelper.SetFinishTest();
    }

    [ServiceMethod(12)]
    public async ZFTask ZFTask_VoidTaskASyncReturn_WaitMomentComplete()
    {
        async ZFTask DoWork()
        {
            await Service.NextFrame();
        }

        var task = DoWork();
        var task2 = DoWork();



        task.IsCompleted.Should().BeFalse();
        task2.IsCompleted.Should().BeFalse();

        await task2;

        await TimerHelper.Delay(1);
        task.IsCompleted.Should().BeTrue();

        TestResultHelper.SetFinishTest();
    }

    [ServiceMethod(13)]
    private async ZFTask ZFTask_NestedCallASyncReturn_WaitMomentComplete()
    {
        async ZFTask<int> Inner()
        {
            await Service.NextFrame();
            return 42;
        }
        ZFTask<int> Outer() => Inner();

        var task = Outer();
        var task2 = Outer();



        task.IsCompleted.Should().BeFalse();
        task2.IsCompleted.Should().BeFalse();

        await task2;

        await TimerHelper.Delay(2);
        task.IsCompleted.Should().BeTrue();

        (await task).Should().Be(42);

        TestResultHelper.SetFinishTest();
    }

    [ServiceMethod(21)]
    private async ZFTask ZFTask_MultipleAwait_Sequential_ExecutesInOrder()
    {

        async ZFTask<List<long>> Sequential()
        {
            var order = new List<long>();
            order.Add(1);
            await Service.NextFrame();
            order.Add(2);
            await Service.NextFrame();
            order.Add(3);
            await TimerHelper.Delay(1);
            order.Add(4);
            await Service.NextFrame();
            order.Add(5);
            return order;
        }

        var order = await Sequential();
        order.Should().Equal(1, 2, 3, 4, 5);
        TestResultHelper.SetFinishTest();
    }

    [ServiceMethod(22)]
    private async ZFTask ZFTask_NestedAwait_DoubleLayer_BothComplete()
    {
        var exec = false;
        var resExec = 0;
        async ZFTask Inner()
        {
            await ZFTask.Yield();
        }
        async ZFTask<int> Inner2()
        {
            await ZFTask.Yield();
            return 42;
        }

        async ZFTask Outer()
        {
            await Inner();
            resExec = await Inner2();
            exec = true;
        }
        var task = Outer();
        exec.Should().BeFalse();
        task.IsCompleted.Should().BeFalse();
        await task;
        exec.Should().BeTrue();
        resExec.Should().Be(42);
        TestResultHelper.SetFinishTest();
    }

    [ServiceMethod(31)]
    private async ZFTask ZFTask_ConditionalAwait_AwaitsAndSyncComplete()
    {
        var executed = false;

        async ZFTask Conditional(bool flag)
        {
            if (flag)
            {
                await ZFTask.Yield();
                executed = true;
            }
        }

        var awaitTask = Conditional(true);
        var syncCompleteTask = Conditional(false);


        awaitTask.IsCompleted.Should().BeFalse();
        syncCompleteTask.IsCompleted.Should().BeTrue();
        executed.Should().BeFalse();

        await awaitTask;

        executed.Should().BeTrue();
        TestResultHelper.SetFinishTest();
    }

    private async ZFTask TaskLoop<T>(int loop, Action? loopAction = null,
        Func<bool>? validStopFunc = null,
        Func<ZFTask>? loopAwaitFunc = null, Func<ZFTask<T>>? loopAwaitFunc2 = null,
        Action? beforeLoopAction = null)
    {
        if (loopAwaitFunc == null)
        {
            loopAwaitFunc = Service.NextFrame;
        }
        if (beforeLoopAction != null)
        {
            beforeLoopAction();
        }
        for (int i = 0; i < loop; i++)
        {
            await loopAwaitFunc();
            if (loopAwaitFunc2 != null)
            {
                await loopAwaitFunc2();
            }
            loopAction?.Invoke();

            if (validStopFunc != null && validStopFunc())
            {
                break;
            }
        }
    }

    private async ZFTask<T?> GenericTaskLoop<T>(int loop, Action? loopAction = null,
        Func<bool>? validStopFunc = null,
        Func<ZFTask>? loopAwaitFunc = null, Func<ZFTask<T>>? loopAwaitFunc2 = null,
        Action? beforeLoopAction = null
        )
    {
        if (loopAwaitFunc == null)
        {
            loopAwaitFunc = Service.NextFrame;
        }
        if (beforeLoopAction != null)
        {
            beforeLoopAction();
        }
        T? ret = default;
        for (int i = 0; i < loop; i++)
        {
            await loopAwaitFunc();
            if (loopAwaitFunc2 != null)
            {
                ret = await loopAwaitFunc2();
            }
            loopAction?.Invoke();

            if (validStopFunc != null && validStopFunc())
            {
                break;
            }
        }
        return ret;
    }

    [ServiceMethod(41)]
    private async ZFTask ZFTask_LoopAwait_ForLoop_ExecutesAllIterations()
    {
        var count = 0;
        void Loop()
        {
            count++;
        }
        await TaskLoop<int>(5, Loop);

        count.Should().Be(5);
        TestResultHelper.SetFinishTest();
    }

    [ServiceMethod(42)]
    private async ZFTask GeneralZFTask_LoopAwait_ForLoop_ExecutesAllIterations()
    {
        var count = 0;

        void Loop()
        {
            count++;
        }
        var data1 = RandomHelper.Random(10000);
        var data2 = RandomHelper.Random(10000);
        var helperServiceId = Service.GetUniqueService<HelpTest_Service>();
        var helpServiceProxy = Service.GetServiceProxy<HelpTest_ServiceProxy>(helperServiceId);
        async ZFTask<int> Loop2()
        {
            var result = await helpServiceProxy.Add(data1, data2);
            return result;
        }

        var res = await GenericTaskLoop<int>(5, Loop, null, null, Loop2);
        count.Should().Be(5);
        res.Should().Be(data1 + data2);
        TestResultHelper.SetFinishTest();
    }

    [ServiceMethod(43)]
    private async ZFTask ZFTask_LoopAwait_WhileLoop_BreakEarly()
    {
        var count = 0;
        void Loop()
        {
            count++;
        }

        bool ValidStop()
        {
            return count >= 3;
        }
        await TaskLoop<int>(5, Loop, ValidStop);

        count.Should().Be(3);
        TestResultHelper.SetFinishTest();
    }

    [ServiceMethod(45)]
    private async ZFTask GeneralZFTask_LoopAwait_WhileLoop_BreakEarly()
    {
        var count = 0;

        void Loop()
        {
            count++;
        }
        bool ValidStop()
        {
            return count >= 3;
        }

        var data1 = RandomHelper.Random(10000);
        var data2 = RandomHelper.Random(10000);
        var helperServiceId = Service.GetUniqueService<HelpTest_Service>();
        var helpTest_ServiceProxy = Service.GetServiceProxy<HelpTest_ServiceProxy>(helperServiceId);
        async ZFTask<int> Loop2()
        {
            return await helpTest_ServiceProxy.Add(data1, data2);
        }
        async ZFTask<string> Loop3()
        {
            var res = await Loop2();
            return res.ToString();
        }

        var res = await GenericTaskLoop<int>(5, Loop, ValidStop, null, Loop2);
        var resString = await GenericTaskLoop(5, loopAwaitFunc2: Loop3);
        count.Should().Be(3);
        res.Should().Be(data1 + data2);
        resString.Should().Be(res.ToString());
        TestResultHelper.SetFinishTest();
    }


    [ServiceMethod(51)]
    private async ZFTask ZFTask_Exception_SyncThrow_CapturedInTask()
    {
        void LoopExceptionAction()
        {
            throw new InvalidOperationException("LoopAction");
        }
        Exception? ex = null;
        try
        {
            await TaskLoop<int>(5, null!, beforeLoopAction: LoopExceptionAction);
        }
        catch (Exception e)
        {
            ex = e;
        }

        ex.Should().NotBeNull();
        ex.Should().BeOfType<InvalidOperationException>();
        ex.Message.Should().Be("LoopAction");

        TestResultHelper.SetFinishTest();
    }

    [ServiceMethod(52)]
    private async ZFTask ZFTask_Exception_AsyncThrow_CapturedInTask()
    {
        void LoopExceptionAction()
        {
            throw new InvalidOperationException("LoopAction");
        }
        Exception? ex = null;
        try
        {
            await TaskLoop<int>(5, LoopExceptionAction);
        }
        catch (Exception e)
        {
            ex = e;
        }

        ex.Should().NotBeNull();
        ex.Should().BeOfType<InvalidOperationException>();
        ex.Message.Should().Be("LoopAction");

        TestResultHelper.SetFinishTest();
    }

    [ServiceMethod(53)]
    private async ZFTask GenericZFTask_Exception_SyncThrow_CapturedInTask()
    {
        void LoopExceptionAction()
        {
            throw new InvalidOperationException("LoopAction");
        }
        Exception? ex = null;
        try
        {
            await GenericTaskLoop<int>(5, null!, beforeLoopAction: LoopExceptionAction);
        }
        catch (Exception e)
        {
            ex = e;
        }

        ex.Should().NotBeNull();
        ex.Should().BeOfType<InvalidOperationException>();
        ex.Message.Should().Be("LoopAction");

        TestResultHelper.SetFinishTest();
    }

    [ServiceMethod(54)]
    private async ZFTask GenericZFTask_Exception_AsyncThrow_CapturedInTask()
    {
        void LoopExceptionAction()
        {
            throw new InvalidOperationException("LoopAction");
        }
        Exception? ex = null;
        try
        {
            await GenericTaskLoop<int>(5, LoopExceptionAction);
        }
        catch (Exception e)
        {
            ex = e;
        }

        ex.Should().NotBeNull();
        ex.Should().BeOfType<InvalidOperationException>();
        ex.Message.Should().Be("LoopAction");

        TestResultHelper.SetFinishTest();
    }

    [ServiceMethod(55)]
    private async ZFTask GenericZFTask_LoopException_AsyncThrow_CapturedInTask()
    {
        int loop = 0;
        async ZFTask LoopExceptionAction()
        {
            await Service.NextFrame();
            loop++;
            if (loop == 3)
            {
                throw new InvalidOperationException("LoopAction");
            }
        }
        Exception? ex = null;
        try
        {
            await GenericTaskLoop<int>(5, loopAwaitFunc: LoopExceptionAction);
        }
        catch (Exception e)
        {
            ex = e;
        }
        loop.Should().Be(3);
        ex.Should().NotBeNull();
        ex.Should().BeOfType<InvalidOperationException>();
        ex.Message.Should().Be("LoopAction");

        TestResultHelper.SetFinishTest();
    }

    [ServiceMethod(56)]
    private async ZFTask GenericZFTask_GenericZFTaskLoopException_AsyncThrow_CapturedInTask()
    {
        int loop = 0;
        async ZFTask<int> LoopExceptionAction()
        {
            await Service.NextFrame();
            loop++;
            if (loop == 3)
            {
                throw new InvalidOperationException("LoopAction");
            }
            return loop;
        }
        Exception? ex = null;
        try
        {
            await GenericTaskLoop(5, loopAwaitFunc2: LoopExceptionAction);
        }
        catch (Exception e)
        {
            ex = e;
        }
        loop.Should().Be(3);
        ex.Should().NotBeNull();
        ex.Should().BeOfType<InvalidOperationException>();
        ex.Message.Should().Be("LoopAction");

        TestResultHelper.SetFinishTest();
    }

    [ServiceMethod(57)]
    private async ZFTask ZFTask_NestedTaskLoopException_AsyncThrow_CapturedInTask()
    {
        int loop = 0;
        async ZFTask LoopExceptionAction()
        {
            await Service.NextFrame();
            loop++;
            if (loop == 3)
            {
                throw new InvalidOperationException("LoopAction");
            }
        }
        async ZFTask LoopExceptionAction2()
        {
            await LoopExceptionAction();
        }
        async ZFTask LoopExceptionAction3()
        {
            await LoopExceptionAction2();
        }
        async ZFTask LoopExceptionAction4()
        {
            await LoopExceptionAction3();
        }
        Exception? ex = null;
        try
        {
            await GenericTaskLoop<int>(5, loopAwaitFunc: LoopExceptionAction4);
        }
        catch (Exception e)
        {
            ex = e;
        }
        loop.Should().Be(3);
        ex.Should().NotBeNull();
        ex.Should().BeOfType<InvalidOperationException>();
        ex.Message.Should().Be("LoopAction");

        TestResultHelper.SetFinishTest();
    }

    [ServiceMethod(58)]
    private async ZFTask GenericZFTask_NestedGenericZFTaskLoopException_AsyncThrow_CapturedInTask()
    {
        int loop = 0;
        async ZFTask<int> LoopExceptionAction()
        {
            await Service.NextFrame();
            loop++;
            if (loop == 3)
            {
                throw new InvalidOperationException("LoopAction");
            }
            return loop;
        }
        async ZFTask<int> LoopExceptionAction2()
        {
            return await LoopExceptionAction();
        }
        async ZFTask<int> LoopExceptionAction3()
        {
            return await LoopExceptionAction2();
        }
        async ZFTask<int> LoopExceptionAction4()
        {
            return await LoopExceptionAction3();
        }
        Exception? ex = null;
        try
        {
            await GenericTaskLoop(5, loopAwaitFunc2: LoopExceptionAction4);
        }
        catch (Exception e)
        {
            ex = e;
        }
        loop.Should().Be(3);
        ex.Should().NotBeNull();
        ex.Should().BeOfType<InvalidOperationException>();
        ex.Message.Should().Be("LoopAction");

        TestResultHelper.SetFinishTest();
    }

    [ServiceMethod(59)]
    private async ZFTask GenericZFTask_NestedStringGenericZFTaskLoopException_AsyncThrow_CapturedInTask()
    {
        int loop = 0;
        async ZFTask<string> LoopExceptionAction()
        {
            await Service.NextFrame();
            loop++;
            if (loop == 3)
            {
                throw new InvalidOperationException("LoopAction");
            }
            return loop.ToString();
        }
        async ZFTask<string> LoopExceptionAction2()
        {
            return await LoopExceptionAction();
        }
        async ZFTask<string> LoopExceptionAction3()
        {
            return await LoopExceptionAction2();
        }
        async ZFTask<string> LoopExceptionAction4()
        {
            return await LoopExceptionAction3();
        }
        Exception? ex = null;
        try
        {
            await GenericTaskLoop(5, loopAwaitFunc2: LoopExceptionAction4);
        }
        catch (Exception e)
        {
            ex = e;
        }
        loop.Should().Be(3);
        ex.Should().NotBeNull();
        ex.Should().BeOfType<InvalidOperationException>();
        ex.Message.Should().Be("LoopAction");

        TestResultHelper.SetFinishTest();
    }

    [ServiceMethod(60)]
    private async ZFTask ZFTask_ComplexFlow_AsyncExceptionTryCatchFinally_ExecutesFinally(int args = 0)
    {
        var continueThrow = args != 0;
        var finallyExecuted = false;
        var continueThrowed = false;
        int cnt = 0;
        async ZFTask TryCatch_AsyncThrow()
        {
            try
            {
                cnt++;
                if (cnt >= 2)
                {
                    throw new InvalidOperationException();
                }
                await Service.NextFrame();
            }
            catch
            {
                if (continueThrow)
                {
                    continueThrowed = true;
                    throw;
                }
            }
            finally
            {
                finallyExecuted = true;
            }
        }

        var task = TaskLoop<int>(5, loopAwaitFunc: TryCatch_AsyncThrow);
        task.IsCompleted.Should().BeFalse();
        finallyExecuted.Should().BeFalse();
        continueThrowed.Should().BeFalse();

        await Service.NextFrame();

        task.IsCompleted.Should().BeTrue();
        finallyExecuted.Should().BeTrue();
        continueThrowed.Should().Be(continueThrow);
        task.IsFaulted.Should().Be(continueThrowed);

        Exception? ex = null;
        try
        {
            await task;
        }
        catch (Exception e)
        {
            ex = e;
        }

        if (continueThrow)
        {

            ex.Should().NotBeNull();
            ex.Should().BeOfType<InvalidOperationException>();
        }
        else
        {
            ex.Should().BeNull();
        }

        TestResultHelper.SetFinishTest();
    }

    [ServiceMethod(61)]
    private async ZFTask ZFTask_ComplexFlow_SyncExceptionTryCatchFinally_ExecutesFinally(int args = 0)
    {
        var continueThrow = args != 0;
        var finallyExecuted = false;
        var continueThrowed = false;
#pragma warning disable CS1998 // 异步方法缺少 "await" 运算符，将以同步方式运行
        async ZFTask TryCatch_SyncThrow()
        {
            try
            {
                throw new InvalidOperationException();
            }
            catch
            {
                if (continueThrow)
                {
                    continueThrowed = true;
                    throw;
                }
            }
            finally
            {
                finallyExecuted = true;
            }
        }
#pragma warning restore CS1998 // 异步方法缺少 "await" 运算符，将以同步方式运行
        int loop = 5;
        var task = TaskLoop<int>(loop, loopAwaitFunc: TryCatch_SyncThrow);
        task.IsCompleted.Should().BeTrue();
        task.IsFaulted.Should().Be(continueThrowed);
        finallyExecuted.Should().BeTrue();
        continueThrowed.Should().Be(continueThrow);

        Exception? ex = null;
        try
        {
            await task;
        }
        catch (Exception e)
        {
            ex = e;
        }

        if (continueThrow)
        {

            ex.Should().NotBeNull();
            ex.Should().BeOfType<InvalidOperationException>();
        }
        else
        {
            ex.Should().BeNull();
        }

        TestResultHelper.SetFinishTest();
    }

    [ServiceMethod(62)]
    private async ZFTask GenericZFTask_ComplexFlow_SyncExceptionTryCatchFinally_ExecutesFinally(int args = 0)
    {
        var continueThrow = args != 0;
        var finallyExecuted = false;
        var continueThrowed = false;
#pragma warning disable CS1998 // 异步方法缺少 "await" 运算符，将以同步方式运行
        async ZFTask<int> TryCatch_SyncThrow()
        {
            try
            {
                throw new InvalidOperationException();
            }
            catch
            {
                if (continueThrow)
                {
                    continueThrowed = true;
                    throw;
                }
                return 42;
            }
            finally
            {
                finallyExecuted = true;
            }
        }
#pragma warning restore CS1998 // 异步方法缺少 "await" 运算符，将以同步方式运行
        int loop = 5;
        var task = GenericTaskLoop<int>(loop, loopAwaitFunc2: TryCatch_SyncThrow);
        task.IsCompleted.Should().BeFalse();
        finallyExecuted.Should().BeFalse();
        continueThrowed.Should().BeFalse();
        for (int i = 0; i < loop * 2; i++)
        {
            await Service.NextFrame();
        }

        task.IsCompleted.Should().BeTrue();
        finallyExecuted.Should().BeTrue();
        continueThrowed.Should().Be(continueThrow);
        task.IsFaulted.Should().Be(continueThrowed);

        var returnRet = 0;
        Exception? ex = null;
        try
        {
            returnRet = await task;
        }
        catch (Exception e)
        {
            ex = e;
        }

        if (continueThrow)
        {

            ex.Should().NotBeNull();
            ex.Should().BeOfType<InvalidOperationException>();
            returnRet.Should().Be(0);
        }
        else
        {
            ex.Should().BeNull();
            returnRet.Should().Be(42);
        }

        TestResultHelper.SetFinishTest();
    }

    [ServiceMethod(63)]
    private async ZFTask GenericZFTask_ComplexFlow_AsyncExceptionTryCatchFinally_ExecutesFinally(int args = 0)
    {
        var continueThrow = args != 0;
        var finallyExecuted = false;
        var continueThrowed = false;
        int cnt = 0;
#pragma warning disable CS1998 // 异步方法缺少 "await" 运算符，将以同步方式运行
        async ZFTask<int> TryCatch_AsyncThrow()
        {
            try
            {
                cnt++;
                if (cnt >= 2)
                {
                    throw new InvalidOperationException();
                }
                await Service.NextFrame();
                return 43;
            }
            catch
            {
                if (continueThrow)
                {
                    continueThrowed = true;
                    throw;
                }
                return 42;
            }
            finally
            {
                finallyExecuted = true;
            }
        }
#pragma warning restore CS1998 // 异步方法缺少 "await" 运算符，将以同步方式运行

        int loop = 5;
        var task = GenericTaskLoop<int>(loop, loopAwaitFunc2: TryCatch_AsyncThrow);
        task.IsCompleted.Should().BeFalse();
        finallyExecuted.Should().BeFalse();
        continueThrowed.Should().BeFalse();

        for (int i = 0; i < loop * 2; i++)
        {
            await Service.NextFrame();
        }

        task.IsCompleted.Should().BeTrue();
        finallyExecuted.Should().BeTrue();
        continueThrowed.Should().Be(continueThrow);
        task.IsFaulted.Should().Be(continueThrowed);

        var returnRet = 0;
        Exception? ex = null;
        try
        {
            returnRet = await task;
        }
        catch (Exception e)
        {
            ex = e;
        }

        if (continueThrow)
        {

            ex.Should().NotBeNull();
            ex.Should().BeOfType<InvalidOperationException>();
            returnRet.Should().Be(0);
        }
        else
        {
            ex.Should().BeNull();
            returnRet.Should().Be(42);
        }

        TestResultHelper.SetFinishTest();
    }

    [ServiceMethod(71)]
    public async ZFTask ZFTask_InfiniteAwaitLoop_DoesNotStackOverflow()
    {
        var iterations = 0;
        int cnt = 10000;

        async ZFTask InfiniteLoop()
        {
            while (iterations < cnt)  // 限制避免真无限
            {
                await Service.NextFrame();
                iterations++;
            }
        }

        await InfiniteLoop();
        iterations.Should().Be(cnt);
        TestResultHelper.SetFinishTest();
    }

    [ServiceMethod(72)]
    public async ZFTask GenericZFTask_InfiniteAwaitLoop_DoesNotStackOverflow()
    {
        var iterations = 0;
        int cnt = 10000;

        async ZFTask<T> InfiniteLoop<T>()
        {
            while (iterations < cnt)  // 限制避免真无限
            {
                await Service.NextFrame();
                iterations++;
            }
            object converted = Convert.ChangeType(iterations, typeof(T));
            var ret = (T)converted;
            return ret;
        }

        var res1 = await InfiniteLoop<int>();
        var res2 = await InfiniteLoop<string>();
        res1.Should().Be(cnt);
        res2.Should().Be(cnt.ToString());
        TestResultHelper.SetFinishTest();
    }

    [ServiceMethod(73)]
    public async ZFTask ZFTask_DeepRecursion_DoesNotCrash()
    {
        var recurseCnt = 0;
        async ZFTask Recurse(int depth)
        {
            if (depth <= 0) return;
            await Service.NextFrame();
            recurseCnt += 1;
            await Recurse(depth - 1);
        }

        await Recurse(100);
        recurseCnt.Should().Be(100);


        TestResultHelper.SetFinishTest();
    }

    [ServiceMethod(74)]
    public async ZFTask GenericZFTask_DeepRecursion_DoesNotCrash()
    {
        async ZFTask<int> Recurse(int depth)
        {
            if (depth <= 0) return 0;
            await Service.NextFrame();
            return await Recurse(depth - 1) + 1;
        }

        var res = await Recurse(100);
        res.Should().Be(100);

        TestResultHelper.SetFinishTest();
    }

}