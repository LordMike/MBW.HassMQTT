#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MBW.HassMQTT.CommonServices.AliveAndWill;
using MBW.HassMQTT.DiscoveryModels.Models;
using MBW.HassMQTT.Interfaces;
using MBW.HassMQTT.Topics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using MQTTnet;
using MQTTnet.Packets;
using MQTTnet.Protocol;
using Xunit;

namespace MBW.HassMQTT.Tests;

public class HassConnectedEntityServiceTests
{
    [Fact]
    public async Task Connect_publishes_status_once_when_manager_connect_handler_runs_afterwards()
    {
        (HassConnectedEntityService service, HassMqttManager manager, FakeMqttClient client) = CreateHarness();
        await StartService(service, manager);
        client.Messages.Clear();
        MqttClientConnectedEventArgs args = ConnectedArgs();

        await ((IMqttEventReceiver)service).OnConnect(args, default);
        await ((IMqttEventReceiver)manager).OnConnect(args, default);

        MqttApplicationMessage status = Assert.Single(client.Messages, message => message.Topic == service.StateTopic);
        Assert.Equal("ok", status.ConvertPayloadToString());
        Assert.True(status.Retain);
        Assert.Equal(MqttQualityOfServiceLevel.AtLeastOnce, status.QualityOfServiceLevel);
    }

    [Fact]
    public async Task Stopping_publishes_problem_while_manager_flush_is_busy()
    {
        (HassConnectedEntityService service, HassMqttManager manager, FakeMqttClient client) = CreateHarness();
        await StartService(service, manager);
        TaskCompletionSource firstPublishStarted = new(TaskCreationOptions.RunContinuationsAsynchronously);
        TaskCompletionSource<MqttClientPublishResult> pendingPublish = new(TaskCreationOptions.RunContinuationsAsynchronously);
        int calls = 0;
        client.PublishOverride = _ =>
        {
            if (Interlocked.Increment(ref calls) == 1)
            {
                firstPublishStarted.TrySetResult();
                return pendingPublish.Task;
            }

            return Task.FromResult(Success());
        };

        Task<MqttFlushResult> flush = manager.FlushAll();
        await firstPublishStarted.Task.WaitAsync(TimeSpan.FromSeconds(5));

        await ((IMqttEventReceiver)service).OnStopping(default);

        MqttApplicationMessage status = Assert.Single(
            client.Messages,
            message => message.Topic == service.StateTopic && message.ConvertPayloadToString() == "problem");
        Assert.True(status.Retain);
        Assert.Equal(MqttQualityOfServiceLevel.AtLeastOnce, status.QualityOfServiceLevel);

        pendingPublish.SetResult(Success());
        Assert.Equal(MqttFlushStatus.Completed, (await flush).Status);
    }

    [Fact]
    public async Task Stopping_while_disconnected_does_not_publish()
    {
        (HassConnectedEntityService service, HassMqttManager manager, FakeMqttClient client) = CreateHarness();
        await StartService(service, manager);
        client.Messages.Clear();
        client.IsConnected = false;

        await ((IMqttEventReceiver)service).OnStopping(default);

        Assert.Empty(client.Messages);
    }

    [Fact]
    public async Task Broker_rejection_is_reported_by_the_lifecycle_handler()
    {
        (HassConnectedEntityService service, HassMqttManager manager, FakeMqttClient client) = CreateHarness();
        await StartService(service, manager);
        client.PublishOverride = _ => Task.FromResult(new MqttClientPublishResult(
            null,
            MqttClientPublishReasonCode.UnspecifiedError,
            "rejected",
            Array.Empty<MqttUserProperty>()));

        InvalidOperationException exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => ((IMqttEventReceiver)service).OnConnect(ConnectedArgs(), default));

        Assert.Contains(service.StateTopic, exception.Message, StringComparison.Ordinal);
    }

    private static (HassConnectedEntityService Service, HassMqttManager Manager, FakeMqttClient Client) CreateHarness()
    {
        FakeMqttClient client = new();
        ServiceProvider provider = new ServiceCollection().BuildServiceProvider();
        HassMqttTopicBuilder topics = new(new HassConfiguration
        {
            DiscoveryPrefix = "homeassistant",
            TopicPrefix = "test"
        });
        HassMqttManager manager = new(
            provider,
            Options.Create(new HassMqttManagerConfiguration()),
            client,
            topics,
            NullLogger<HassMqttManager>.Instance);
        HassConnectedEntityService service = new(
            Options.Create(new HassConnectedEntityServiceConfig
            {
                DeviceId = "test-application",
                EntityId = "status",
                DiscoveryDeviceName = "Test application",
                DiscoveryEntityName = "Status"
            }),
            client,
            manager,
            topics);

        return (service, manager, client);
    }

    private static MqttClientConnectedEventArgs ConnectedArgs() =>
        new(new MqttClientConnectResult());

    private static async Task StartService(HassConnectedEntityService service, HassMqttManager manager)
    {
        await service.StartAsync(default);
        DateTime deadline = DateTime.UtcNow.AddSeconds(5);
        while (!manager.TryGetEntity<MqttBinarySensor>("test-application", "status", out _))
        {
            if (DateTime.UtcNow >= deadline)
                throw new TimeoutException("The connected entity service did not register its status entity.");
            await Task.Delay(10);
        }
    }

    private static MqttClientPublishResult Success() =>
        new(
            null,
            MqttClientPublishReasonCode.Success,
            null,
            Array.Empty<MqttUserProperty>());

    private sealed class FakeMqttClient : IHassMqttClient
    {
        public bool IsConnected { get; set; } = true;
        public List<MqttApplicationMessage> Messages { get; } = new();
        public Func<MqttApplicationMessage, Task<MqttClientPublishResult>>? PublishOverride { get; set; }

        public Task<MqttClientPublishResult> PublishAsync(MqttApplicationMessage message, CancellationToken token = default)
        {
            Messages.Add(message);
            return PublishOverride?.Invoke(message) ?? Task.FromResult(Success());
        }

        public Task SubscribeAsync(MqttClientSubscribeOptions options, CancellationToken token = default) => Task.CompletedTask;
        public Task UnsubscribeAsync(MqttClientUnsubscribeOptions options, CancellationToken token = default) => Task.CompletedTask;
    }
}
