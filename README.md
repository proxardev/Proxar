# 描述

Proxar 是一个轻量、高性能的 .NET Actor 服务框架。它使用编译时生成的强类型代理统一服务调用的方式，让远程通信像本地方法一样简单且类型安全。

## 核心设计目标

- **轻量**：核心只做通信与 actor 调度，额外功能以扩展包提供。
- **高性能**：编译时生成代理，零反射；支持零拷贝消息转发；多线程并行消费 Actor 消息。
- **强类型**：服务调用统一走代理，协议方法标记即生成，编译期保证安全。

---
  
# 核心特性  
  
| 特性 | 说明 |  
| :--- | :--- |  
| **Actor 模型** | 每个服务（`ServiceBase`）拥有独立的消息队列和执行上下文，业务逻辑天然单线程，无需手动加锁 |  
| **自定义异步** | 自定义 `ZFTask` 类型替代标准 `Task`，通过对象池实现近零 GC 压力；生命周期与服务绑定，销毁时自动清理未完成任务 |  
| **编译期安全** | 自定义 Roslyn 分析器在编译阶段捕获协议不匹配等错误 |  
| **自动代码生成** | Source Generator 自动生成 RPC 代理的 `Send`/`Call` 扩展方法，消除样板代码 |  
| **透明远程调用** | 自动判断目标服务是本地还是远程节点，调用方无需感知 | 
| **内部/外部代理分离** | 通过 `ExternalProxy` 和 `InternalProxy` 物理隔离对外接口，客户端无法访问内部方法 | 
| **热更新支持** | 基于 `Harmony` 的无侵入热更新扩展，运行时加载补丁 DLL，支持版本校验和回滚 | 
  
---  

# 快速开始

## 基础准备工作

**创建一个新的 .NET 控制台项目，并安装 NuGet 包。**

```csharp
dotnet add package Proxar.Core
```

## 完整示例

以下是定义服务、创建实例、并通过代理调用它的完整流程：

```csharp

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

```


# 文档
更多架构说明、开发工具使用和测试指南正在编写中，欢迎贡献。


# 许可证
本项目基于 Apache-2.0 许可证开源。
