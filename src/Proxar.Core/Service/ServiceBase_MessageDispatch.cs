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


using Proxar.Core.Extensions;
using Proxar.ServiceCore.Interfaces;
using Proxar.ServiceCore.Message;
using Proxar.Tasks;
using System.Net.Sockets;
namespace Proxar.ServiceCore;


public abstract partial class ServiceBase
{

    private IServiceMessage? curMessage;
    private long curMsgSourceServiceId;
    private long curMsgSeq;
    private readonly Dictionary<long, Type> _msgSeq2TypeTable = new();

    private Dictionary<Type, IPendingTable> penddingTable = new();
    private Dictionary<long, IPendingTable> mesSeq2penddingTable = new();

    private PendingTable<T> GetTable<T>()
    {
        var type = typeof(T);
        if (!penddingTable.ContainsKey(type))
        {
            penddingTable[type] = new PendingTable<T>();
        }
        return (penddingTable[type] as PendingTable<T>)!;
    }


    //protected abstract Dictionary<Type, object> GetPendingTable();


    internal ZFTask<T> SetPendingTable<T>(long msgSeq)
    {
        var table = GetTable<T>();
        var task = ZFTask<T>.CreateZFTask();
        //this.SetMsgCallResultType(msgSeq, typeof(T));
        table.Add(msgSeq, task);
        mesSeq2penddingTable[msgSeq] = table;
        return task;
    }

    internal IPendingTable? PopPenddingTableByMsgSeq(long msgSeq)
    {
        var table = mesSeq2penddingTable!.Get(msgSeq);

        mesSeq2penddingTable.Remove(msgSeq);
        return table;
    }

    //protected string? GetCallResultTypeStringName(Type type)
    //{
    //    var table = this.GetType2StringNameDict();
    //    return table!.Get(type);
    //}

    //protected abstract Dictionary<Type, string> GetType2StringNameDict();

    private void SetMsgCallResultType(long msgSeq, Type type)
    {
        _msgSeq2TypeTable[msgSeq] = type;
    }

    protected Type? GetMsgCallResultType(long msgSeq)
    {
        if (!_msgSeq2TypeTable.ContainsKey(msgSeq))
        {
            return null;
        }
        return _msgSeq2TypeTable[msgSeq];
    }

    protected void RemoveMsgCallResultType(long msgSeq)
    {
        this._msgSeq2TypeTable.Remove(msgSeq);
    }

    protected int GetCallWaitCOunt()
    {
        return this._msgSeq2TypeTable.Count;
    }

    public void Dispatch()
    {
        var msg = this.curMessage!;
        this.curMsgSourceServiceId = msg.GetFromServiceId();
        this.curMsgSeq = msg.GetSeq();
        var proto = msg.GetProto();
        try
        {
            this.Dispatch(proto, msg);
        }
        catch (Exception e)
        {
            HandleDispatchError(e);
            throw;
        }
        finally
        {
            this.curMessage!.Dispose();
            this.curMessage = default;
            this.curMsgSourceServiceId = 0;
            this.curMsgSeq = 0;
        }
    }

    private void HandleDispatchError(Exception exception)
    {
        TryResponseException(exception);
    }

    private void TryResponseException(Exception exception)
    {
        if (this.curMsgSeq == 0)
        {
            return;
        }
        if (this.curMsgSourceServiceId == 0)
        {
            return;
        }
        var header = new MessageHeader(this.curMsgSeq);
        Service.Send(this.curMsgSourceServiceId, ProtoBase.RpcCallbackError, exception.Message, header: header);
    }

#pragma warning disable P0004 // 服务方法保留协议约束

    [ServiceMethod(ProtoBase.ContextAction)]
    private void ExecuteContextAction()
    {
        var msg = this.curMessage as ContextActionMessage;
        try
        {
            msg!.Execute();
        }
        catch (Exception)
        {
            throw;
        }
    }

    [ServiceMethod(ProtoBase.RpcCallBack)]
    private void ExecuteRpcCallBack()
    {
        var msgSeq = this.curMessage!.ReadHead<long>();
        this.OnRpcCallBack(msgSeq, this.curMessage!);
    }

    [ServiceMethod(ProtoBase.RpcCallbackError)]
    private void ExecuteRpcCallBackError()
    {
        var msgSeq = this.curMessage!.ReadHead<long>();
        this.OnRpcCallBackError(msgSeq, this.curMessage!);
    }

    [ServiceMethod(ProtoBase.ReceiveSocket)]
    private void ExecuteReceiveSocket()
    {
        //var msg = this.CurMessage as MessageSocket;
        //msg = Assert.AssertNotNull(msg);
        //var socket = msg.socket;
        //this.OnReceiveSocket(socket);
    }

    [ServiceMethod(ProtoBase.ServiceStart)]
    private void ExecuteServiceStart()
    {
        var msg = this.curMessage as Message_ServiceStartUp;
        // 初始化服务
        this.InitService();
        this.SetServiceStatueRunning();
        msg!.Execute(this.serviceId).Coroutine();
    }

    [ServiceMethod(ProtoBase.ServiceClose, true)]
    private async ZFTask<int> ExecuteServiceClose()
    {
        await ZFTask.CompletedTask;
        var reply = this.PackReplyAction<int>()!;
        this.CloseService(reply).Coroutine();
        return 0;
    }

    [ServiceMethod(ProtoBase.ServiceClose2)]
    [ServiceMethodAction(true, true)]
    private void ExecuteServiceClose2(Action<int> reply)
    {
        this.CloseService2();
        reply((int)ServiceStatue.Close);
    }

    [ServiceMethod(ProtoBase.EchoConfirm)]
    private int ExecuteEchoConfirm()
    {
        Reply(1);
        return 1;
    }

#pragma warning restore P0004 // 服务方法保留协议约束

    protected virtual void OnRpcCallBack(long msgSeq, IServiceMessage serviceMessage)
    {
        var penddingTable = this.PopPenddingTableByMsgSeq(msgSeq);
        penddingTable?.SetResult(msgSeq, serviceMessage);
    }

    protected virtual void OnRpcCallBackError(long msgSeq, IServiceMessage serviceMessage)
    {
        var penddingTable = this.PopPenddingTableByMsgSeq(msgSeq);
        penddingTable?.SetException(msgSeq, serviceMessage);
    }

    private void InitService()
    {
        this.actorSynchronizationContext
            .InitActorSynchronizationContext();
    }

    private async ZFTask CloseService(Action<int> reply)
    {
        if (this.IsServiceStatue(ServiceStatue.WaitClose))
        {
            reply((int)ServiceStatue.WaitClose);
            return;
        }
        this.SetServiceStatueWaitClose();
        await this.OnCloseService();

        var proxy = Service.GetServiceProxy<ServiceBaseProxy>(this.GetServiceId());


        // 发送一个消息，回应后，立即执行退出
        // 回应则说明此前的队列内容执行完了，后续的内容不再理会
        await proxy.ExecuteEchoConfirm();

        // 清理
        this.ClearAllMessage();
        // 插入最后的关闭消息，执行最后的关闭逻辑
        proxy.Queue0Raw.ExecuteServiceClose2(reply);

    }

    private void CloseService2()
    {
        // 让本线程继续读取多一次消息。将会读取到空消息，解除对本服务的继续消费
        this.willConsumeCount++;
        this.SetServiceStatueClose();
        ServiceManager.Instance.RemoveService(this.GetServiceId());
        this.ClearService();
    }


    internal void ClearService()
    {
        this.ClearAllMessage();
        this.actorSynchronizationContext.CloseServiceHandler();
    }


    protected virtual void OnReceiveSocket(Socket socket)
    {
        throw new NotImplementedException();
    }

    protected virtual async ZFTask OnCloseService()
    {
        await ZFTask.CompletedTask;
    }


    protected void Reply<T>(T result)
    {
        var fromServiceId = this.curMsgSourceServiceId;
        if (fromServiceId == 0)
        {
            return;
        }
        var mesSeq = this.curMsgSeq;
        this.curMsgSeq = 0;
        this.curMsgSourceServiceId = 0;
        this.DoReply(fromServiceId, mesSeq, result);
    }

    private void DoReply<T>(long targetServiceId, long msgSeq, T result)
    {
        if (targetServiceId == 0)
        {
            return;
        }
        var header = new MessageHeader(msgSeq);
        Service.Send(targetServiceId, ProtoBase.RpcCallBack, result, header: header);

    }

    protected Action<T>? PackReplyAction<T>()
    {
        var serviceId = this.curMsgSourceServiceId;
        var msgSeq = this.curMsgSeq;
        this.curMsgSeq = 0;
        this.curMsgSourceServiceId = 0;

        Action<T> action = (result) =>
        {
            this.DoReply(serviceId, msgSeq, result);
        };
        return action;
    }


    internal void ReplyRaw<T>(T? result)
    {
        var fromServiceId = this.curMsgSourceServiceId;
        if (fromServiceId == 0)
        {
            return;
        }
        var mesSeq = this.curMsgSeq;
        this.curMsgSeq = 0;
        this.curMsgSourceServiceId = 0;
        this.DoReplyRaw(fromServiceId, mesSeq, result);
    }

    private void DoReplyRaw<T>(long targetServiceId, long msgSeq, T result)
    {
        if (targetServiceId == 0)
        {
            return;
        }
        var header = new MessageHeader(msgSeq);
        Service.SendRaw(targetServiceId, ProtoBase.RpcCallBack, result, header: header);

    }

    internal Action<T>? PackReplyActionRaw<T>()
    {
        var serviceId = this.curMsgSourceServiceId;
        if (serviceId == 0)
        {
            return null;
        }
        var msgSeq = this.curMsgSeq;
        this.curMsgSeq = 0;
        this.curMsgSourceServiceId = 0;

        Action<T> action = (result) =>
        {
            this.DoReplyRaw(serviceId, msgSeq, result);
        };
        return action;
    }

    protected internal long GetResponseServiceId()
    {
        return this.curMsgSourceServiceId;
    }

    protected internal long GetResponseMsgIdx()
    {
        return this.curMsgSeq;
    }



    //public MessagePackReader GetUnpackReader()
    //{
    //    if (CurMessage is IServiceMessageReadOnlySequence readOnlySequence)
    //    {
    //        return new MessagePackReader(readOnlySequence.GetReadOnlySequence());
    //    }
    //    var reader = new MessagePackReader(CurMessage!.GetReadOnlyMemory());
    //    return reader;
    //}

    // 1 个参数
    //public Message PackAsync<T0>(int proto, T0 t0)
    //{
    //    var writer = PackHeader();
    //    writer.Write(proto);
    //    writer.Flush();

    //    MessagePackSerializer.Serialize(buf, t0);


    //    return new Message(new ArraySegment<byte>(buf.WrittenMemory.ToArray()));
    //}
    //public static Message Pack<T0>(long source, long index, int proto, T0 t0)
    //{
    //    var buf = new ArrayBufferWriter<byte>();
    //    var writer = new MessagePackWriter(buf);
    //    writer.Write(proto);
    //    writer.Flush();
    //    MessagePackSerializer.Serialize(buf, t0);


    //    return new Message(new ArraySegment<byte>(buf.WrittenMemory.ToArray()));
    //}

}