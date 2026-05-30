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


namespace Proxar.Network;

public interface IChannel
{
    string StrId { get; set; }

    long Id { get; set; }

    bool IsConnected { get; }
    string GetRemoteIp();
    int GetRemotePort();
    void Send(INetMessage message);
    void Close(string reason);

    event Action<IChannel, ReadOnlyMemory<byte>> MessageReceived;
    event Action<IChannel, Exception> ErrorOccurred;
    event Action<IChannel, string> Disconnected;
}