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


using FluentAssertions;
using Proxar.AppHost;
using System.Text.Json;


namespace FrameUnitTest.AppOptionsLoaderTests;


public class AppOptionsLoaderTests
{
    // 辅助方法：创建临时 JSON 配置文件
    private static string CreateTempJsonConfigFile(object content)
    {
        var tempFile = Path.GetTempFileName();
        var json = JsonSerializer.Serialize(content);
        File.WriteAllText(tempFile, json);
        return tempFile;
    }

    [Fact]
    public void Load_NoConfigFile_ShouldUseCommandLineOnly()
    {
        // Arrange
        var args = new[] { "--Ip=192.168.1.100", "--Port=8080" };

        // Act
        var options = AppOptionsLoader.Load<AppOptions>(args);

        // Assert
        options.Ip.Should().Be("192.168.1.100");
        options.Port.Should().Be(8080);
    }

    [Fact]
    public void Load_NoConfigFile_WithPartialCommandLine_ShouldUseDefaultForMissing()
    {
        // Arrange
        var args = new[] { "--Port=9090" };

        // Act
        var options = AppOptionsLoader.Load<AppOptions>(args);

        // Assert
        options.Port.Should().Be(9090);
        options.Ip.Should().Be(string.Empty);        // 默认值
    }

    [Fact]
    public void Load_WithConfigFile_PartialFields_ShouldUseConfigValuesAndDefaults()
    {
        // Arrange
        var configContent = new { Ip = "10.0.0.1" }; // 只指定 Ip
        var configFile = CreateTempJsonConfigFile(configContent);
        var args = new[] { $"--ConfigFile={configFile}" };

        try
        {
            // Act
            var options = AppOptionsLoader.Load<AppOptions>(args);

            // Assert
            options.Ip.Should().Be("10.0.0.1");
            options.Port.Should().Be(0);              // 默认值
        }
        finally
        {
            File.Delete(configFile);
        }
    }

    [Fact]
    public void Load_WithConfigFile_AllFields_ShouldUseAllConfigValues()
    {
        // Arrange
        var configContent = new { Ip = "192.168.1.1", Port = 1234 };
        var configFile = CreateTempJsonConfigFile(configContent);
        var args = new[] { $"--ConfigFile={configFile}" };

        try
        {
            // Act
            var options = AppOptionsLoader.Load<AppOptions>(args);

            // Assert
            options.Ip.Should().Be("192.168.1.1");
            options.Port.Should().Be(1234);
        }
        finally
        {
            File.Delete(configFile);
        }
    }

    [Fact]
    public void Load_WithConfigFileAndCommandLine_CommandLineShouldOverrideConflictingFields()
    {
        // Arrange
        // 配置文件：Ip 和 Port，命令行：Port（且 Port 与配置文件冲突）
        var configContent = new { Ip = "10.0.0.10", Port = 5000 };
        var configFile = CreateTempJsonConfigFile(configContent);
        var args = new[]
        {
        $"--ConfigFile={configFile}",
        "--Port=8080",           // 覆盖配置文件中的 Port
    };

        try
        {
            // Act
            var options = AppOptionsLoader.Load<AppOptions>(args);

            // Assert
            options.Ip.Should().Be("10.0.0.10");   // 仅配置文件有，按配置文件生效
            options.Port.Should().Be(8080);        // 命令行覆盖
        }
        finally
        {
            File.Delete(configFile);
        }
    }

    [Fact]
    public void Load_WithConfigFileAndCommandLine_CommandLineOnlyPartiallyOverride_ShouldMergeCorrectly()
    {
        // Arrange
        var configContent = new { Ip = "172.16.0.1", Port = 3000 };
        var configFile = CreateTempJsonConfigFile(configContent);
        var args = new[]
        {
        $"--ConfigFile={configFile}",
        "--Port=4000"      // 只覆盖 Port，Ip 保留配置文件的值
    };

        try
        {
            // Act
            var options = AppOptionsLoader.Load<AppOptions>(args);

            // Assert
            options.Ip.Should().Be("172.16.0.1");
            options.Port.Should().Be(4000);
        }
        finally
        {
            File.Delete(configFile);
        }
    }
}