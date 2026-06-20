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


using Proxar.ActorSingletonCore.Interfaces;
using Proxar.IdGenerator;
using Proxar.IdGenerator.Interfaces;
using Proxar.ServiceCore;
using Proxar.ServiceCore.Message;
using Proxar.Tasks.Interfaces;

namespace Proxar.ServiceSynchronizationContext;


/// <summary>
/// 为 <see cref="ServiceBase"/> 提供的专用同步上下文，确保所有对服务的操作都在其专属的 Actor 线程上执行。
/// 负责管理待处理的异步任务、单例对象以及与调度线程的绑定。
/// </summary>
public sealed class ActorSynchronizationContext : AbstractSynchronizationContext
{
    private ServiceBase attachedService;
    private IIdGenerator<long> timerInt64IdGenerator = new Int64IdGenerator() { InitValue = 1 };
    private HashSet<ITask> pendingZFTask = new HashSet<ITask>();

    /// <summary>
    /// 与此上下文关联的 <see cref="IActorSingleton"/> 列表，用于管理服务的单例生命周期。
    /// </summary>
    internal List<IActorSingleton?> ActorSingletons { get; set; } = new List<IActorSingleton?>(100);

    /// <summary>
    /// 初始化一个新的 <see cref="ActorSynchronizationContext"/> 实例。
    /// </summary>
    /// <param name="service">拥有此上下文的 <see cref="ServiceBase"/> 实例。</param>
    public ActorSynchronizationContext(ServiceBase service)
    {
        this.attachedService = service;
    }

    /// <summary>
    /// 获取与此同步上下文关联的服务实例。
    /// </summary>
    /// <returns><see cref="ServiceBase"/> 实例。</returns>
    public ServiceBase GetService()
    {
        return attachedService;
    }

    /// <summary>
    /// 初始化 Actor 同步上下文（预留的初始化钩子）。
    /// </summary>
    internal void InitActorSynchronizationContext()
    {
    }

    /// <summary>
    /// 将一个回调及其状态对象投递到 Actor 的消息队列中，由 Actor 线程异步执行。
    /// </summary>
    /// <param name="d">要调用的回调。</param>
    /// <param name="state">传递给回调的状态对象。</param>
    public override void Post(SendOrPostCallback d, object? state)
    {
        if (this.attachedService == null)
        {
            return;
        }
        var msg = new ContextActionMessage(d, state);
        msg.Init();
        this.attachedService.PushMessage(msg);
    }

    /// <summary>
    /// 将一个操作投递到 Actor 的消息队列中，由 Actor 线程异步执行。
    /// </summary>
    /// <param name="action">要执行的操作。</param>
    public override void Post(Action action)
    {
        if (this.attachedService == null)
        {
            return;
        }
        var msg = new ContextActionMessage(action);
        msg.Init();
        this.attachedService.PushMessage(msg);
    }

    internal IIdGenerator<long> GetTimerIdGenerator()
    {
        return timerInt64IdGenerator;
    }

    /// <summary>
    /// 关闭服务并清理所有待处理的任务、消息和单例对象。
    /// </summary>
    public void CloseServiceHandler()
    {
        foreach (var task in pendingZFTask)
        {
            task.Cancel();
        }
        pendingZFTask.Clear();
        this.attachedService?.ClearAllMessage();
        this.attachedService = null!;
        foreach (var singleInstance in ActorSingletons)
        {
            singleInstance?.Dispose();
        }
        ActorSingletons.Clear();
    }

    /// <summary>
    /// 将一个待处理的 <see cref="ITask"/> 注册到此上下文中，以便在服务关闭时统一取消。
    /// </summary>
    /// <param name="task">要注册的任务。</param>
    internal void RegisterPenddingZFTask(ITask task)
    {
        pendingZFTask.Add(task);
    }

    /// <summary>
    /// 从此上下文中注销一个已完成的 <see cref="ITask"/>。
    /// </summary>
    /// <param name="task">要注销的任务。</param>
    internal void UnRegisterPenddingZFTask(ITask task)
    {
        pendingZFTask.Remove(task);
    }

    /// <summary>
    /// 将当前执行线程绑定到此 Actor 的线程作用域中。
    /// </summary>
    public void BindThread()
    {
        ActorThreadScope.BindActor();
    }
}