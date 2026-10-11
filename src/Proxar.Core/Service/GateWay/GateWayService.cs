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
using Proxar.Logging;
using Proxar.Network.Interfaces;
using Proxar.ServiceCore.Interfaces;
using Proxar.ServiceCore.Message;
using System.Buffers;
using System.Diagnostics;

namespace Proxar.ServiceCore.GateWay;


[KnownService]
internal partial class GateWayService : ServiceBase, IKnownService
{
    private static bool OpenDebug = false;

    private Dictionary<long, IChannel> channels = new();
    private Dictionary<long, IChannel> target2Channels = new();
    private Dictionary<IChannel, long> channel2TargetIds = new();
    private Dictionary<long, ExternalServiceMapping> externalServicesMapping = new();
    private Dictionary<long, ChannelRequestRecord> rpcConvertTable = new();


    [Conditional("DEBUG")]
    private void DebugLog(string message)
    {
        if (!OpenDebug)
        {
            return;
        }
        var host = ActorThreadScope.ServiceGroup;
        var msg = $"{host.Flag},{GetServiceId()}|{message}";
        ProxarLogger.TestDebugLog(msg);
    }

    [ServiceMethod(1)]
    [ServiceMethodAction(true)]
    private void AddChannel(IChannel channel)
    {
        DebugLog($"AddChannel {channel.Id}");
        channels[channel.Id] = channel;
    }

    [ServiceMethod]
    [ServiceMethodAction(true)]
    private void RemoveChannel(IChannel channel)
    {
        DebugLog($"RemoveChannel {channel.Id}");
        channels.Remove(channel.Id);
        if (channel2TargetIds.TryGetValue(channel, out var targetId))
        {
            channel2TargetIds.Remove(channel);
            target2Channels.Remove(targetId);
        }
    }


    [ServiceMethod(10)]
    [ServiceMethodAction(true)]
    private void SetTarget2Channel(long targetId, long channelId)
    {
        if (!channels.TryGetValue(channelId, out var channel))
        {
            DebugLog($"SetTarget2Channel fail {channelId}");
            return;
        }
        DebugLog($"SetTarget2Channel {channel.Id} {targetId}");
        target2Channels[targetId] = channel;
        channel2TargetIds[channel] = targetId;
    }

    [ServiceMethod(11)]
    [ServiceMethodAction(true)]
    private void RemoveTarget2Channel(long targetId)
    {
        if (target2Channels.TryGetValue(targetId, out var channel))
        {
            DebugLog($"RemoveTarget2Channel {channel.Id} {targetId}");
            target2Channels.Remove(targetId);
            channel2TargetIds.Remove(channel);
        }
        else
        {
            DebugLog($"RemoveTarget2Channel fail {targetId}");
        }
    }

    [ServiceMethod(40)]
    private void SetTargetServiceMapping(long targetId, long ProxyId, long serviceId)
    {
        var mapping = this.externalServicesMapping!.Get(targetId);
        if (mapping == null)
        {
            mapping = this.externalServicesMapping[targetId] = new();
        }
        DebugLog($"SetTargetServiceMapping {targetId} {ProxyId} {serviceId}");
        mapping.SetMapping(ProxyId, serviceId);
    }

    [ServiceMethod(41)]
    private void RemoveTargetServiceMapping(long targetId)
    {
        this.externalServicesMapping.Remove(targetId);
    }

    [ServiceMethod(42)]
    private void RemoveTargetServiceProxyMapping(long targetId, long proxyId)
    {
        if (!this.externalServicesMapping.TryGetValue(targetId, out var mapping))
        {
            return;
        }
        mapping.RemoveMapping(targetId);
    }

    private long GetMappingServiceId(long targetId, long proxyId)
    {
        var mapping = this.externalServicesMapping!.Get(targetId);
        if (mapping == null)
        {
            return 0;
        }
        return mapping.GetMapping(proxyId);
    }

    protected override void OnRpcCallBack(long callbackRpcId, IServiceMessage serviceMessage)
    {
        var succ = this.rpcConvertTable.TryGetValue(callbackRpcId, out var result);
        if (!succ)
        {
            base.OnRpcCallBack(callbackRpcId, serviceMessage);
            return;
        }
        this.rpcConvertTable.Remove(callbackRpcId);
        var targetId = result!.TargetId;
        var rpcId = result.RpcId;

        var fromServiceId = result.FromServiceId;
        var proxyId = result.ProxyId;

        if (fromServiceId != 0)
        {
            var payloadReadOnlyMemory = serviceMessage.GetPayloadReadOnlyMemory();
            var gateServiceMessage = new GateServiceMessage(payloadReadOnlyMemory);
            var header = new MessageHeader();
            header.Set(HeaderSlot.Seq, rpcId);
            gateServiceMessage.SetHeadData(0, fromServiceId, serviceMessage.GetSeq(), serviceMessage.GetProto(), header);
            Service.MessageInvoker.Send(0, fromServiceId, gateServiceMessage);
        }
        else if (proxyId != 0)
        {
            var channel = this.target2Channels.GetValueOrDefault(targetId);
            if (channel == null)
            {
                return;
            }
            var payloadReadOnlyMemory = serviceMessage.GetPayloadReadOnlyMemory();
            var gateServiceMessage = new GateServiceMessage(payloadReadOnlyMemory);
            var header = new MessageHeader();
            header.Set(HeaderSlot.Seq, rpcId);
            gateServiceMessage.SetHeadData(0, proxyId, serviceMessage.GetSeq(), serviceMessage.GetProto(), header);
            channel.Send(gateServiceMessage);
        }

        DebugLog($"rpc back now {callbackRpcId}, to seq:{rpcId}, from proxy:{proxyId}, from service:{fromServiceId}");

    }

    protected override void OnRpcCallBackError(long msgSeq, IServiceMessage serviceMessage)
    {
        DebugLog($"OnRpcCallBackError");
        var succ = this.rpcConvertTable.TryGetValue(msgSeq, out var result);
        if (!succ)
        {
            base.OnRpcCallBackError(msgSeq, serviceMessage);
            return;
        }
        this.OnRpcCallBack(msgSeq, serviceMessage);
    }

    [ServiceMethod(20)]
    [ServiceMethodAction(true)]
    private void ReceiveChannelData(IChannel channel, byte[] data)
    {
        this.ReceiveChannelData2(channel, new GatePayload(data));
    }


    [ServiceMethod(24)]
    [ServiceMethodAction(true)]
    private void ReceiveChannelData2(IChannel channel, IPayloadBuffer payloadBuffer)
    {
        var serviceMessage = new ServiceMessageForNet(payloadBuffer);
        var targetId = channel2TargetIds.Get(channel);
        serviceMessage.NetMessageDeserialize();
        var proxyId = serviceMessage.GetToServiceId();
        var toServiceId = GetMappingServiceId(targetId, proxyId);

        if (targetId == 0 || toServiceId == 0)
        {
            DebugLog($"ReceiveChannelData err");
            return;
        }

        var header = serviceMessage.GetHeader();
        header.Set(HeaderSlot.TargetId, targetId);

        var rpcId = serviceMessage.GetSeq();
        var newRpcId = rpcId;
        if (rpcId != 0)
        {
            newRpcId = this.NewMessageSeq();
            this.rpcConvertTable[newRpcId] = new ChannelRequestRecord()
            {
                TargetId = targetId,
                RpcId = rpcId,
                FromServiceId = 0,
                ProxyId = proxyId,
            };
        }

        DebugLog($"ReceiveChannelData rpc:{rpcId} new rpc {newRpcId}, proxy:{proxyId} to service:{toServiceId}");

        serviceMessage.SetToServiceId(toServiceId);
        serviceMessage.SetFromServiceId(this.GetServiceId());
        serviceMessage.SetMsgSeq(newRpcId);
        Service.MessageInvoker.Send(0, toServiceId, serviceMessage);
    }

    [ServiceMethod(21)]
    [ServiceMethodAction(true)]
    private void ReceiveData1(long channelId, byte[] data)
    {
        var channel = channels.GetValueOrDefault(channelId);
        if (channel == null)
        {
            return;
        }
        this.ReceiveChannelData(channel, data);
    }

    [ServiceMethod(22)]
    [ServiceMethodAction(true)]
    private void ReceiveData2(long channelId, IPayloadBuffer payloadBuffer)
    {
        var channel = channels.GetValueOrDefault(channelId);
        if (channel == null)
        {
            return;
        }
        this.ReceiveChannelData2(channel, payloadBuffer);
    }

    [ServiceMethod(30)]
    private void ForwardServiceMessage2Client(long fromServiceId, long targetId, long proxyId, long rpcId, int proto, byte[] payloadBytes)
    {
        TrySendToChannel(fromServiceId, targetId, proxyId, rpcId, proto, payloadBytes);
    }

    private void TrySendToChannel(long fromServiceId, long targetId, long proxyId, long rpcId, int proto, byte[] payloadBytes)
    {
        var channel = this.target2Channels.GetValueOrDefault(targetId);
        if (channel == null)
        {
            return;
        }
        DebugLog($"TrySendToChannel rpc:{rpcId}, fromServiceId:{proxyId} to proxy:{proxyId}");

        var gateServiceMessage = new GateServiceMessage(payloadBytes.AsMemory());
        gateServiceMessage.SetHeadData(0, proxyId, rpcId, proto);
        channel.Send(gateServiceMessage);

    }

    private sealed class GatePayload : IPayloadBuffer
    {
        private readonly byte[] payload;

        public GatePayload(byte[] payload)
        {
            this.payload = payload;
        }

        public ReadOnlyMemory<byte> Memory => payload.AsMemory();

        public ReadOnlySequence<byte> MemorySequence => new ReadOnlySequence<byte>(payload);

        public void Dispose()
        {
        }
    }
}