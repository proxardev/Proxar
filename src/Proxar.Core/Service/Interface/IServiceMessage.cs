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


namespace Proxar.ServiceCore.Interfaces;


/// <summary>
/// 定义服务间通信消息的核心接口，提供对消息头、负载的访问以及序列化、反序列化能力。
/// 所有通过 Proxar 框架传输的消息都必须实现此接口。
/// </summary>
public interface IServiceMessage : IDisposable
{
    /// <summary>
    /// 获取消息的目标服务 ID。
    /// </summary>
    /// <returns>目标服务的唯一标识符。</returns>
    long GetToServiceId();

    /// <summary>
    /// 获取消息的源服务 ID。
    /// </summary>
    /// <returns>发送此消息的服务的唯一标识符。</returns>
    long GetFromServiceId();

    /// <summary>
    /// 获取消息的序列号（MsgSeq），用于请求-响应匹配。
    /// </summary>
    /// <returns>消息的序列号。</returns>
    long GetSeq();

    /// <summary>
    /// 获取消息的协议方法编号（Proto）。
    /// </summary>
    /// <returns>协议方法编号。</returns>
    int GetProto();

    /// <summary>
    /// 将消息的负载（Args）序列化，以便通过网络发送。
    /// </summary>
    /// <remarks>
    /// 需要自行存储序列化后的字节数组
    /// </remarks>
    void Serialize();

    /// <summary>
    /// 获取消息负载的只读字节序列。
    /// </summary>
    /// <returns>消息负载的 <see cref="ReadOnlyMemory{Byte}"/> 表示。</returns>
    ReadOnlyMemory<byte> GetPayloadReadOnlyMemory();

    /// <summary>
    /// 从消息头中读取指定类型的数据。
    /// </summary>
    /// <typeparam name="T">要从头部读取的数据类型。</typeparam>
    /// <returns>头部数据的实例。</returns>
    public T ReadHead<T>();

    /// <summary>
    /// 将消息负载反序列化为单个指定类型的对象。
    /// </summary>
    /// <typeparam name="T">要反序列化的目标类型。</typeparam>
    /// <returns>反序列化后的对象实例。</returns>
    public T DeserializeArgs<T>();

    /// <summary>
    /// 将消息负载反序列化为包含两个指定类型元素的元组。
    /// </summary>
    /// <typeparam name="T1">元组第一个元素的类型。</typeparam>
    /// <typeparam name="T2">元组第二个元素的类型。</typeparam>
    /// <returns>包含两个反序列化对象的元组。</returns>
    public (T1, T2) DeserializeArgs<T1, T2>();

    /// <summary>
    /// 将消息负载反序列化为包含三个指定类型元素的元组。
    /// </summary>
    /// <typeparam name="T1">元组第一个元素的类型。</typeparam>
    /// <typeparam name="T2">元组第二个元素的类型。</typeparam>
    /// <typeparam name="T3">元组第三个元素的类型。</typeparam>
    /// <returns>包含三个反序列化对象的元组。</returns>
    public (T1, T2, T3) DeserializeArgs<T1, T2, T3>();

    /// <summary>
    /// 将消息负载反序列化为包含四个指定类型元素的元组。
    /// </summary>
    /// <typeparam name="T1">元组第一个元素的类型。</typeparam>
    /// <typeparam name="T2">元组第二个元素的类型。</typeparam>
    /// <typeparam name="T3">元组第三个元素的类型。</typeparam>
    /// <typeparam name="T4">元组第四个元素的类型。</typeparam>
    /// <returns>包含四个反序列化对象的元组。</returns>
    public (T1, T2, T3, T4) DeserializeArgs<T1, T2, T3, T4>();

    /// <summary>
    /// 将消息负载反序列化为包含五个指定类型元素的元组。
    /// </summary>
    /// <typeparam name="T1">元组第一个元素的类型。</typeparam>
    /// <typeparam name="T2">元组第二个元素的类型。</typeparam>
    /// <typeparam name="T3">元组第三个元素的类型。</typeparam>
    /// <typeparam name="T4">元组第四个元素的类型。</typeparam>
    /// <typeparam name="T5">元组第五个元素的类型。</typeparam>
    /// <returns>包含五个反序列化对象的元组。</returns>
    public (T1, T2, T3, T4, T5) DeserializeArgs<T1, T2, T3, T4, T5>();

    /// <summary>
    /// 将消息负载反序列化为包含六个指定类型元素的元组。
    /// </summary>
    /// <typeparam name="T1">元组第一个元素的类型。</typeparam>
    /// <typeparam name="T2">元组第二个元素的类型。</typeparam>
    /// <typeparam name="T3">元组第三个元素的类型。</typeparam>
    /// <typeparam name="T4">元组第四个元素的类型。</typeparam>
    /// <typeparam name="T5">元组第五个元素的类型。</typeparam>
    /// <typeparam name="T6">元组第六个元素的类型。</typeparam>
    /// <returns>包含六个反序列化对象的元组。</returns>
    public (T1, T2, T3, T4, T5, T6) DeserializeArgs<T1, T2, T3, T4, T5, T6>();

    /// <summary>
    /// 将消息负载反序列化为包含七个指定类型元素的元组。
    /// </summary>
    /// <typeparam name="T1">元组第一个元素的类型。</typeparam>
    /// <typeparam name="T2">元组第二个元素的类型。</typeparam>
    /// <typeparam name="T3">元组第三个元素的类型。</typeparam>
    /// <typeparam name="T4">元组第四个元素的类型。</typeparam>
    /// <typeparam name="T5">元组第五个元素的类型。</typeparam>
    /// <typeparam name="T6">元组第六个元素的类型。</typeparam>
    /// <typeparam name="T7">元组第七个元素的类型。</typeparam>
    /// <returns>包含七个反序列化对象的元组。</returns>
    public (T1, T2, T3, T4, T5, T6, T7) DeserializeArgs<T1, T2, T3, T4, T5, T6, T7>();

    /// <summary>
    /// 将消息负载反序列化为包含八个指定类型元素的元组。
    /// </summary>
    /// <typeparam name="T1">元组第一个元素的类型。</typeparam>
    /// <typeparam name="T2">元组第二个元素的类型。</typeparam>
    /// <typeparam name="T3">元组第三个元素的类型。</typeparam>
    /// <typeparam name="T4">元组第四个元素的类型。</typeparam>
    /// <typeparam name="T5">元组第五个元素的类型。</typeparam>
    /// <typeparam name="T6">元组第六个元素的类型。</typeparam>
    /// <typeparam name="T7">元组第七个元素的类型。</typeparam>
    /// <typeparam name="T8">元组第八个元素的类型。</typeparam>
    /// <returns>包含八个反序列化对象的元组。</returns>
    public (T1, T2, T3, T4, T5, T6, T7, T8) DeserializeArgs<T1, T2, T3, T4, T5, T6, T7, T8>();

    /// <summary>
    /// 将消息负载反序列化为包含九个指定类型元素的元组。
    /// </summary>
    /// <typeparam name="T1">元组第一个元素的类型。</typeparam>
    /// <typeparam name="T2">元组第二个元素的类型。</typeparam>
    /// <typeparam name="T3">元组第三个元素的类型。</typeparam>
    /// <typeparam name="T4">元组第四个元素的类型。</typeparam>
    /// <typeparam name="T5">元组第五个元素的类型。</typeparam>
    /// <typeparam name="T6">元组第六个元素的类型。</typeparam>
    /// <typeparam name="T7">元组第七个元素的类型。</typeparam>
    /// <typeparam name="T8">元组第八个元素的类型。</typeparam>
    /// <typeparam name="T9">元组第九个元素的类型。</typeparam>
    /// <returns>包含九个反序列化对象的元组。</returns>
    public (T1, T2, T3, T4, T5, T6, T7, T8, T9) DeserializeArgs<T1, T2, T3, T4, T5, T6, T7, T8, T9>();
}