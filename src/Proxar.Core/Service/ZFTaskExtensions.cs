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


using Proxar.ServiceCore.Message;
using Proxar.Tasks;
namespace Proxar.ServiceCore;



public static class ZFTaskExtensions
{
    public static async ZFTask AutoResponse<T>(this ZFTask<T> task, long serviceId, long msgIdx)
    {
        try
        {
            var result = await task;
            var header = new MessageHeader(msgIdx);
            Service.Send(serviceId, ProtoBase.RpcCallBack, result, header: header);
        }
        catch (Exception exception)
        {
            var header = new MessageHeader(msgIdx);
            Service.Send(serviceId, ProtoBase.RpcCallbackError, exception.Message, header: header);
            throw;
        }
    }
}