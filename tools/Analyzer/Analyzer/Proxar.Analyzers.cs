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


namespace Analyzers
{


    /// <summary>
    ///   一个强类型的资源类，用于查找本地化的字符串等。
    /// </summary>
    // 此类是由 StronglyTypedResourceBuilder
    // 类通过类似于 ResGen 或 Visual Studio 的工具自动生成的。
    // 若要添加或移除成员，请编辑 .ResX 文件，然后重新运行 ResGen
    // (以 /str 作为命令选项)，或重新生成 VS 项目。
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "18.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Service_Analyzers_Generators
    {

        private static global::System.Resources.ResourceManager resourceMan;

        private static global::System.Globalization.CultureInfo resourceCulture;

        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Service_Analyzers_Generators()
        {
        }

        /// <summary>
        ///   返回此类使用的缓存的 ResourceManager 实例。
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager
        {
            get
            {
                if (object.ReferenceEquals(resourceMan, null))
                {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Analyzers.Proxar.Analyzers", typeof(Service_Analyzers_Generators).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }

        /// <summary>
        ///   重写当前线程的 CurrentUICulture 属性，对
        ///   使用此强类型资源类的所有资源查找执行重写。
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture
        {
            get
            {
                return resourceCulture;
            }
            set
            {
                resourceCulture = value;
            }
        }

        /// <summary>
        ///   查找类似 Type names should be all uppercase. 的本地化字符串。
        /// </summary>
        internal static string AnalyzerDescription
        {
            get
            {
                return ResourceManager.GetString("AnalyzerDescription", resourceCulture);
            }
        }

        /// <summary>
        ///   查找类似 Type name &apos;{0}&apos; contains lowercase letters 的本地化字符串。
        /// </summary>
        internal static string AnalyzerMessageFormat
        {
            get
            {
                return ResourceManager.GetString("AnalyzerMessageFormat", resourceCulture);
            }
        }

        /// <summary>
        ///   查找类似 Type name contains lowercase letters 的本地化字符串。
        /// </summary>
        internal static string AnalyzerTitle
        {
            get
            {
                return ResourceManager.GetString("AnalyzerTitle", resourceCulture);
            }
        }

        /// <summary>
        ///   查找类似 A service call args type need met service method args type 的本地化字符串。
        /// </summary>
        internal static string ServiceCallFormat
        {
            get
            {
                return ResourceManager.GetString("ServiceCallFormat", resourceCulture);
            }
        }
    }
}