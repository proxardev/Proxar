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

#pragma warning disable CS1591

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System.Text;

namespace ZFSourceGenerator
{
    [Generator(LanguageNames.CSharp)]
    public class UniversaltServiceMessageGenerator : IIncrementalGenerator
    {

        public string MakeDeserializeFunc(List<string> templateTypes, List<string> parameters)

        {
            var argsCnt = templateTypes.Count;
            if (argsCnt == 0)
            {
                return string.Empty;
            }
            var deserializeGenericTypes = Enumerable.Range(0, templateTypes.Count)
                .Select(x => $"TArgs{x + 1}")
                .ToList();
            var deserializeResultTypes = deserializeGenericTypes.First();
            if (argsCnt > 1)
            {
                deserializeResultTypes = $"({string.Join(", ", deserializeGenericTypes)})";
            }
            var deserializeGenericTypesStatement = Helper.MakeGenericStatement(deserializeGenericTypes);

            var convertEnum = Enumerable.Range(0, templateTypes.Count)
                .Select(i => $"Service.TypeConvert<{deserializeGenericTypes[i]}, {templateTypes[i]}>({parameters[i]})!")
                .ToList();
            var convertTypeStatement = string.Join(",\n                     ", convertEnum);

            var typeMatchResults = Enumerable.Range(0, templateTypes.Count)
                .Select(x => $"x{x + 1}")
                .ToList();
            var typeMatchStatements = new List<string>();
            // is匹配
            for (int i = 0; i < templateTypes.Count; i++)
            {
                typeMatchStatements.Add($"{parameters[i]} is {deserializeGenericTypes[i]} {typeMatchResults[i]}");
            }
            var typeMatchStatement = string.Join("\n            && ", typeMatchStatements);
            var typeMatchResult = typeMatchResults.First();
            if (argsCnt > 1)
            {
                typeMatchResult = $"({string.Join(", ", typeMatchResults)})";
            }


            var deserializerFunc = $@"

    public override {deserializeResultTypes} DeserializeArgs{deserializeGenericTypesStatement}()
    {{
        if (!isSerializeArgs)
        {{
            return ArgsReturn{deserializeGenericTypesStatement}();
        }}
        return base.DeserializeArgs{deserializeGenericTypesStatement}();
    }}

    private {deserializeResultTypes} ArgsReturn{deserializeGenericTypesStatement}()
    {{
        if ({typeMatchStatement})
        {{
            return {typeMatchResult};
        }}
        var result = ({convertTypeStatement});
        return result;
    }}

";
            return deserializerFunc;
        }


        public string MakeUniversaltServiceMessageContent(
            List<string> templateTypes,
            List<string> parameters,
            string fieldDeclaration,


            List<string> deserializerArgsTemplateList,
            List<string> deserializerArgs)
        {
            var genericTypeStatement = Helper.MakeGenericStatement(templateTypes);
            var serializerString = "";
            var fieldArgs = "";
            var setArgsFuncArgs = "";
            var setArgsFuncBody = "";


            var deserializerArgsTemplate = "";
            var deserializerArgsTemplate2 = "";

            var ifStatement = "";
            var notDeserializerResult = "";
            var forceConvertResult = "";

            var deserializerFunc = "";

            if (templateTypes.Count != 0)
            {
                var stringList = new List<string>();
                var tmp = string.Join(", ", templateTypes);


                fieldArgs = string.Join(", ", parameters);
                serializerString = $"MessagePackSerializer.Serialize(writer, ({fieldArgs}));";

                var stringEnumerable = Enumerable.Range(0, templateTypes.Count)
                    .Select(idx => $"{templateTypes[idx]} {parameters[idx]}");
                setArgsFuncArgs = string.Join(", ", stringEnumerable);

                stringEnumerable = Enumerable.Range(0, templateTypes.Count)
                    .Select(idx => $"\t\tthis.{parameters[idx]} = t{idx + 1};");
                setArgsFuncBody = string.Join("\n", stringEnumerable);



                //

                tmp = string.Join(", ", deserializerArgsTemplateList);
                deserializerArgsTemplate = $"<{tmp}>";

                deserializerArgsTemplate2 = tmp;
                if (deserializerArgsTemplateList.Count != 1)
                {
                    deserializerArgsTemplate2 = $"({tmp})";
                }

                var notDeserializerResultList = Enumerable.Range(0, deserializerArgsTemplateList.Count)
                    .Select(idx => $"x{idx + 1}")
                    .ToList();
                var stringList2 = new List<string>();
                stringList.Clear();
                stringList2.Clear();
                for (int i = 0; i < deserializerArgsTemplateList.Count; i++)
                {
                    stringList.Add($"{parameters[i]} is {deserializerArgsTemplateList[i]} {notDeserializerResultList[i]}");
                    //stringList2.Add($"({deserializerArgsTemplateList[i]})(object){fieldList[i]}");
                    //stringList2.Add($"({deserializerArgsTemplateList[i]})(object){parameters[i]}!");
                    stringList2.Add($"Service.TypeConvert<{deserializerArgsTemplateList[i]}, {templateTypes[i]}>({parameters[i]})!");
                }
                ifStatement = string.Join(" && ", stringList);

                forceConvertResult = string.Join(",", stringList2);
                notDeserializerResult = string.Join(", ", notDeserializerResultList);
                if (deserializerArgsTemplateList.Count != 1)
                {
                    notDeserializerResult = $"({notDeserializerResult})";
                    forceConvertResult = $"({forceConvertResult})";
                }



            }
            deserializerFunc = MakeDeserializeFunc(templateTypes, parameters);
            var MessageClass = $@"
internal class UniversaltServiceMessage{genericTypeStatement} : PayloadServiceMessage
{{
{fieldDeclaration}

    public void SetArgs({setArgsFuncArgs})
    {{
{setArgsFuncBody}
    }}

    public override void SerializeArgs(IBufferWriter<byte> writer)
    {{
        this.SetIsSerializeArgs();
        {serializerString}
    }}

{deserializerFunc}
}}
";
            return MessageClass;
        }


        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
            int count = 9;
            var fieldTemplateList = new List<string>() { "" };
            var fieldList = new List<string>() { "" };
            var argsTemplateList = new List<string>() { "" };
            var argsList = new List<string>() { "" };

            var fieldDeclarationList = new List<string>() { "" };

            var argsTypeList = new List<string>();

            for (int i = 0; i < count + 1; i++)
            {
                fieldTemplateList.Add($"T{i}");
                fieldList.Add($"Args{i}");

                argsTemplateList.Add($"TArgs{i}");
                argsList.Add($"t{i}");

                fieldDeclarationList.Add($"private T{i}? Args{i};");
            }

            var funcList = new List<string>();
            var callFuncList = new List<string>();
            for (int i = 0; i < count + 1; i++)
            {
                argsTemplateList.Clear();
                fieldTemplateList.Clear();
                fieldList.Clear();
                argsList.Clear();
                argsTypeList.Clear();
                fieldDeclarationList.Clear();
                var argsDeclarationList = new List<string>();
                for (int j = 0; j < i; j++)
                {
                    var idx = j + 1;
                    argsTemplateList.Add($"TArgs{idx}");
                    argsList.Add($"args{idx}");
                    argsDeclarationList.Add($"TArgs{idx} args{idx}");

                    argsTypeList.Add($"{argsTemplateList[j]} {argsList[j]}");


                    fieldTemplateList.Add($"T{idx}");
                    fieldList.Add($"t{idx}");
                    fieldDeclarationList.Add($"\tprivate T{idx} t{idx};");
                }

                var fieldTemplate = string.Join(", ", fieldTemplateList);
                var fieldArgs = string.Join(", ", fieldList);
                var fieldDeclaration = string.Join("\n", fieldDeclarationList);

                var argsTemplate = string.Join(", ", argsTemplateList);
                var argsStr = string.Join(", ", argsList);
                var argsDeclaration = string.Join(", ", argsDeclarationList);


                var argsType = string.Join("", argsTypeList.Select(x => $", {x}"));
                var args = string.Join(", ", argsList);
                var funcStr = this.MakeUniversaltServiceMessageContent(fieldTemplateList, fieldList, fieldDeclaration,
                    argsTemplateList, argsList);
                funcList.Add(funcStr);
            }

            var funcBody = string.Join("\n\n", funcList);
            var callFuncBody = string.Join("\n\n", callFuncList);

            var msgCode = @$"
//#nullable enable

using MessagePack;
using System.Buffers;
namespace Proxar.ServiceCore;
using Proxar.ServiceCore.Interfaces;
using Proxar.ServiceCore.Message;

{funcBody}

";

            context.RegisterPostInitializationOutput(ctx =>
                ctx.AddSource($"UniversaltServiceMessage.g.cs",
                SourceText.From(msgCode, Encoding.UTF8)));

        }
    }
}