using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using MBW.HassMQTT.CommonServices;
using MBW.HassMQTT.Interfaces;
using MBW.HassMQTT.Services;
using MBW.HassMQTT.Topics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using MQTTnet;
using MQTTnet.Protocol;
using MQTTnet.Server;
using Xunit;

namespace MBW.HassMQTT.Tests;

public class MqttReconnectIntegrationTests
{
    [Fact]
    public async Task Broker_restart_reconnects_and_flushes_latest_offline_value()
    {
        int port = GetAvailablePort();
        MqttServerOptions serverOptions = new MqttServerOptionsBuilder()
            .WithDefaultEndpoint()
            .WithDefaultEndpointBoundIPAddress(IPAddress.Loopback)
            .WithDefaultEndpointBoundIPV6Address(IPAddress.None)
            .WithDefaultEndpointPort(port)
            .Build();

        using MqttServer server = new MqttServerFactory().CreateMqttServer(serverOptions);
        ConcurrentQueue<string> payloads = new ConcurrentQueue<string>();
        ConcurrentQueue<string> availability = new ConcurrentQueue<string>();
        server.InterceptingPublishAsync += args =>
        {
            if (args.ApplicationMessage.Topic == "test/reconnect")
                payloads.Enqueue(args.ApplicationMessage.ConvertPayloadToString());
            if (args.ApplicationMessage.Topic == "test/ReconnectTest/status/state")
                availability.Enqueue(args.ApplicationMessage.ConvertPayloadToString());
            return Task.CompletedTask;
        };
        await server.StartAsync();

        using IHost host = Host.CreateDefaultBuilder()
            .ConfigureServices(services =>
            {
                services.Configure<HassConfiguration>(config => config.TopicPrefix = "test");
                services.AddSingleton(provider => new HassMqttTopicBuilder(provider.GetRequiredService<IOptions<HassConfiguration>>().Value));
                services.AddAndConfigureMqtt("ReconnectTest");
                services.Configure<CommonMqttConfiguration>(config =>
                {
                    config.Server = "127.0.0.1";
                    config.Port = port;
                    config.ClientId = $"hassmqtt-test-{Guid.NewGuid():N}";
                    config.ReconnectInterval = TimeSpan.FromMilliseconds(50);
                    config.MaximumReconnectInterval = TimeSpan.FromMilliseconds(200);
                    config.ReconnectWarningInterval = TimeSpan.FromSeconds(5);
                    config.ConnectTimeout = TimeSpan.FromSeconds(2);
                });
            })
            .Build();

        MqttEvents events = host.Services.GetRequiredService<MqttEvents>();
        int connections = 0;
        TaskCompletionSource firstConnection = new(TaskCreationOptions.RunContinuationsAsynchronously);
        TaskCompletionSource secondConnection = new(TaskCreationOptions.RunContinuationsAsynchronously);
        TaskCompletionSource disconnected = new(TaskCreationOptions.RunContinuationsAsynchronously);
        events.OnConnect += (_, _) =>
        {
            int count = Interlocked.Increment(ref connections);
            if (count >= 1)
                firstConnection.TrySetResult();
            if (count >= 2)
                secondConnection.TrySetResult();
            return Task.CompletedTask;
        };
        events.OnDisconnect += (_, _) =>
        {
            disconnected.TrySetResult();
            return Task.CompletedTask;
        };

        await host.StartAsync().WaitAsync(TimeSpan.FromSeconds(10));
        IHassMqttClient client = host.Services.GetRequiredService<IHassMqttClient>();
        await WaitUntil(() => client.IsConnected, TimeSpan.FromSeconds(10));
        await firstConnection.Task.WaitAsync(TimeSpan.FromSeconds(10));

        HassMqttManager manager = host.Services.GetRequiredService<HassMqttManager>();
        MqttStateValueTopic state = manager.GetValueSender("test/reconnect");
        state.Value = "first";
        Assert.Equal(MqttFlushStatus.Completed, (await manager.FlushAll()).Status);
        await WaitUntil(() => payloads.Contains("first"), TimeSpan.FromSeconds(5));

        await server.StopAsync();
        await disconnected.Task.WaitAsync(TimeSpan.FromSeconds(10));

        state.Value = "second";
        Assert.Equal(MqttFlushStatus.Disconnected, (await manager.FlushAll()).Status);
        Assert.True(state.Dirty);

        await server.StartAsync();
        await secondConnection.Task.WaitAsync(TimeSpan.FromSeconds(10));
        await WaitUntil(() => payloads.Contains("second"), TimeSpan.FromSeconds(5));
        Assert.False(state.Dirty);

        await host.StopAsync().WaitAsync(TimeSpan.FromSeconds(10));
        Assert.Equal("problem", availability.Last());
        await server.StopAsync();
    }

    private static int GetAvailablePort()
    {
        TcpListener listener = new TcpListener(IPAddress.Loopback, 0);
        listener.Start();
        int port = ((IPEndPoint)listener.LocalEndpoint).Port;
        listener.Stop();
        return port;
    }

    private static async Task WaitUntil(Func<bool> condition, TimeSpan timeout)
    {
        using CancellationTokenSource cancellation = new CancellationTokenSource(timeout);
        while (!condition())
            await Task.Delay(20, cancellation.Token);
    }
}
