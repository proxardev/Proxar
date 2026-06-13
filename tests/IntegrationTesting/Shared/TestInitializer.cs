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
using Proxar.ServiceCore;
using Proxar.Tasks;
using Proxar.Utilities;
using ServiceIntegrationTesting;
using System.Collections.Concurrent;
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

    public static IChannel channel { get; set; } = null!;

    public static ConcurrentDictionary<long, IChannel> ClusterChannels = new ConcurrentDictionary<long, IChannel>();


    public static Func<ZFTask> OnIniting { get; set; } = null!;

    public static IServiceGroup ServerServiceGroup { get; private set; } = null!;

    public static IServiceGroup ClientServiceGroup { get; private set; } = null!;


    static TestInitializer()
    {
        StartTimeStr = TimeHelper.TimeStr();
        var args = GenerateArgs();

        var host = AppHostBuilder.Instance;
        host
            .SetCommandLineArgs(args)
            .ConfigureDefaultServiceGroup((serviceGroup) =>
                {
                    ServerServiceGroup = serviceGroup;
                    serviceGroup.AddServiceGroupStartAction(async (serviceId) =>
                    {
                        await NetWorkV2Helper.Configure(
                            OnClientChannelConnected
                            );
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
                        TestResultHelper.SetFinishTest(e);
                    });
                }
            )
            .Build();
    }

    public static string[] GenerateArgs()
    {

        var clusterJsonFile = "TestClusterConfig.json";


        var args = new string[] {
            $"--WorkerId={WorkerId}",
            $"--ConfigFile=Config.Json",
            $"--ClusterConfigFile={clusterJsonFile}",
        };
        return args;
    }

    private static void OnClientChannelConnected(IChannel channel)
    {
        channel.StrId = new Guid().ToString();
        var remoteEndPoint = channel.GetRemoteIp;
        var remotePort = channel.GetRemotePort();

    }
}