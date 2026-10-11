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

namespace Proxar.Abstractions.Service
{
    internal static class ServiceConst
    {
        public const int MinReservedProto = 900;
        public const int MaxReservedProto = 999;
        public const int StartProto = 1;
        public const int AppintProtoStart = StartProto;
        public const int AppintProtoEnd = MinReservedProto - 1;
        public const int AutoProtoStart = 1 << 10;
        public const int AutoProtoEnd = 1 << 17;

        public const int AutoProxyIdStart = 1 << 8;
        public const int AutoProxyIdEnd = 1 << 13;
        public const int MinProxyId = 1;
        public const int MaxProxyId = AutoProxyIdEnd;
        public const int AppintProxyIdStart = MinProxyId;
        public const int AppintProxyIdEnd = AutoProxyIdStart - 1;

        public const int MinKnownServiceId = 1;
        public const int MaxKnownServiceId = AutoKnownServiceIdEnd;
        public const int AutoKnownServiceIdStart = 1 << 9;
        public const int AutoKnownServiceIdEnd = 1 << 16;
        public const int AppintKnownServiceIdStart = MinKnownServiceId;
        public const int AppintKnownServiceIdEnd = AutoKnownServiceIdStart - 1;
    }

}
