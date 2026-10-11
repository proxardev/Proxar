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
#pragma warning disable CS1591


using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System.Text;

namespace ZFSourceGenerator
{
    // 生成call和send函数
    // 产物拆分：
    //   1) Service           -> 真正的实现，全部 internal；仅 Send 系列
    //   2) ISecretService    -> public 接口（EditorBrowsable(Never)），仅包含 Send 系列；Call 不作为接口定义
    //                          并在其中分部定义 CallResult<TRet>，Call 系列为其实例方法
    //   3) SecretService     -> internal partial，显式接口实现（仅 Send，转发到 Service）
    [Generator(LanguageNames.CSharp)]
    public class ServiceCommunicationMethodGenerator : IIncrementalGenerator
    {
        /*
         * call和send函数生成0-9个参数重载版本
         */

        private const int Count = 9;

        // ==================== 原有 Service 实现体（不变） ====================

        public string MakeNeedSerializeTest(List<string> argsTemplateList)
        {
            if (argsTemplateList.Count == 0)
            {
                return "false";
            }
            var data = argsTemplateList
                .Select(x => $"Service.IsNeedSerialize<{x}>()");

            return string.Join("\n                    || ", data);
        }

        public string MakeMessageSetArgsStatement(List<string> argsList)
        {
            if (argsList.Count == 0)
            {
                return string.Empty;
            }
            var statement = "msg.SetArgs("
                + string.Join(", ", argsList)
                + ");";
            return statement;
        }

        public string MakeCheckSerializeStatement(List<string> argsTemplateList,
            bool checkSerialize)
        {
            if (!checkSerialize)
            {
                return string.Empty;
            }
            var statement = $@"
        var needSerialize = {this.MakeNeedSerializeTest(argsTemplateList)};
        if (needSerialize)
        {{
            msg.Serialize();
        }}";
            return statement;
        }

        public string MakeMessageHandleBody(List<string> argsTemplateList, List<string> argsList, bool checkSerialize = true, string pushMsgFunc = "Send",
            string fromServiceId = "0", string msgSeq = "0", string messageInvoker = "Service.MessageInvoker", string messageInvokerProxyId = "0")
        {
            var genericTypeStatement = Helper.MakeGenericStatement(argsTemplateList);
            var realArgsStatement = Helper.MakeRealArgsStatement(argsTemplateList, argsList);
            var setMsgArgsStatement = MakeMessageSetArgsStatement(argsList);
            var body = $@"
        var msg = new UniversaltServiceMessage{genericTypeStatement}();
        msg.SetHeadData({fromServiceId}, serviceId, {msgSeq}, proto, header);
        {setMsgArgsStatement}
{this.MakeCheckSerializeStatement(argsTemplateList, checkSerialize)};
        {messageInvoker}.{pushMsgFunc}({messageInvokerProxyId}, serviceId, msg);
";
            return body;
        }

        // ==================== Service 实现（全部 internal） ====================
        // Send 保持 static

        public string GetServiceSendFuncContent(List<string> argsTemplateList, List<string> argsList)
        {
            var genericTypeStatement = Helper.MakeGenericStatement(argsTemplateList);
            var realArgsStatement = Helper.MakeRealArgsStatement(argsTemplateList, argsList);
            var func = $@"
    internal static void Send{genericTypeStatement}(long serviceId, int proto,{realArgsStatement} IMessageHeader header = null)
    {{
{this.MakeMessageHandleBody(argsTemplateList, argsList)}
    }}

    internal static void Send{genericTypeStatement}(long serviceId, int proto,{realArgsStatement} IMessageInvoker messageInvoker, long ProxyId, IMessageHeader header = null)
    {{
{this.MakeMessageHandleBody(argsTemplateList, argsList, messageInvoker: "messageInvoker", messageInvokerProxyId: "ProxyId")}
    }}

    internal static void SendRaw{genericTypeStatement}(long serviceId, int proto,{realArgsStatement} IMessageHeader header = null)
    {{
{this.MakeMessageHandleBody(argsTemplateList, argsList, checkSerialize: false, pushMsgFunc: "SendRaw", messageInvoker: "Service.InternalMessageInvoker")}
    }}

    internal static void SendQueue0{genericTypeStatement}(long serviceId, int proto,{realArgsStatement} IMessageHeader header = null)
    {{
{this.MakeMessageHandleBody(argsTemplateList, argsList, pushMsgFunc: "PushQueue0Message", messageInvoker: "Service.InternalMessageInvoker")}
    }}

    internal static void SendQueue0Raw{genericTypeStatement}(long serviceId, int proto,{realArgsStatement} IMessageHeader header = null)
    {{
{this.MakeMessageHandleBody(argsTemplateList, argsList, checkSerialize: false, pushMsgFunc: "PushQueue0MessageRaw", messageInvoker: "Service.InternalMessageInvoker")}
    }}
";
            return func;
        }

        // ==================== CallResult<TRet> 实例方法（Call 系列） ====================
        // 定义在 ISecretService.CallResult<TRet> 分部结构体上，均为实例方法

        public string GetServiceCallFuncContent(List<string> argsTemplateList, List<string> argsList)
        {
            var resultType = "TRet";
            var genericTypeStatement = Helper.MakeGenericStatement(argsTemplateList.ToList());
            var realArgsStatement = Helper.MakeRealArgsStatement(argsTemplateList, argsList);

            var func = $@"
        public ZFTask<{resultType}> Call{genericTypeStatement}(
            ServiceBase fromService, long serviceId, int proto,{realArgsStatement} IMessageHeader header = null)
        {{
            var fromServiceId = fromService.GetServiceId();
            var msgSeq = fromService.NewMessageSeq();
            var ts = fromService.SetPendingTable<{resultType}>(msgSeq);

{this.MakeMessageHandleBody(argsTemplateList, argsList, fromServiceId: "fromServiceId", msgSeq: "msgSeq")}

            return ts;
        }}

        public ZFTask<{resultType}> Call{genericTypeStatement}(
            long serviceId, int proto, {realArgsStatement} IMessageHeader header = null)
        {{
            var fromService = ActorThreadScope.Service;
            var fromServiceId = fromService.GetServiceId();
            var msgSeq = fromService.NewMessageSeq();
            var ts = fromService.SetPendingTable<{resultType}>(msgSeq);

{this.MakeMessageHandleBody(argsTemplateList, argsList, fromServiceId: "fromServiceId", msgSeq: "msgSeq")}
            return ts;
        }}

        public ZFTask<{resultType}> Call{genericTypeStatement}(
            long serviceId, int proto, {realArgsStatement} IMessageInvoker messageInvoker, long ProxyId, IMessageHeader header = null)
        {{
            var fromService = ActorThreadScope.Service;
            var fromServiceId = fromService.GetServiceId();
            var msgSeq = fromService.NewMessageSeq();
            var ts = fromService.SetPendingTable<{resultType}>(msgSeq);

{this.MakeMessageHandleBody(argsTemplateList, argsList,
fromServiceId: "fromServiceId", msgSeq: "msgSeq", messageInvoker: "messageInvoker", messageInvokerProxyId: "ProxyId")}
            return ts;
        }}

        public ZFTask<{resultType}> CallRaw{genericTypeStatement}(
            long serviceId, int proto, {realArgsStatement} IMessageHeader header = null)
        {{
            var fromService = ActorThreadScope.Service;
            var fromServiceId = fromService.GetServiceId();
            var msgSeq = fromService.NewMessageSeq();
            var ts = fromService.SetPendingTable<{resultType}>(msgSeq);

{this.MakeMessageHandleBody(argsTemplateList, argsList, checkSerialize: false, pushMsgFunc: "SendRaw", fromServiceId: "fromServiceId", msgSeq: "msgSeq", messageInvoker: "Service.InternalMessageInvoker")}
            return ts;
        }}

        public ZFTask<{resultType}> CallQueue0{genericTypeStatement}(
            long serviceId, int proto, {realArgsStatement} IMessageHeader header = null)
        {{
            var fromService = ActorThreadScope.Service;
            var fromServiceId = fromService.GetServiceId();
            var msgSeq = fromService.NewMessageSeq();
            var ts = fromService.SetPendingTable<{resultType}>(msgSeq);

{this.MakeMessageHandleBody(argsTemplateList, argsList, checkSerialize: false, pushMsgFunc: "PushQueue0Message", fromServiceId: "fromServiceId", msgSeq: "msgSeq", messageInvoker: "Service.InternalMessageInvoker")}
            return ts;
        }}

        public ZFTask<{resultType}> CallQueue0Raw{genericTypeStatement}(
            long serviceId, int proto, {realArgsStatement} IMessageHeader header = null)
        {{
            var fromService = ActorThreadScope.Service;
            var fromServiceId = fromService.GetServiceId();
            var msgSeq = fromService.NewMessageSeq();
            var ts = fromService.SetPendingTable<{resultType}>(msgSeq);

{this.MakeMessageHandleBody(argsTemplateList, argsList, checkSerialize: false, pushMsgFunc: "PushQueue0MessageRaw", fromServiceId: "fromServiceId", msgSeq: "msgSeq", messageInvoker: "Service.InternalMessageInvoker")}
            return ts;
        }}
";
            return func;
        }

        // ==================== ISecretService 接口签名（仅 Send，Call 不作为接口定义） ====================

        public string GetInterfaceSendSignature(List<string> argsTemplateList, List<string> argsList)
        {
            var genericTypeStatement = Helper.MakeGenericStatement(argsTemplateList);
            var realArgsStatement = Helper.MakeRealArgsStatement(argsTemplateList, argsList);
            return $@"
    void Send{genericTypeStatement}(long serviceId, int proto,{realArgsStatement} IMessageHeader header = null);
    void Send{genericTypeStatement}(long serviceId, int proto,{realArgsStatement} IMessageInvoker messageInvoker, long ProxyId, IMessageHeader header = null);
    void SendRaw{genericTypeStatement}(long serviceId, int proto,{realArgsStatement} IMessageHeader header = null);
    void SendQueue0{genericTypeStatement}(long serviceId, int proto,{realArgsStatement} IMessageHeader header = null);
    void SendQueue0Raw{genericTypeStatement}(long serviceId, int proto,{realArgsStatement} IMessageHeader header = null);
";
        }

        // ==================== SecretService 显式接口实现（仅 Send，转发到 Service） ====================

        private string ForwardArgs(List<string> argsList, string extraLeadingArgs)
        {
            var list = new List<string>();
            if (!string.IsNullOrEmpty(extraLeadingArgs)) list.Add(extraLeadingArgs);
            list.AddRange(argsList);
            return string.Join(", ", list);
        }

        public string GetImplSend(List<string> argsTemplateList, List<string> argsList)
        {
            var genericTypeStatement = Helper.MakeGenericStatement(argsTemplateList);
            var realArgsStatement = Helper.MakeRealArgsStatement(argsTemplateList, argsList);
            var func = $@"
    void ISecretService.Send{genericTypeStatement}(long serviceId, int proto,{realArgsStatement} IMessageHeader header)
        => Service.Send({ForwardArgs(argsList, "serviceId, proto")}, header);

    void ISecretService.Send{genericTypeStatement}(long serviceId, int proto,{realArgsStatement} IMessageInvoker messageInvoker, long ProxyId, IMessageHeader header)
        => Service.Send({ForwardArgs(argsList, "serviceId, proto")}, messageInvoker, ProxyId, header);

    void ISecretService.SendRaw{genericTypeStatement}(long serviceId, int proto,{realArgsStatement} IMessageHeader header)
        => Service.SendRaw({ForwardArgs(argsList, "serviceId, proto")}, header);

    void ISecretService.SendQueue0{genericTypeStatement}(long serviceId, int proto,{realArgsStatement} IMessageHeader header)
        => Service.SendQueue0({ForwardArgs(argsList, "serviceId, proto")}, header);

    void ISecretService.SendQueue0Raw{genericTypeStatement}(long serviceId, int proto,{realArgsStatement} IMessageHeader header)
        => Service.SendQueue0Raw({ForwardArgs(argsList, "serviceId, proto")}, header);
";
            return func;
        }

        // ==================== Initialize：输出源文件 ====================

        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
            var serviceSendList = new List<string>();
            var serviceCallList = new List<string>();
            var interfaceSendList = new List<string>();
            var implSendList = new List<string>();

            for (int i = 0; i <= Count; i++)
            {
                var argsTemplateList = new List<string>();
                var argsList = new List<string>();
                for (int j = 0; j < i; j++)
                {
                    argsTemplateList.Add($"T{j + 1}");
                    argsList.Add($"t{j + 1}");
                }

                serviceSendList.Add(GetServiceSendFuncContent(argsTemplateList, argsList));
                serviceCallList.Add(GetServiceCallFuncContent(argsTemplateList, argsList));
                interfaceSendList.Add(GetInterfaceSendSignature(argsTemplateList, argsList));
                implSendList.Add(GetImplSend(argsTemplateList, argsList));
            }

            EmitService(context, serviceSendList);
            EmitInterface(context, interfaceSendList);
            EmitSecretService(context, implSendList);
            EmitCallResult(context, serviceCallList);
        }

        private void EmitService(IncrementalGeneratorInitializationContext context,
            List<string> sendList)
        {
            var funcBody = string.Join("\n\n", sendList);

            var msgCode = $@"

using MessagePack;
using System.Buffers;
using Proxar.ServiceCore.Interfaces;
using Proxar.Tasks;

namespace Proxar.ServiceCore;


public static partial class Service
{{

{funcBody}

}}
";
            context.RegisterPostInitializationOutput(ctx =>
                ctx.AddSource("ServiceCallfunc.g.cs", SourceText.From(msgCode, Encoding.UTF8)));
        }

        private void EmitInterface(IncrementalGeneratorInitializationContext context,
            List<string> sendList)
        {
            var sendBody = string.Join("\n", sendList);

            var code = $@"

namespace Proxar.ServiceCore.Interfaces;


public partial interface ISecretService
{{

{sendBody}

}}
";
            context.RegisterPostInitializationOutput(ctx =>
                ctx.AddSource("ISecretService.g.cs", SourceText.From(code, Encoding.UTF8)));
        }

        private void EmitSecretService(IncrementalGeneratorInitializationContext context,
            List<string> sendList)
        {
            var sendBody = string.Join("\n", sendList);

            var code = $@"


using MessagePack;
using System.Buffers;
using Proxar.Tasks;
using Proxar.ServiceCore.Interfaces;

namespace Proxar.ServiceCore;


internal partial class SecretService
{{

{sendBody}

}}
";
            context.RegisterPostInitializationOutput(ctx =>
                ctx.AddSource("SecretService.g.cs", SourceText.From(code, Encoding.UTF8)));
        }

        private void EmitCallResult(IncrementalGeneratorInitializationContext context,
            List<string> callList)
        {
            var callBody = string.Join("\n\n", callList);

            var code = $@"

using MessagePack;
using System.Buffers;
using Proxar.Tasks;

namespace Proxar.ServiceCore.Interfaces;


public partial interface ISecretService
{{
    public partial struct CallResult<TRet>
    {{

{callBody}

    }}
}}
";
            context.RegisterPostInitializationOutput(ctx =>
                ctx.AddSource("ISecretServiceCallResult.g.cs", SourceText.From(code, Encoding.UTF8)));
        }
    }
}