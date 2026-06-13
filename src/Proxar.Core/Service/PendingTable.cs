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
using Proxar.ServiceCore.Interfaces;
using Proxar.Tasks;
namespace Proxar.ServiceCore;



internal sealed class PendingTable<T> : IPendingTable
{
    private readonly Dictionary<long, ZFTask<T>> _map = new();
    // 改为内存池。
    private readonly Queue<ZFTask<T>> _pool = new();

    public ZFTask<T> Rent()
    {
        //return new();
        return _pool.TryDequeue(out var v) ? v : new();
    }
    public void Add(long session, ZFTask<T> vts) => _map[session] = vts;

    public bool TrySet(long session, IServiceMessage msg)
    {
        var vts = _map!.Get(session);
        _map.Remove(session);
        if (vts != null)
        {
            T value = msg.DeserializeArgs<T>();
            vts.SetResult(value);
            //_pool.Enqueue(vts);
            return true;
        }
        return false;
    }
    public bool TrySetException(long session, Exception exception)
    {
        var vts = _map!.Get(session);
        _map.Remove(session);
        if (vts != null)
        {
            vts.SetException(exception);
            //_pool.Enqueue(vts);
            return true;
        }
        return false;
    }

    public ZFTask<T>? PopPendingResultTask(long session)
    {
        var ts = _map!.Get(session);
        _map.Remove(session);
        return ts;
    }

    public void SetResult(long msgSeq, IServiceMessage msg)
    {
        var task = _map!.Get(msgSeq);
        _map.Remove(msgSeq);
        if (task == null)
        {
            return;
        }
        T value = msg.DeserializeArgs<T>();
        task.SetResult(value);
    }

    public void SetException(long msgSeq, IServiceMessage msg)
    {
        var task = _map!.Get(msgSeq);
        _map.Remove(msgSeq);
        if (task == null)
        {
            return;
        }
        string errorInfo = msg.DeserializeArgs<string>();
        var exception = new Exception(errorInfo);
        task.SetException(exception);
    }
}
