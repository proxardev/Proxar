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


using Microsoft.CodeAnalysis;
using System.Linq;

namespace Analyzer
{

    public static class NamedTypeSymbolExt
    {

        public static bool TryGetSymbolAttrValue<T>(this INamedTypeSymbol self, string attributeClassName, string parameterName, out T value)
        {
            var attr = self.GetAttributes()
                .FirstOrDefault(a =>
                    a.AttributeClass?.ToDisplayString() == attributeClassName
                );
            value = default;
            if (attr == null)
                return false;
            var parameters = attr.AttributeConstructor!.Parameters;
            var args = attr.ConstructorArguments;

            for (int i = 0; i < parameters.Length; i++)
            {
                if (parameters[i].Name == parameterName)
                {
                    value = (T)args[i].Value;
                    return true;
                }
            }

            return true;
        }

    }
}