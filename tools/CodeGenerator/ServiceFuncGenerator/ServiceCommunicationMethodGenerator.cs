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


using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System.Text;

namespace ZFSourceGenerator
{

    // 生成call和send函数
    [Generator(LanguageNames.CSharp)]
    public class ServiceCommunicationMethodGenerator : IIncrementalGenerator
    {
        /*
         * 
         * call和send函数生成1-9个参数重载版本
         * 
         */

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

        public string GetSendFuncContent(List<string> argsTemplateList, List<string> argsList)
        {
            var genericTypeStatement = Helper.MakeGenericStatement(argsTemplateList);
            var realArgsStatement = Helper.MakeRealArgsStatement(argsTemplateList, argsList);
            var func = $@"
    public static void Send{genericTypeStatement}(long serviceId, int proto,{realArgsStatement} IMessageHeader header = null)
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

        public string GetCallFuncContent(List<string> argsTemplateList, List<string> argsList)
        {
            var resultType = "TRet";
            var genericTypeStatement = Helper.MakeGenericStatement(argsTemplateList.Prepend(resultType).ToList());
            var realArgsStatement = Helper.MakeRealArgsStatement(argsTemplateList, argsList);


            var func = $@"
    public static ZFTask<{resultType}> Call{genericTypeStatement}(this CallResult<{resultType}> _,
        ServiceBase fromService, long serviceId, int proto,{realArgsStatement} IMessageHeader header = null)
    {{
        var fromServiceId = fromService.GetServiceId();
        var msgSeq = fromService.NewMessageSeq();
        var ts = fromService.SetPendingTable<{resultType}>(msgSeq);

{this.MakeMessageHandleBody(argsTemplateList, argsList, fromServiceId: "fromServiceId", msgSeq: "msgSeq")}

        return ts;
    }}

    public static ZFTask<{resultType}> Call{genericTypeStatement}(this CallResult<{resultType}> _,
        long serviceId, int proto, {realArgsStatement} IMessageHeader header = null)
    {{
        var fromService = ActorThreadScope.Service;
        var fromServiceId = fromService.GetServiceId();
        var msgSeq = fromService.NewMessageSeq();
        var ts = fromService.SetPendingTable<{resultType}>(msgSeq);

{this.MakeMessageHandleBody(argsTemplateList, argsList, fromServiceId: "fromServiceId", msgSeq: "msgSeq")}
        return ts;
    }}

    public static ZFTask<{resultType}> Call{genericTypeStatement}(this CallResult<{resultType}> _,
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

    internal static ZFTask<{resultType}> CallRaw{genericTypeStatement}(this CallResult<{resultType}> _,
        long serviceId, int proto, {realArgsStatement} IMessageHeader header = null)
    {{
        var fromService = ActorThreadScope.Service;
        var fromServiceId = fromService.GetServiceId();
        var msgSeq = fromService.NewMessageSeq();
        var ts = fromService.SetPendingTable<{resultType}>(msgSeq);

{this.MakeMessageHandleBody(argsTemplateList, argsList, checkSerialize: false, pushMsgFunc: "SendRaw", fromServiceId: "fromServiceId", msgSeq: "msgSeq", messageInvoker: "Service.InternalMessageInvoker")}
        return ts;
    }}

    internal static ZFTask<{resultType}> CallQueue0{genericTypeStatement}(this CallResult<{resultType}> _,
        long serviceId, int proto, {realArgsStatement} IMessageHeader header = null)
    {{
        var fromService = ActorThreadScope.Service;
        var fromServiceId = fromService.GetServiceId();
        var msgSeq = fromService.NewMessageSeq();
        var ts = fromService.SetPendingTable<{resultType}>(msgSeq);

{this.MakeMessageHandleBody(argsTemplateList, argsList, checkSerialize: false, pushMsgFunc: "PushQueue0Message", fromServiceId: "fromServiceId", msgSeq: "msgSeq", messageInvoker: "Service.InternalMessageInvoker")}
        return ts;
    }}

    internal static ZFTask<{resultType}> CallQueue0Raw{genericTypeStatement}(this CallResult<{resultType}> _,
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


        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
            int count = 9;
            var argsTemplateList = new List<string>();
            var argsList = new List<string>();
            var argsTypeList = new List<string>();
            for (int i = 0; i < count + 1; i++)
            {
                argsTemplateList.Add($"T{i}");
                argsList.Add($"t{i}");
            }
            var funcList = new List<string>();
            var callFuncList = new List<string>();
            for (int i = 0; i < count + 1; i++)
            {
                argsTemplateList.Clear();
                argsList.Clear();
                for (int j = 0; j < i; j++)
                {
                    argsTemplateList.Add($"T{j + 1}");
                    argsList.Add($"t{j + 1}");
                }
                var funcStr = this.GetSendFuncContent(argsTemplateList, argsList);
                funcList.Add(funcStr);

                callFuncList.Add(this.GetCallFuncContent(argsTemplateList, argsList));
            }

            var funcBody = string.Join("\n\n", funcList);
            var callFuncBody = string.Join("\n\n", callFuncList);

            var msgCode = @$"

using MessagePack;
using System.Buffers;
using Proxar.ServiceCore.Interfaces;
using Proxar.Tasks;

namespace Proxar.ServiceCore;




public static partial class Service
{{

{funcBody}

{callFuncBody}

}}
";

            context.RegisterPostInitializationOutput(ctx =>
                ctx.AddSource($"ServiceCallfunc.g.cs", SourceText.From(msgCode, Encoding.UTF8)));

        }
    }
}