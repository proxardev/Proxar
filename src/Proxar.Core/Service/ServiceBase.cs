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


using Proxar.AppHost.Interfaces;
using Proxar.IdGenerator.Interfaces;
using Proxar.IdGenerator.SnowflakeId;
using Proxar.ServiceCore.Interfaces;
using Proxar.ServiceSynchronizationContext;
using Proxar.Threading;

namespace Proxar.ServiceCore;

/// <summary>
/// 所有服务的抽象基类，提供消息队列、服务标识、状态管理等核心能力。
/// 每个服务都是一个独立的 Actor，拥有自己的消息队列和同步上下文，确保业务逻辑在单线程上执行。
/// </summary>
public abstract partial class ServiceBase
{
    private long serviceId;
    private Queue<IServiceMessage>? queue = new Queue<IServiceMessage>(100);
    private Queue<IServiceMessage> queue0 = new Queue<IServiceMessage>();
    private ActorSynchronizationContext actorSynchronizationContext;

    private bool inGlobalQueue = false;

    private SpinLockScope msgSpinLock { get; } = new SpinLockScope();

    private IIdGenerator<long> msgSeqGenerator { get; set; } = SnowflakeIdHelper.CreateSnowflakeIdGenerator();


    private ServiceStatue serviceStatue = ServiceStatue.None;

    /// <summary>
    /// 获取此服务所属的服务组。
    /// </summary>
    public IServiceGroup ServiceGroup { get; init; }

    /// <summary>
    /// 初始化 <see cref="ServiceBase"/> 类的新实例。
    /// </summary>
    public ServiceBase()
    {
        actorSynchronizationContext = new ActorSynchronizationContext(this);
        ServiceGroup = ActorThreadScope.ServiceGroup;
    }

    /// <summary>
    /// 生成一个新的消息序列号（MsgSeq），用于请求-响应匹配。
    /// </summary>
    /// <returns>一个新的唯一消息序列号。</returns>
    protected internal long NewMessageSeq()
    {
        return msgSeqGenerator.NewId();
    }

    /// <summary>
    /// 设置此服务的唯一标识符。
    /// </summary>
    /// <param name="serviceId">服务的唯一标识符。</param>
    internal void SetServiceId(long serviceId)
    {
        this.serviceId = serviceId;
    }

    /// <summary>
    /// 获取此服务的唯一标识符。
    /// </summary>
    /// <returns>服务的唯一标识符。</returns>
    public long GetServiceId()
    {
        return this.serviceId;
    }

    /// <summary>
    /// 将消息推送到此服务的消息队列中，由服务工作线程异步处理。
    /// </summary>
    /// <param name="message">要入队的服务消息。</param>
    public void PushMessage(IServiceMessage message)
    {
        this.msgSpinLock.Execute(this.PushMessage2, message);
    }

    private void PushMessage2(IServiceMessage message)
    {
        if (this.queue == null)
        {
            return;
        }
        this.queue.Enqueue(message);
        this.CheckPushGlobalQueue();
    }

    internal void PushQueue0Message(IServiceMessage message)
    {
        this.msgSpinLock.Execute(this.PushQueue0Message2, message);
    }

    private void PushQueue0Message2(IServiceMessage message)
    {
        if (this.queue0 == null)
        {
            return;
        }
        this.queue0.Enqueue(message);
        this.CheckPushGlobalQueue();
    }

    private void CheckPushGlobalQueue()
    {
        if (inGlobalQueue)
        {
            return;
        }
        inGlobalQueue = true;
        ServiceManager.Instance.PushGlobalQueue(serviceId);
    }

    internal void ClearAllMessage()
    {
        this.msgSpinLock.Execute(this.ClearAllMessage2);
    }

    private void ClearAllMessage2()
    {
        this.InvalidQueue12();
        this.queue0.Clear();
        if (this.queue == null)
        {
            return;
        }
        var count = this.queue.Count;
        for (int i = 0; i < count; i++)
        {
            this.queue.Dequeue();
        }
    }

    internal void InvalidQueue1()
    {
        this.msgSpinLock.Execute(this.InvalidQueue12);
    }

    private void InvalidQueue12()
    {
        this.queue = null;
    }

    private void SetServiceStatueRunning()
    {
        this.serviceStatue = ServiceStatue.Running;
    }

    private void SetServiceStatueWaitClose()
    {
        this.serviceStatue = ServiceStatue.WaitClose;
    }

    private void SetServiceStatueClose()
    {
        this.serviceStatue = ServiceStatue.Close;
    }

    /// <summary>
    /// 获取此服务的当前状态。
    /// </summary>
    /// <returns>一个 <see cref="ServiceStatue"/> 枚举值。</returns>
    public ServiceStatue GetServiceStatue()
    {
        return this.serviceStatue;
    }

    /// <summary>
    /// 检查此服务是否处于指定状态。
    /// </summary>
    /// <param name="serviceStatue">要检查的状态。</param>
    /// <returns>如果服务处于指定状态则返回 <c>true</c>，否则返回 <c>false</c>。</returns>
    public bool IsServiceStatue(ServiceStatue serviceStatue)
    {
        return this.serviceStatue == serviceStatue;
    }
}