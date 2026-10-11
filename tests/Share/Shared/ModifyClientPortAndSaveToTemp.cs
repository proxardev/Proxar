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


using System.Text.Json;
using System.Text.Json.Nodes;
namespace TestShared;

public static class ModifyClientPortAndSaveToTemp
{
    private static string MakePort(JsonNode jsonNode)
    {
        var port = int.Parse(jsonNode!.GetValue<string>());
        var newPort = TestContextHelper.NewPort(port);
        return newPort.ToString();
    }

    /// <summary>
    /// 读取 Config.json，修改 ClientPort 值，写入临时文件并返回临时文件名
    /// </summary>
    public static TempFile Handle(string configPath)
    {
        // 1. 读取并反序列化为 JsonNode
        string jsonContent = File.ReadAllText(configPath);
        JsonNode rootNode = JsonNode.Parse(jsonContent)!;
        // 2. 修改 ClientPort
        rootNode["ClientPort"] = MakePort(rootNode["ClientPort"]!);
        rootNode["Port"] = MakePort(rootNode["Port"]!);

        var tempFile = new TempFile(".tmp.json");
        // 3. 创建临时文件并写入
        string tempFilePath = tempFile.FilePath;

        using StreamWriter writer = new StreamWriter(tempFilePath);
        JsonWriterOptions options = new JsonWriterOptions { Indented = true };
        using Utf8JsonWriter jsonWriter = new Utf8JsonWriter(writer.BaseStream, options);
        rootNode.WriteTo(jsonWriter);


        return tempFile;
    }

}
public class TempFile : IDisposable
{
    public string FilePath { get; }

    public TempFile(string extension = ".tmp")
    {
        FilePath = Path.Combine(Directory.GetCurrentDirectory(), $"temp_{Guid.NewGuid()}{extension}");
        // 创建空文件
        File.Create(FilePath).Close();
    }

    public void Dispose()
    {
        if (File.Exists(FilePath))
        {
            File.Delete(FilePath);
        }
    }
}