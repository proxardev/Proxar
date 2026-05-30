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
using Proxar.ServiceCore.Interfaces;
using Proxar.ServiceSynchronizationContext;
using Proxar.Tasks;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Reflection;
using Xunit.Abstractions;
namespace TestShared.TestClass;


public abstract class TestClassBaseWithOutput : TestClassBase
{
    public TestClassBaseWithOutput(ITestOutputHelper output)
    {
        _output = output;
    }
}

public abstract class TestClassBase
{
    private readonly ConcurrentQueue<TestResult> testResultQueue = new();
    protected long serviceId = 0;
    protected long id = 0;
    protected int curProto = 0;
    protected int args1 = 0;
    protected int args2 = 0;
    protected int args3 = 0;
    protected int args4 = 0;

    private Action<Exception?, string> resultAction = null!;

    // 1. 定义这个字段
    protected ITestOutputHelper? _output;


    protected virtual void CreateServiceCallBack(long serviceId)
    {

    }

    protected void SetFinishTest(long fromServiceId, long fromId, Exception? exception = null, string info = "")
    {
        var nowService = SynchronizationContextHelper
            .GetSynchronization<ActorSynchronizationContext>()?
            .GetService();
        var nowServiceId = nowService?.GetServiceId();
        Log($"{nowService?.GetType().Name} {nowServiceId} {nowServiceId == fromServiceId} finish {fromServiceId} {fromId}");
        var result = new TestResult(exception, TestState.Close, fromServiceId, fromId, info);
        testResultQueue.Enqueue(result);
    }

    private TestResult? TryGetTestResult()
    {
        testResultQueue.TryDequeue(out var result);
        return result;
    }

    public void WaitResult(int count)
    {
        Exception? exception = null;
        var msg = "";
        for (; ; )
        {

            var res = TryGetTestResult();
            if (res == null)
            {
                Thread.Sleep(1);
                continue;
            }
            if (res.FromId != id || id == 0)
            {
                throw new Exception($"error get other service result, id mine:{id}, other:{res.FromId}");
            }
            if (res.FromServiceId != serviceId || serviceId == 0)
            {
                throw new Exception($"error get other service result, service mine:{serviceId}, other:{res.FromServiceId}");
            }
            if (res.Exception != null && exception == null)
            {
                exception = res.Exception;
                msg = $"{id} {res.FromId}";
            }
            if (res.Info.Count() != 0)
            {
                _output?.WriteLine(res.Info);
            }
            count--;
            if (count <= 0)
            {
                break;
            }
        }
        GlobalTestResultSingleton.Instance.RemoveAction(resultAction);
        if (exception != null)
        {
            throw new Exception(exception.ToString() + msg);
        }
    }

    public async Task CheckCreateTargetTestService<T>()
        where T : ServiceBase, new()
    {
        if (serviceId != 0)
        {
            return;
        }
        //var registerServiceName = GetRegisterServiceName<T>();
        //var registerServiceId = Service.GetServiceIdByName(registerServiceName);
        //if (registerServiceId != 0)
        //{
        //    Log($"get register");
        //    serviceId = registerServiceId;
        //    return;
        //}

        serviceId = await InvokeCreateService(CreateTargetTestService<T>);
        //Service.SetService(registerServiceName, serviceId);
        Log($"create");
    }

    protected virtual async Task<long> InvokeCreateService(Func<Task<long>> func)
    {
        return await func.Invoke();
    }

    protected virtual string GetRegisterServiceName<T>()
        where T : ServiceBase
    {
        return typeof(T).Name;
    }

    private async Task<long> CreateTargetTestService<T>()
        where T : ServiceBase, new()
    {
        id = TestInitializer.Int32IdSafeGenerator.NewId();
        ServiceBootstrapper serviceBootstrapper = new ServiceBootstrapper();
        var task = new TaskCompletionSource();
        serviceBootstrapper.RegisterBootFunc(async createServiceId =>
        {
            var cur = IntergrationTestResultActorSingleton.Current;
            cur.Id = id;
            cur.TargetServiceId = createServiceId;

            resultAction = (ex, info) =>
            {
                SetFinishTest(createServiceId, id, ex, info);
            };
            IntergrationTestResultActorSingleton.Current
            .TestResultAction = resultAction;

            GlobalTestResultSingleton.Instance.AddAction(resultAction);

            CreateServiceCallBack(createServiceId);
            task.SetResult();
            await ZFTask.CompletedTask;
        }
        );
        var serviceId = Service.CreateService<T>(serviceBootstrapper);
        await task.Task;
        return serviceId;
    }

    public void Log(string msg)
    {
        var argsInfo = $"args:{args1},{args2},{args3},{args4}";
        var info = $"{TestInitializer.StartTimeStr}|N:{this.GetType().Name}|S{serviceId}|ID{this.id}|P{curProto}|{argsInfo}";
        TestLogHelper.TestDebugLog($"{info}|{msg}");
    }


    public async Task Call_TargetProto<T>(int proto, int args, int args2 = int.MinValue,
        int args3 = int.MinValue,
        int args4 = int.MinValue
        )
        where T : ServiceBase, new()
    {
        curProto = proto;
        this.args1 = args;
        this.args2 = args2;
        this.args3 = args3;
        this.args4 = args4;
        await TestInitializer.InitFinish.Task;


        await CheckCreateTargetTestService<T>();

        Log($"proto:{proto}");

        int count = 0;

        if (args2 != int.MinValue && args3 != int.MinValue && args4 != int.MinValue)
        {
            Service.Send(serviceId, (int)proto, args, args2, args3, args4);
        }
        else if (args2 != int.MinValue && args3 != int.MinValue)
        {
            Service.Send(serviceId, (int)proto, args, args2, args3);
        }
        else if (args2 != int.MinValue)
        {
            Service.Send(serviceId, (int)proto, args, args2);
        }
        else if (args != int.MinValue)
        {
            Service.Send(serviceId, (int)proto, args);
        }
        else
        {
            Service.Send(serviceId, (int)proto);
        }
        count++;

        WaitResult(count);
    }

    internal T GetServiceProxy<T>()
        where T : class, IServiceProxy
    {
        return Service.GetServiceProxy<T>(this.serviceId);
    }


    public async Task CallMethodWithExternalProxy<TService, TServiceProxy>(string methodName, int args, int args2 = int.MinValue,
        int args3 = int.MinValue,
        int args4 = int.MinValue
        )
        where TService : ServiceBase, new()
        where TServiceProxy : class, IExternalProxy
    {

        this.args1 = args;
        this.args2 = args2;
        this.args3 = args3;
        this.args4 = args4;
        await TestInitializer.InitFinish.Task;


        await CheckCreateTargetTestService<TService>();

        Log($"proto:{methodName} {Process.GetCurrentProcess().Id}");
        var proxy = ExternalService.GetExternalServiceProxy<TServiceProxy>(this.serviceId);

        await CallMethod(proxy, methodName, args, args2, args3, args4);
    }

    protected virtual object CreateProxy()
    {
        return null!;
    }


    public async Task CallMethodByProxy<TService, TServiceProxy>(string methodName, int args, int args2 = int.MinValue,
        int args3 = int.MinValue,
        int args4 = int.MinValue
        )
        where TService : ServiceBase, new()
        where TServiceProxy : class, IServiceProxy
    {

        this.args1 = args;
        this.args2 = args2;
        this.args3 = args3;
        this.args4 = args4;
        await TestInitializer.InitFinish.Task;


        await CheckCreateTargetTestService<TService>();

        Log($"proto:{methodName} {Process.GetCurrentProcess().Id}");
        var proxy = GetServiceProxy<TServiceProxy>();

        await CallMethod(proxy, methodName, args, args2, args3, args4);
    }


    private async Task CallMethod(object proxy, string methodName, int args, int args2 = int.MinValue,
        int args3 = int.MinValue,
        int args4 = int.MinValue
        )
    {

        this.args1 = args;
        this.args2 = args2;
        this.args3 = args3;
        this.args4 = args4;
        await TestInitializer.InitFinish.Task;

        int count = 0;
        var method = proxy.GetType().GetMethod(methodName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)!;

        if (args2 != int.MinValue && args3 != int.MinValue && args4 != int.MinValue)
        {
            method.Invoke(proxy, new object[] { args, args2, args3, args4 });
        }
        else if (args2 != int.MinValue && args3 != int.MinValue)
        {
            method.Invoke(proxy, new object[] { args, args2, args3 });
        }
        else if (args2 != int.MinValue)
        {
            method.Invoke(proxy, new object[] { args, args2 });
        }
        else if (args != int.MinValue)
        {
            method.Invoke(proxy, new object[] { args });
        }
        else
        {
            method.Invoke(proxy, null);
        }
        count++;

        WaitResult(count);
    }



    public async Task CallMethod<TService>(string methodName, int args, int args2 = int.MinValue,
        int args3 = int.MinValue,
        int args4 = int.MinValue
        )
        where TService : ServiceBase, new()
    {

        this.args1 = args;
        this.args2 = args2;
        this.args3 = args3;
        this.args4 = args4;
        await TestInitializer.InitFinish.Task;


        await CheckCreateTargetTestService<TService>();

        Log($"proto:{methodName} {Process.GetCurrentProcess().Id}");
        var proxy = CreateProxy();

        await CallMethod(proxy, methodName, args, args2, args3, args4);
    }

}