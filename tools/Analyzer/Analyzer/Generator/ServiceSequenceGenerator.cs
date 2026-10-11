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


using Analyzer;
using System.Collections.Generic;
using System.Text;

public static partial class ServiceSequenceGenerator
{

    /// <summary>
    /// 生成 KnownServiceInitializer.g.cs
    /// 该方法会扫描所有标记了 [KnownService] 的类，
    /// 生成一个带 [ModuleInitializer] 的静态初始化类，
    /// 在模块加载时自动触发所有知名服务类型的静态构造函数，
    /// 完成 ServiceIdRegistry.Register 注册。
    /// </summary>
    public static string GenerateKnownServiceInitializer(string assemblyName, List<ServiceInfo> serviceInfos)
    {
        // 1. 筛选出标记了 [KnownService] 的类
        var knownServices = new List<(string ClassName, int Sequence)>();

        foreach (var serviceInfo in serviceInfos)
        {
            if (ServiceInfo.TryGetKnownServiceSequence(serviceInfo.ServiceClassSymbol, out var sequence))
            {
                knownServices.Add((serviceInfo.GetFullClassName(), sequence));
            }
        }
        knownServices.Sort((a, b) => a.Sequence.CompareTo(b.Sequence));

        // 2. 如果没有知名服务，返回空字符串
        if (knownServices.Count == 0)
            return $"// {knownServices.Count} service";

        // 3. 生成静态初始化类
        var sb = new StringBuilder();
        sb.AppendLine("using System.Runtime.CompilerServices;");
        sb.AppendLine("using Proxar.ServiceCore;");
        sb.AppendLine($"// {knownServices.Count} service");
        sb.AppendLine();
        sb.AppendLine("namespace Proxar.ServiceCore.Generated");
        sb.AppendLine("{");
        sb.AppendLine($"    internal static class KnownServiceInitializer_{assemblyName}");
        sb.AppendLine("    {");
        sb.AppendLine("        [ModuleInitializer]");
        sb.AppendLine("        public static void Initialize()");
        sb.AppendLine("        {");

        sb.AppendLine($"             var resolver = ServiceSequenceResolver.Instance;");
        foreach (var (className, sequence) in knownServices)
        {
            //sb.AppendLine($"            RuntimeHelpers.RunClassConstructor(typeof({className}).TypeHandle);");

            sb.AppendLine($"             resolver.Register<{className}>({sequence});");
        }


        sb.AppendLine("        }");
        sb.AppendLine("    }");
        sb.AppendLine("}");

        return sb.ToString();
    }
}