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



using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using Proxar.AppHost;
using Proxar.AppHost.Interfaces;
using Proxar.IdGenerator;
using Proxar.Network;
using Proxar.Network.Interfaces;
using Proxar.ServiceCore;
using Proxar.Tasks;
using Proxar.Utilities;
using ServiceIntegrationTesting;
namespace TestShared;


public static class TestInitializer
{
    public static TaskCompletionSource<bool> InitFinish = new();
    public static Int32IdSafeGenerator Int32IdSafeGenerator = new Int32IdSafeGenerator() { InitValue = 1 };
    public static string StartTimeStr = "";

    public static long WorkerId = 1;
    public static long SecondWorkerId = 2;
    public static long FakeClientWorkerId = 101;

    public static TcpSocketManager SocketServiceManager = new TcpSocketManager();


    public static Func<ZFTask> OnIniting { get; set; } = null!;

    public static IServiceGroup ServerServiceGroup { get; private set; } = null!;

    public static IServiceGroup ClientServiceGroup { get; private set; } = null!;


    static TestInitializer()
    {
        StartTimeStr = TimeHelper.TimeStr();
        using var tempFile = GenerateArgs(out var args);
        try
        {
            var host = AppHostBuilder.Instance;
            host.SetCommandLineArgs(args)
                .ConfigureChannel(
                    x =>
                    {
                        x.UseEndpointResolver(new EndpointResolver());
                    }
                )
                .ConfigureDefaultServiceGroup((serviceGroup) =>
                    {
                        ServerServiceGroup = serviceGroup;
                        serviceGroup.AddServiceGroupStartAction(async (serviceId) =>
                        {
                            await NetWorkV2Helper.ConfigureInternalNet();
                            await NetWorkV2Helper.ConfigureExternalNet(onClientChannelConnectedCallback);
                            var (secondIp, port) = ChannelConfig.EndpointResolver.Resolve(SecondWorkerId);
                            await SocketServiceManager.InitializeInternalNetworkSocketService(secondIp, port);
                            Service.CreateUniqueService<HelpTest_Service>();
                            OnIniting?.Invoke();
                            InitFinish.SetResult(true);
                        });
                    }
                )
                .ConfigureMainThreadAction(() =>
                    {
                    }
                )
                .CreateServiceGroup(serviceGroup =>
                    {
                        ClientServiceGroup = serviceGroup;
                        serviceGroup.Flag = "Client";
                        serviceGroup.WorkerId = SecondWorkerId;
                    }
                )
                .ConfigureLogger(loggerConfigbuilder =>
                    {
                        var loggerFactory = LoggerFactory.Create(builder =>
                        {
                            builder.ClearProviders();
                            builder.AddNLog();
                        });
                        loggerConfigbuilder.SetLoggerFactory(loggerFactory)
                        .SetErrorAction((e, msg) =>
                        {
                            if (!InitFinish.Task.IsCompleted)
                            {
                                InitFinish.SetException(e);
                            }
                            TestResultHelper.SetFinishTest(e);
                        });
                    }
                )
                .Build();

        }
        catch (Exception e)
        {
            InitFinish.SetException(e);
        }
        finally
        {
        }
    }

    public static TempFile GenerateArgs(out string[] args)
    {
        var clusterJsonFile = "TestClusterConfig.json";

        var tempFile = ModifyClientPortAndSaveToTemp.Handle("Config.Json");


        args = new string[] {
            $"--WorkerId={WorkerId}",
            $"--ConfigFile={tempFile.FilePath}",
            $"--ClusterConfigFile={clusterJsonFile}",
        };
        return tempFile;
    }

    private static void onClientChannelConnectedCallback(IChannel channel)
    {
        TestConfig.onClientChannelConnectedCallback(channel);

    }
}