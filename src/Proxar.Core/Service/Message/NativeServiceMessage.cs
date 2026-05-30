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


using MessagePack;
using System.Buffers;
namespace Proxar.ServiceCore.Message;



internal abstract class NativeServiceMessage : AbstractServiceMessage
{
    public override MessagePackReader GetMessagePackReader()
    {
        throw new NotImplementedException();
    }

    public override IBufferWriter<byte> GetSerializeArgeWriter()
    {
        throw new NotImplementedException();
    }

    public override void SerializeArgs(IBufferWriter<byte> writer)
    {
        throw new NotImplementedException();
    }

    public override ReadOnlyMemory<byte> GetPayloadReadOnlyMemory()
    {
        throw new NotImplementedException();
    }
}