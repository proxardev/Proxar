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


using Proxar.ServiceCore.Interfaces;
using Proxar.ServiceCore.Message;
using System.Net.Sockets;
using System.Runtime.CompilerServices;

namespace Proxar.ServiceCore;

public static partial class Service
{
    public static IMessageInvoker MessageInvoker => ActorThreadScope.ServiceGroup.Invoker;

    internal static IInternalMessageInvoker InternalMessageInvoker => ActorThreadScope.ServiceGroup.InternalInvoker;


    internal static bool IsNeedSerialize<T>()
    {
        if (!RuntimeHelpers.IsReferenceOrContainsReferences<T>())
        {
            return false;
        }
        if (typeof(T) == typeof(string))
        {
            return false;
        }
        return true;
    }

    internal static TResult TypeConvert<TResult, TArgs>(TArgs args)
    {
        try
        {
            return (TResult)Convert.ChangeType(args, typeof(TResult))!;
        }
        catch (Exception)
        {
            return (TResult)(object)args!;
        }
    }

    /// <summary>
    /// 1. 数值类型之间，可以尽可能转换
    /// 2. 可空数值类型之间，可以尽可能转换
    /// 3. 可空数值类型与数值类型之间，可以尽可能转换
    /// 4. 同一struct,与可空类型，可以尽可能转换
    /// 5. 派生类型转接口、基类，可以尽可能转换
    /// 
    /// 6. 数值类型与其他类型之间，不满足1-5点，不可转换
    /// 7. 其他类型相互之间，不满足1-5点，转换失败。
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
    /// <typeparam name="TArgs"></typeparam>
    /// <param name="args"></param>
    /// <returns></returns>
    public static TResult TypeConvert2<TResult, TArgs>(TArgs args)
    {
        try
        {
            if (args is TResult result)
            {
                return result;
            }
            return (TResult)Convert.ChangeType(args, typeof(TResult))!;
        }
        catch (Exception)
        {
            return (TResult)(object)args!;
        }
    }

    public static void Send(long serviceId, int proto, Socket socket)
    {
        var msg = new MessageSocket(socket);
        msg.Init();

        var service = ServiceManager.Instance.GetService(serviceId);
        service!.PushMessage(msg);
    }

    internal static void Send(long serviceId, int proto, ServiceBootstrapper? bootloaderInfo)
    {
        var msg = new Message_ServiceStartUp(bootloaderInfo);
        msg.SetHeadData(0, serviceId, 0, ProtoBase.ServiceStart);
        var service = ServiceManager.Instance.GetService(serviceId);
        service!.PushMessage(msg);
    }

}