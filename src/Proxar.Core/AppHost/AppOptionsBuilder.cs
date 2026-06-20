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


namespace Proxar.AppHost;



/// <summary>
/// 用于加载和绑定 <see cref="AppOptions"/> 的构造器。支持业务层扩展自定义配置类型。
/// </summary>
/// <typeparam name="T">配置类型，必须继承 <see cref="AppOptions"/> 并具有无参数构造函数。</typeparam>
public class AppOptionsBuilder<T> where T : AppOptions, new()
{
    internal T Options { get; private set; } = null!;

    /// <summary>
    /// （内部使用）从命令行参数加载并绑定配置。
    /// </summary>
    /// <param name="args">命令行参数数组。</param>
    internal void Load(string[] args)
    {
        Options = AppOptionsLoader.Load<T>(args);
    }
}