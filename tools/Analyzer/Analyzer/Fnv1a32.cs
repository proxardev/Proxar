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


using Proxar.Abstractions.Service;
using System;
using System.Text;

namespace Analyzer
{
    internal static class Fnv1a32
    {
        private const uint OffsetBasis = 0x811c9dc5;
        private const uint Prime = 0x01000193;

        public static uint Hash(string input)
        {
            if (input == null) throw new ArgumentNullException(nameof(input));

            uint hash = OffsetBasis;
            foreach (byte b in Encoding.UTF8.GetBytes(input))
            {
                hash ^= b;
                hash *= Prime;
            }
            return hash;
        }

        public static int HashToRange(string input)
        {
            const int min = ServiceConst.AutoProtoStart;
            const int max = ServiceConst.AutoProtoEnd;
            const long range = max - min + 1;

            uint h = Hash(input);
            return (int)(min + (h % range));
        }

        public static int HashToRange(string input, int min, int max)
        {
            var range = max - min + 1;

            uint h = Hash(input);
            return (int)(min + (h % range));
        }
    }
}