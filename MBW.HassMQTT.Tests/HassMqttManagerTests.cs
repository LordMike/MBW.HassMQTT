using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MBW.HassMQTT.DiscoveryModels.Models;
using MBW.HassMQTT.Interfaces;
using MBW.HassMQTT.Topics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using MQTTnet;
using MQTTnet.Packets;
using Xunit;

namespace MBW.HassMQTT.Tests;

public class HassMqttManagerTests
{
    [Fact]
    public async Task Disconnected_flush_returns_without_clearing_state()
    {
        FakeMqttClient client = new FakeMqttClient { IsConnected = false };
        HassMqttManager manager = CreateManager(client);
        MqttStateValueTopic sender = manager.GetValueSender("test/state");
        sender.Value = "value";

        MqttFlushResult result = await manager.FlushAll();

        Assert.Equal(MqttFlushStatus.Disconnected, result.Status);
        Assert.True(sender.Dirty);
        Assert.Empty(client.Messages);
    }

    [Fact]
    public async Task Change_during_publish_remains_dirty_and_is_sent_next()
    {
        TaskCompletionSource<MqttClientPublishResult> pending = new(TaskCreationOptions.RunContinuationsAsynchronously);
        FakeMqttClient client = new FakeMqttClient { PublishOverride = _ => pending.Task };
        HassMqttManager manager = CreateManager(client);
        MqttStateValueTopic sender = manager.GetValueSender("test/state");
        sender.Value = "first";

        Task<MqttFlushResult> flush = manager.FlushAll();
        await client.PublishStarted.Task.WaitAsync(TimeSpan.FromSeconds(5));
        sender.Value = "second";
        pending.SetResult(Success());

        Assert.Equal(MqttFlushStatus.Completed, (await flush).Status);
        Assert.True(sender.Dirty);

        client.PublishOverride = null;
        Assert.Equal(MqttFlushStatus.Completed, (await manager.FlushAll()).Status);
        Assert.False(sender.Dirty);
        Assert.Equal("second", client.Messages[^1].ConvertPayloadToString());
    }

    [Fact]
    public async Task Concurrent_flush_returns_busy()
    {
        TaskCompletionSource<MqttClientPublishResult> pending = new(TaskCreationOptions.RunContinuationsAsynchronously);
        FakeMqttClient client = new FakeMqttClient { PublishOverride = _ => pending.Task };
        HassMqttManager manager = CreateManager(client);
        manager.GetValueSender("test/state").Value = "value";

        Task<MqttFlushResult> first = manager.FlushAll();
        await client.PublishStarted.Task.WaitAsync(TimeSpan.FromSeconds(5));
        MqttFlushResult second = await manager.FlushAll();
        pending.SetResult(Success());

        Assert.Equal(MqttFlushStatus.Busy, second.Status);
        Assert.Equal(MqttFlushStatus.Completed, (await first).Status);
    }

    [Fact]
    public async Task Reconnect_waits_for_active_flush_and_publishes_latest_value()
    {
        TaskCompletionSource<MqttClientPublishResult> pending = new(TaskCreationOptions.RunContinuationsAsynchronously);
        FakeMqttClient client = new FakeMqttClient { PublishOverride = _ => pending.Task };
        HassMqttManager manager = CreateManager(client);
        MqttStateValueTopic sender = manager.GetValueSender("test/state");
        sender.Value = "first";

        Task<MqttFlushResult> activeFlush = manager.FlushAll();
        await client.PublishStarted.Task.WaitAsync(TimeSpan.FromSeconds(5));
        sender.Value = "latest";
        Task reconnect = ((IMqttEventReceiver)manager).OnConnect(new MqttClientConnectedEventArgs(new MqttClientConnectResult()), default);

        Assert.False(reconnect.IsCompleted);
        pending.SetResult(Success());

        Assert.Equal(MqttFlushStatus.Completed, (await activeFlush).Status);
        await reconnect;
        Assert.Equal(2, client.Messages.Count);
        Assert.Equal("first", client.Messages[0].ConvertPayloadToString());
        Assert.Equal("latest", client.Messages[1].ConvertPayloadToString());
        Assert.False(sender.Dirty);
    }

    [Fact]
    public async Task Standalone_topic_registries_preserve_case_distinct_topics()
    {
        FakeMqttClient client = new FakeMqttClient();
        HassMqttManager manager = CreateManager(client);
        MqttStateValueTopic upperValue = manager.GetValueSender("test/State");
        MqttStateValueTopic lowerValue = manager.GetValueSender("test/state");
        MqttAttributesTopic upperAttributes = manager.GetAttributesSender("test/Attributes");
        MqttAttributesTopic lowerAttributes = manager.GetAttributesSender("test/attributes");

        upperValue.Value = "upper";
        lowerValue.Value = "lower";
        upperAttributes.SetAttribute("value", "upper");
        lowerAttributes.SetAttribute("value", "lower");
        MqttFlushResult result = await manager.FlushAll();

        Assert.NotSame(upperValue, lowerValue);
        Assert.NotSame(upperAttributes, lowerAttributes);
        Assert.Equal(2, result.Values);
        Assert.Equal(2, result.Attributes);
        Assert.Contains(client.Messages, message => message.Topic == "test/State");
        Assert.Contains(client.Messages, message => message.Topic == "test/state");
        Assert.Contains(client.Messages, message => message.Topic == "test/Attributes");
        Assert.Contains(client.Messages, message => message.Topic == "test/attributes");
    }

    [Fact]
    public async Task Removal_registry_preserves_case_distinct_discovery_topics()
    {
        FakeMqttClient client = new FakeMqttClient();
        HassMqttManager manager = CreateManager(client);

        manager.RemoveEntity<MqttSensor>("weather", "State");
        manager.RemoveEntity<MqttSensor>("weather", "state");
        MqttFlushResult result = await manager.FlushAll();

        Assert.Equal(2, result.RemovedTopics);
        Assert.Contains(client.Messages, message => message.Topic == "homeassistant/sensor/weather/State/config");
        Assert.Contains(client.Messages, message => message.Topic == "homeassistant/sensor/weather/state/config");
        Assert.All(client.Messages, message => Assert.Equal(0, message.Payload.Length));
    }

    private static HassMqttManager CreateManager(IHassMqttClient client)
    {
        ServiceProvider provider = new ServiceCollection().BuildServiceProvider();
        HassMqttTopicBuilder topics = new HassMqttTopicBuilder(new HassConfiguration
        {
            DiscoveryPrefix = "homeassistant",
            TopicPrefix = "test"
        });
        return new HassMqttManager(provider, Options.Create(new HassMqttManagerConfiguration()), client, topics, NullLogger<HassMqttManager>.Instance);
    }

    private static MqttClientPublishResult Success() =>
        new(null, MqttClientPublishReasonCode.Success, null, Array.Empty<MqttUserProperty>());

    private sealed class FakeMqttClient : IHassMqttClient
    {
        public bool IsConnected { get; set; } = true;
        public List<MqttApplicationMessage> Messages { get; } = new();
        public Func<MqttApplicationMessage, Task<MqttClientPublishResult>> PublishOverride { get; set; }
        public TaskCompletionSource PublishStarted { get; } = new(TaskCreationOptions.RunContinuationsAsynchronously);

        public Task<MqttClientPublishResult> PublishAsync(MqttApplicationMessage message, CancellationToken token = default)
        {
            Messages.Add(message);
            PublishStarted.TrySetResult();
            return PublishOverride?.Invoke(message) ?? Task.FromResult(Success());
        }

        public Task SubscribeAsync(MqttClientSubscribeOptions options, CancellationToken token = default) => Task.CompletedTask;
        public Task UnsubscribeAsync(MqttClientUnsubscribeOptions options, CancellationToken token = default) => Task.CompletedTask;
    }
}
