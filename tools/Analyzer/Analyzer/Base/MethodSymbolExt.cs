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

    public static class MethodSymbolExt
    {
        public static Accessibility GenerateServiceProtoMethodAccessibility(this IMethodSymbol methodSymbol)
        {

            var original = methodSymbol.DeclaredAccessibility;

            return original switch
            {
                Accessibility.Private => Accessibility.Internal,
                Accessibility.ProtectedAndInternal => Accessibility.Internal,
                _ => original
            };
        }

        public static bool HasAttribute(this IMethodSymbol methodSymbol, string attribute)
        {
            var attr = methodSymbol.GetAttributes()
                .FirstOrDefault(a =>
                    a.AttributeClass?.ToDisplayString() == attribute
                );

            if (attr == null)
                return false;
            return true;
        }

        public static bool IsRequestCallBackMethod(this IMethodSymbol methodSymbol)
        {
            var attr = methodSymbol.GetAttributes()
                .FirstOrDefault(a =>
                    a.AttributeClass?.ToDisplayString() == "Proxar.ServiceCore.ServiceMethodAttribute"
                );

            if (attr == null)
                return false;
            var parameters = attr.AttributeConstructor!.Parameters;
            var args = attr.ConstructorArguments;

            for (int i = 0; i < parameters.Length; i++)
            {
                if (parameters[i].Name == "selfHandleReturn" && args[i].Value is bool value)
                {
                    return value;
                }
            }

            return false;
        }

        public static object GetMethodAttrValue(this IMethodSymbol self, string attributeClassName, string parameterName)
        {
            var attr = self.GetAttributes()
                .FirstOrDefault(a =>
                    a.AttributeClass?.ToDisplayString() == attributeClassName
                );

            if (attr == null)
                return null;
            var parameters = attr.AttributeConstructor!.Parameters;
            var args = attr.ConstructorArguments;

            for (int i = 0; i < parameters.Length; i++)
            {
                if (parameters[i].Name == parameterName)
                {
                    return args[i].Value;
                }
            }

            return null;
        }

        public static bool IsProvideRawArgsProxy(this IMethodSymbol methodSymbol)
        {
            var value = methodSymbol.GetMethodAttrValue("Proxar.ServiceCore.ServiceMethodActionAttribute", "rawArgsMethod");

            if (value == null)
                return false;
            if (value is bool valueBool)
            {
                return valueBool;
            }
            return false;
        }

        public static bool IsProvideQueue0ArgsProxy(this IMethodSymbol methodSymbol)
        {
            var value = methodSymbol.GetMethodAttrValue("Proxar.ServiceCore.ServiceMethodActionAttribute", "queue0ArgsMethod");

            if (value == null)
                return false;
            if (value is bool valueBool)
            {
                return valueBool;
            }
            return false;
        }
    }
}