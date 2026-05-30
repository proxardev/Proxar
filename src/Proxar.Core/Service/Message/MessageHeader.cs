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

namespace Proxar.ServiceCore.Message;


public class MessageHeader : IMessageHeader
{
    private long[] _data;


    public MessageHeader(long data)
    {
        _data = new long[] { data };
    }
    public MessageHeader(long data, long data2)
    {
        _data = new long[] { data, data2 };
    }

    public MessageHeader(long data, long data2, long data3)
    {
        _data = new long[] { data, data2, data3 };
    }

    public MessageHeader(long data, long data2, long data3, long data4)
    {
        _data = new long[] { data, data2, data3, data4 };
    }

    public ReadOnlySpan<long> GetHeaderReadOnlySpan()
    {
        return _data.AsSpan().Slice(0, _data.Length);
    }
}