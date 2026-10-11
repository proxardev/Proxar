///*
// * Copyright 2026 Proxar
// * SPDX-License-Identifier: Apache-2.0
// * 
// * Licensed under the Apache License, Version 2.0 (the "License");
// * you may not use this file except in compliance with the License.
// * You may obtain a copy of the License at
// * 
// *     http://www.apache.org/licenses/LICENSE-2.0
// * 
// * Unless required by applicable law or agreed to in writing, software
// * distributed under the License is distributed on an "AS IS" BASIS,
// * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// * See the License for the specific language governing permissions and
// * limitations under the License.
// */


//using TestShared;

//[assembly: AssemblyFixture(typeof(GlobalFixture))]



using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Versioning;
using TestShared;

internal static class TestProjectSetup
{
    [ModuleInitializer]
    public static void Initialize()
    {
        TestContextHelper.TargetFrameworkMajorVersion = GetTargetFrameworkMajorVersion();
        TestContextHelper.OffsetFromMetadata = GetOffsetFromMetadata();
    }

    private static int GetTargetFrameworkMajorVersion()
    {
        var assembly = Assembly.GetExecutingAssembly();
        var attribute = assembly!.GetCustomAttribute<TargetFrameworkAttribute>();
        if (attribute == null) return 0; // fallback

        // 属性值格式如 ".NETCoreApp,Version=v9.0" 或 ".NETFramework,Version=v4.8"
        var frameworkName = attribute.FrameworkName;
        // 提取版本号
        var versionPart = frameworkName.Split('=')[1]; // "v9.0"
        var majorStr = versionPart.TrimStart('v').Split('.')[0]; // "9"
        return int.Parse(majorStr);
    }

    private static int GetOffsetFromMetadata()
    {
        var assembly = Assembly.GetExecutingAssembly();
        var metadata = assembly!.GetCustomAttributes<AssemblyMetadataAttribute>()
                               .FirstOrDefault(a => a.Key == "TestOffset");
        return metadata != null ? int.Parse(metadata.Value!) : 0;
    }

}