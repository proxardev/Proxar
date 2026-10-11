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


namespace TestShared;

public static class TestContextHelper
{
    /// <summary>
    /// 当前测试项目的目标框架主版本号（如 8、9、10）。
    /// 由每个测试项目在加载时自动设置。
    /// </summary>
    public static int TargetFrameworkMajorVersion { get; set; } = 0;
    public static int OffsetFromMetadata { get; set; } = 0;

    public static int NewPort(int port)
    {
        return port + OffsetFromMetadata * 10 + TargetFrameworkMajorVersion;
    }
}
