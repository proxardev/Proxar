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



public interface IServiceMessage : IDisposable
{

    long GetToServiceId();
    long GetFromServiceId();
    long GetSeq();
    int GetProto();
    void Serialize();

    ReadOnlyMemory<byte> GetPayloadReadOnlyMemory();

    public T ReadHead<T>();

    public T DeserializeArgs<T>();
    public (T1, T2) DeserializeArgs<T1, T2>();
    public (T1, T2, T3) DeserializeArgs<T1, T2, T3>();
    public (T1, T2, T3, T4) DeserializeArgs<T1, T2, T3, T4>();
    public (T1, T2, T3, T4, T5) DeserializeArgs<T1, T2, T3, T4, T5>();
    public (T1, T2, T3, T4, T5, T6) DeserializeArgs<T1, T2, T3, T4, T5, T6>();
    public (T1, T2, T3, T4, T5, T6, T7) DeserializeArgs<T1, T2, T3, T4, T5, T6, T7>();
    public (T1, T2, T3, T4, T5, T6, T7, T8) DeserializeArgs<T1, T2, T3, T4, T5, T6, T7, T8>();
    public (T1, T2, T3, T4, T5, T6, T7, T8, T9) DeserializeArgs<T1, T2, T3, T4, T5, T6, T7, T8, T9>();
}