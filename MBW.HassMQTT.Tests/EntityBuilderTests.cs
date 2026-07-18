using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using MBW.HassMQTT.DiscoveryModels.Device;
using MBW.HassMQTT.DiscoveryModels.Enum;
using MBW.HassMQTT.DiscoveryModels.Models;
using MBW.HassMQTT.DiscoveryModels.Interfaces;
using MBW.HassMQTT.Extensions;
using MBW.HassMQTT.Interfaces;
using MBW.HassMQTT.Topics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using MQTTnet;
using MQTTnet.Packets;
using Newtonsoft.Json.Linq;
using Xunit;

namespace MBW.HassMQTT.Tests;

public class EntityBuilderTests
{
    [Fact]
    public void Entity_is_registered_only_after_build_and_template_can_build_multiple_identities()
    {
        HassMqttManager manager = CreateManager(new FakeMqttClient());
        IEntityBuilder<MqttSensor> template = ValidSensorTemplate(manager);

        Assert.False(manager.TryGetEntity("weather", "celsius", out _));

        IHassMqttEntity celsius = template.Build("weather", "celsius");
        IHassMqttEntity fahrenheit = template.Build("weather", "fahrenheit");

        Assert.True(manager.TryGetEntity("weather", "celsius", out IHassMqttEntity found));
        Assert.Same(celsius, found);
        Assert.NotSame(celsius.GetValueSender(HassTopicKind.State), fahrenheit.GetValueSender(HassTopicKind.State));
        Assert.Equal("weather_celsius", celsius.UniqueId);
        Assert.Equal("weather_fahrenheit", fahrenheit.UniqueId);
    }

    [Fact]
    public async Task Builder_branches_and_retained_callback_references_are_isolated()
    {
        FakeMqttClient client = new FakeMqttClient();
        HassMqttManager manager = CreateManager(client);
        MqttDeviceDocument retainedDevice = null;
        MqttSensor retainedDiscovery = null;

        IEntityBuilder<MqttSensor> common = manager.CreateEntity<MqttSensor>()
            .ConfigureTopics(HassTopicKind.State)
            .ConfigureDevice(device =>
            {
                device.Identifiers.Add("weather-hardware");
                retainedDevice = device;
            });

        IEntityBuilder<MqttSensor> celsius = common.ConfigureDiscovery(discovery =>
        {
            discovery.Name = "Celsius";
            discovery.UnitOfMeasurement = "°C";
            retainedDiscovery = discovery;
        });
        IEntityBuilder<MqttSensor> fahrenheit = common.ConfigureDiscovery(discovery =>
        {
            discovery.Name = "Fahrenheit";
            discovery.UnitOfMeasurement = "°F";
        });

        retainedDevice.Identifiers.Add("late-mutation");
        retainedDiscovery.Name = "Mutated after configuration";

        celsius.Build("weather", "celsius");
        fahrenheit.Build("weather", "fahrenheit");
        await manager.FlushAll();

        JObject celsiusPayload = DiscoveryPayload(client, "homeassistant/sensor/weather/celsius/config");
        JObject fahrenheitPayload = DiscoveryPayload(client, "homeassistant/sensor/weather/fahrenheit/config");
        Assert.Equal("Celsius", (string)celsiusPayload["name"]);
        Assert.Equal("°C", (string)celsiusPayload["unit_of_measurement"]);
        Assert.Equal("Fahrenheit", (string)fahrenheitPayload["name"]);
        Assert.Equal("°F", (string)fahrenheitPayload["unit_of_measurement"]);
        Assert.Equal("weather-hardware", (string)celsiusPayload["device"]?["identifiers"]?[0]);
        Assert.Single(celsiusPayload["device"]?["identifiers"] ?? throw new InvalidOperationException());
    }

    [Fact]
    public async Task Build_preserves_explicit_topics_and_applies_unique_id_precedence()
    {
        FakeMqttClient client = new FakeMqttClient();
        HassMqttManager manager = CreateManager(client);
        IEntityBuilder<MqttSensor> builder = ValidSensorTemplate(manager)
            .ConfigureDiscovery(discovery =>
            {
                discovery.StateTopic = "custom/state";
                discovery.UniqueId = "configured-id";
            });

        IHassMqttEntity configured = builder.Build("weather", "configured");
        IHassMqttEntity explicitValue = builder.Build("weather", "explicit", "explicit-id");
        await manager.FlushAll();

        Assert.Equal("custom/state", configured.GetValueSender(HassTopicKind.State).PublishTopic);
        Assert.Equal("configured-id", configured.UniqueId);
        Assert.Equal("explicit-id", explicitValue.UniqueId);
        Assert.Equal("configured-id", (string)DiscoveryPayload(client, "homeassistant/sensor/weather/configured/config")["unique_id"]);
        Assert.Equal("explicit-id", (string)DiscoveryPayload(client, "homeassistant/sensor/weather/explicit/config")["unique_id"]);
    }

    [Fact]
    public void Invalid_and_duplicate_builds_do_not_change_registration()
    {
        HassMqttManager manager = CreateManager(new FakeMqttClient());
        IEntityBuilder<MqttSensor> invalid = manager.CreateEntity<MqttSensor>()
            .ConfigureTopics(HassTopicKind.State);

        Assert.Throws<ValidationException>(() => invalid.Build("weather", "invalid"));
        Assert.False(manager.TryGetEntity("weather", "invalid", out _));

        IEntityBuilder<MqttSensor> valid = ValidSensorTemplate(manager);
        IHassMqttEntity first = valid.Build("weather", "duplicate");
        Assert.Throws<InvalidOperationException>(() => valid.Build("weather", "duplicate"));
        Assert.True(manager.TryGetEntity("weather", "duplicate", out IHassMqttEntity found));
        Assert.Same(first, found);
    }

    [Fact]
    public void Validation_warnings_are_reported_but_do_not_reject_build()
    {
        RecordingLogger<HassMqttManager> logger = new RecordingLogger<HassMqttManager>();
        HassMqttManager manager = CreateManager(new FakeMqttClient(), logger);
        IEntityBuilder<MqttSensor> builder = ValidSensorTemplate(manager)
            .ConfigureDiscovery(discovery => discovery.JsonAttributesTemplate = "{{ value_json.attributes }}");

        IHassMqttEntity entity = builder.Build("weather", "warning");

        Assert.NotNull(entity);
        Assert.Contains(logger.Entries, entry => entry.Level == LogLevel.Warning && entry.Message.Contains("JsonAttributesTopic"));
    }

    [Fact]
    public async Task Default_plan_publishes_discovery_state_and_attributes_separately()
    {
        FakeMqttClient client = new FakeMqttClient();
        HassMqttManager manager = CreateManager(client);
        IHassMqttEntity entity = manager.CreateEntity<MqttSensor>()
            .ConfigureTopics(HassTopicKind.State, HassTopicKind.JsonAttributes)
            .ConfigureDevice(device => device.Identifiers.Add("weather-hardware"))
            .Build("weather", "outside");

        entity.SetValue(HassTopicKind.State, 21.4);
        entity.SetAttribute("quality", "good");
        MqttFlushResult result = await manager.FlushAll();

        Assert.Equal(MqttFlushStatus.Completed, result.Status);
        Assert.Equal(1, result.DiscoveryDocuments);
        Assert.Equal(1, result.Values);
        Assert.Equal(1, result.Attributes);
        Assert.Equal(3, client.Messages.Count);
        Assert.All(client.Messages, message => Assert.True(message.Retain));
        Assert.Contains(client.Messages, message => message.Topic == "test/weather/outside/state" && message.ConvertPayloadToString() == "21.4");
        Assert.Contains(client.Messages, message => message.Topic == "test/weather/outside/json_attributes" && message.ConvertPayloadToString() == "{\"quality\":\"good\"}");
    }

    [Fact]
    public async Task Broker_rejection_leaves_entity_source_dirty()
    {
        FakeMqttClient client = new FakeMqttClient();
        HassMqttManager manager = CreateManager(client);
        IHassMqttEntity entity = ValidSensorTemplate(manager).Build("weather", "rejected");
        await manager.FlushAll();

        MqttStateValueTopic state = entity.GetValueSender(HassTopicKind.State);
        state.Value = "pending";
        client.PublishOverride = _ => Task.FromResult(new MqttClientPublishResult(
            null,
            MqttClientPublishReasonCode.UnspecifiedError,
            "rejected",
            Array.Empty<MqttUserProperty>()));

        MqttFlushResult result = await manager.FlushAll();

        Assert.Equal(MqttFlushStatus.BrokerRejected, result.Status);
        Assert.True(state.Dirty);
    }

    [Fact]
    public async Task Entity_change_during_publish_remains_dirty_for_the_next_flush()
    {
        FakeMqttClient client = new FakeMqttClient();
        HassMqttManager manager = CreateManager(client);
        IHassMqttEntity entity = ValidSensorTemplate(manager).Build("weather", "concurrent");
        await manager.FlushAll();

        MqttStateValueTopic state = entity.GetValueSender(HassTopicKind.State);
        state.Value = "first";
        TaskCompletionSource started = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        TaskCompletionSource<MqttClientPublishResult> pending = new TaskCompletionSource<MqttClientPublishResult>(TaskCreationOptions.RunContinuationsAsynchronously);
        client.PublishOverride = _ =>
        {
            started.TrySetResult();
            return pending.Task;
        };

        Task<MqttFlushResult> flush = manager.FlushAll();
        await started.Task.WaitAsync(TimeSpan.FromSeconds(5));
        state.Value = "second";
        pending.SetResult(Success());

        Assert.Equal(MqttFlushStatus.Completed, (await flush).Status);
        Assert.True(state.Dirty);

        client.PublishOverride = null;
        await manager.FlushAll();
        Assert.False(state.Dirty);
        Assert.Equal("second", client.Messages[^1].ConvertPayloadToString());
    }

    [Fact]
    public async Task Reconnect_does_not_publish_uninitialized_entity_values()
    {
        FakeMqttClient client = new FakeMqttClient();
        HassMqttManager manager = CreateManager(client);
        ValidSensorTemplate(manager).Build("weather", "untouched");
        await manager.FlushAll();
        client.Messages.Clear();

        manager.MarkAllValuesDirty();
        MqttFlushResult result = await manager.FlushAll();

        Assert.Equal(1, result.DiscoveryDocuments);
        Assert.Equal(0, result.Values);
        Assert.Single(client.Messages);
    }

    [Fact]
    public void Runtime_entity_does_not_expose_a_discovery_document()
    {
        Assert.Null(typeof(IHassMqttEntity).GetProperty("Discovery"));
    }

    [Fact]
    public void Every_concrete_discovery_model_can_create_an_identity_free_builder()
    {
        HassMqttManager manager = CreateManager(new FakeMqttClient());
        MethodInfo createEntity = typeof(HassMqttManager).GetMethod(nameof(HassMqttManager.CreateEntity));
        Type[] discoveryTypes = typeof(MqttSensor).Assembly.GetTypes()
            .Where(type => !type.IsAbstract && typeof(IHassDiscoveryDocument).IsAssignableFrom(type))
            .ToArray();

        Assert.NotEmpty(discoveryTypes);
        foreach (Type discoveryType in discoveryTypes)
            Assert.NotNull(createEntity.MakeGenericMethod(discoveryType).Invoke(manager, null));
    }

    private static IEntityBuilder<MqttSensor> ValidSensorTemplate(HassMqttManager manager) =>
        manager.CreateEntity<MqttSensor>()
            .ConfigureTopics(HassTopicKind.State)
            .ConfigureDevice(device => device.Identifiers.Add("weather-hardware"));

    private static JObject DiscoveryPayload(FakeMqttClient client, string topic)
    {
        MqttApplicationMessage message = Assert.Single(client.Messages, candidate => candidate.Topic == topic);
        return JObject.Parse(message.ConvertPayloadToString());
    }

    private static HassMqttManager CreateManager(FakeMqttClient client, ILogger<HassMqttManager> logger = null)
    {
        ServiceProvider provider = new ServiceCollection().BuildServiceProvider();
        HassMqttTopicBuilder topics = new HassMqttTopicBuilder(new HassConfiguration
        {
            DiscoveryPrefix = "homeassistant",
            TopicPrefix = "test"
        });
        return new HassMqttManager(
            provider,
            Options.Create(new HassMqttManagerConfiguration()),
            client,
            topics,
            logger ?? NullLogger<HassMqttManager>.Instance);
    }

    private sealed class FakeMqttClient : IHassMqttClient
    {
        public bool IsConnected { get; set; } = true;
        public List<MqttApplicationMessage> Messages { get; } = new List<MqttApplicationMessage>();
        public Func<MqttApplicationMessage, Task<MqttClientPublishResult>> PublishOverride { get; set; }

        public Task<MqttClientPublishResult> PublishAsync(MqttApplicationMessage message, CancellationToken token = default)
        {
            Messages.Add(message);
            return PublishOverride?.Invoke(message) ?? Task.FromResult(Success());
        }

        public Task SubscribeAsync(MqttClientSubscribeOptions options, CancellationToken token = default) => Task.CompletedTask;
        public Task UnsubscribeAsync(MqttClientUnsubscribeOptions options, CancellationToken token = default) => Task.CompletedTask;
    }

    private static MqttClientPublishResult Success() =>
        new MqttClientPublishResult(
            null,
            MqttClientPublishReasonCode.Success,
            null,
            Array.Empty<MqttUserProperty>());

    private sealed class RecordingLogger<T> : ILogger<T>
    {
        public List<(LogLevel Level, string Message)> Entries { get; } = new List<(LogLevel, string)>();

        public IDisposable BeginScope<TState>(TState state) => NullScope.Instance;
        public bool IsEnabled(LogLevel logLevel) => true;
        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter) =>
            Entries.Add((logLevel, formatter(state, exception)));

        private sealed class NullScope : IDisposable
        {
            public static NullScope Instance { get; } = new NullScope();
            public void Dispose()
            {
            }
        }
    }
}
