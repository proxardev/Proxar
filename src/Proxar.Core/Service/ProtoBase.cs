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


namespace Proxar.ServiceCore;

public static class ProtoBase
{
    public const int BaseProtoStart = 900;

    public const int ContextAction = 900;// 上下文回调

    public const int RpcCallBack = 901; // Rpc回调

    public const int ReceiveSocket = 902;

    public const int ReceiveOuterSocket = 903;

    public const int ReceiveChannel = 904;

    public const int ServiceStart = 905;

    public const int ServiceClose = 906;

    public const int EchoConfirm = 907;

    public const int ServiceClose2 = 908;

    public const int RpcCallbackError = 909;
}