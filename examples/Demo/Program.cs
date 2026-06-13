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



using Proxar.AppHost;
using Proxar.ServiceCore;
using Proxar.Tasks;

namespace MyGame.Demo;

// 1. 定义服务
internal partial class HelloService : ServiceBase
{
    [ServiceMethod(1)]
    public async ZFTask<string> Greet(string name)
    {
        await ZFTask.CompletedTask;
        return $"Hello, {name}!";
    }

    [ServiceMethod(2)]
    public void ReportOnline(long playerId)
    {
        Console.WriteLine($"Player {playerId} is online.");
    }
}

// 2. 启动入口
internal class Program
{
    public static void Main(string[] args)
    {
        AppHostBuilder.Instance
            .SetCommandLineArgs(args)
            .ConfigureDefaultServiceGroup(hostCluster =>
            {
                hostCluster.AddServiceGroupStartAction(async serviceId =>
                {
                    var helloServiceId = Service.CreateService<HelloService>();
                    await CallHelloService(helloServiceId);
                });
            })
            .Build();
    }

    public static async ZFTask CallHelloService(long helloServiceId)
    {
        // 创建代理，传入目标服务的唯一 ID
        var proxy = Service.GetServiceProxy<HelloServiceProxy>(helloServiceId);

        // 单向消息：无需等待响应
        proxy.ReportOnline(999);

        // 可等待 RPC：调用远程方法并获取返回结果
        string result = await proxy.Greet("Proxar");
        Console.WriteLine(result); // 输出: Hello, Proxar!

    }
}