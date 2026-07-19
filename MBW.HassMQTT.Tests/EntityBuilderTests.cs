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
using System.Text.Json.Nodes;
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

        JsonObject celsiusPayload = DiscoveryPayload(client, "homeassistant/sensor/weather/celsius/config");
        JsonObject fahrenheitPayload = DiscoveryPayload(client, "homeassistant/sensor/weather/fahrenheit/config");
        Assert.Equal("Celsius", (string)celsiusPayload["name"]);
        Assert.Equal("°C", (string)celsiusPayload["unit_of_measurement"]);
        Assert.Equal("Fahrenheit", (string)fahrenheitPayload["name"]);
        Assert.Equal("°F", (string)fahrenheitPayload["unit_of_measurement"]);
        Assert.Equal("weather-hardware", (string)celsiusPayload["device"]?["identifiers"]);
    }

    [Fact]
    public async Task Builder_snapshot_preserves_optional_presence_states()
    {
        FakeMqttClient client = new FakeMqttClient();
        HassMqttManager manager = CreateManager(client);
        IEntityBuilder<MqttSensor> builder = ValidSensorTemplate(manager)
            .ConfigureDiscovery(discovery =>
            {
                discovery.Name = null;
                discovery.UnitOfMeasurement = default;
                discovery.DeviceClass = HassSensorDeviceClass.Temperature;
            });

        builder.Build("weather", "optional");
        await manager.FlushAll();

        JsonObject payload = DiscoveryPayload(client, "homeassistant/sensor/weather/optional/config");
        Assert.True(payload.ContainsKey("name"));
        Assert.Null(payload["name"]);
        Assert.False(payload.ContainsKey("unit_of_measurement"));
        Assert.Equal("temperature", payload["device_class"]!.GetValue<string>());
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
    public async Task Combined_plan_publishes_state_and_attributes_as_one_payload()
    {
        FakeMqttClient client = new FakeMqttClient();
        HassMqttManager manager = CreateManager(client);
        IHassMqttEntity entity = manager.CreateEntity<MqttSensor>()
            .ConfigureDevice(device => device.Identifiers.Add("weather-hardware"))
            .PublishStateAndAttributesTogether()
            .Build("weather", "combined");

        entity.SetValue(HassTopicKind.State, 21.4);
        entity.SetAttribute("quality", "good");
        MqttFlushResult result = await manager.FlushAll();

        Assert.Equal(MqttFlushStatus.Completed, result.Status);
        Assert.Equal(1, result.DiscoveryDocuments);
        Assert.Equal(1, result.Values);
        Assert.Equal(0, result.Attributes);
        Assert.Equal(2, client.Messages.Count);

        JsonObject discovery = DiscoveryPayload(client, "homeassistant/sensor/weather/combined/config");
        Assert.Equal("test/weather/combined/state", (string)discovery["state_topic"]);
        Assert.Equal((string)discovery["state_topic"], (string)discovery["json_attributes_topic"]);
        Assert.Equal("{{ value_json.state }}", (string)discovery["value_template"]);
        Assert.Equal("{{ value_json.attributes | tojson }}", (string)discovery["json_attributes_template"]);

        MqttApplicationMessage valueMessage = Assert.Single(client.Messages, message => message.Topic == "test/weather/combined/state");
        Assert.True(JsonNode.DeepEquals(
            JsonNode.Parse("{\"state\":21.4,\"attributes\":{\"quality\":\"good\"}}"),
            JsonNode.Parse(valueMessage.ConvertPayloadToString())));
    }

    [Fact]
    public async Task Combined_plan_preserves_identical_custom_topics_and_supports_state_value_template_models()
    {
        FakeMqttClient client = new FakeMqttClient();
        HassMqttManager manager = CreateManager(client);
        manager.CreateEntity<MqttSiren>()
            .ConfigureDevice(device => device.Identifiers.Add("siren-hardware"))
            .ConfigureDiscovery(discovery =>
            {
                discovery.StateTopic = "custom/siren/state";
                discovery.JsonAttributesTopic = "custom/siren/state";
            })
            .PublishStateAndAttributesTogether()
            .Build("siren", "combined");

        await manager.FlushAll();

        JsonObject discovery = DiscoveryPayload(client, "homeassistant/siren/siren/combined/config");
        Assert.Equal("custom/siren/state", (string)discovery["state_topic"]);
        Assert.Equal("custom/siren/state", (string)discovery["json_attributes_topic"]);
        Assert.Equal("{{ value_json.state }}", (string)discovery["state_value_template"]);
    }

    [Fact]
    public void Combined_plan_rejects_unequal_topics_and_custom_templates_without_registration()
    {
        HassMqttManager manager = CreateManager(new FakeMqttClient());

        IEntityBuilder<MqttSensor> unequalTopics = manager.CreateEntity<MqttSensor>()
            .ConfigureDevice(device => device.Identifiers.Add("weather-hardware"))
            .ConfigureDiscovery(discovery =>
            {
                discovery.StateTopic = "custom/State";
                discovery.JsonAttributesTopic = "custom/state";
            })
            .PublishStateAndAttributesTogether();
        IEntityBuilder<MqttSensor> oneSidedTopic = manager.CreateEntity<MqttSensor>()
            .ConfigureDevice(device => device.Identifiers.Add("weather-hardware"))
            .ConfigureDiscovery(discovery => discovery.StateTopic = "custom/state")
            .PublishStateAndAttributesTogether();
        IEntityBuilder<MqttSensor> customStateTemplate = manager.CreateEntity<MqttSensor>()
            .ConfigureDevice(device => device.Identifiers.Add("weather-hardware"))
            .ConfigureDiscovery(discovery => discovery.ValueTemplate = "{{ value | upper }}")
            .PublishStateAndAttributesTogether();
        IEntityBuilder<MqttSensor> customAttributesTemplate = manager.CreateEntity<MqttSensor>()
            .ConfigureDevice(device => device.Identifiers.Add("weather-hardware"))
            .ConfigureDiscovery(discovery => discovery.JsonAttributesTemplate = "{{ value_json.custom | tojson }}")
            .PublishStateAndAttributesTogether();
        IEntityBuilder<MqttSensor> additionalStateTemplate = manager.CreateEntity<MqttSensor>()
            .ConfigureDevice(device => device.Identifiers.Add("weather-hardware"))
            .ConfigureDiscovery(discovery => discovery.LastResetValueTemplate = "{{ value_json.last_reset }}")
            .PublishStateAndAttributesTogether();

        Assert.Throws<ValidationException>(() => unequalTopics.Build("weather", "unequal"));
        Assert.Throws<ValidationException>(() => oneSidedTopic.Build("weather", "one-sided"));
        Assert.Throws<ValidationException>(() => customStateTemplate.Build("weather", "state-template"));
        Assert.Throws<ValidationException>(() => customAttributesTemplate.Build("weather", "attributes-template"));
        Assert.Throws<ValidationException>(() => additionalStateTemplate.Build("weather", "additional-template"));
        Assert.False(manager.TryGetEntity("weather", "unequal", out _));
        Assert.False(manager.TryGetEntity("weather", "one-sided", out _));
        Assert.False(manager.TryGetEntity("weather", "state-template", out _));
        Assert.False(manager.TryGetEntity("weather", "attributes-template", out _));
        Assert.False(manager.TryGetEntity("weather", "additional-template", out _));
    }

    [Fact]
    public void Combined_plan_rejects_unsupported_models_at_build()
    {
        HassMqttManager manager = CreateManager(new FakeMqttClient());
        IEntityBuilder<MqttButton> builder = manager.CreateEntity<MqttButton>()
            .PublishStateAndAttributesTogether();

        Assert.Throws<NotSupportedException>(() => builder.Build("button", "unsupported"));
        Assert.False(manager.TryGetEntity("button", "unsupported", out _));
    }

    [Fact]
    public async Task Combined_plan_waits_for_initialized_state_and_includes_explicit_null()
    {
        FakeMqttClient client = new FakeMqttClient();
        HassMqttManager manager = CreateManager(client);
        IHassMqttEntity entity = manager.CreateEntity<MqttSensor>()
            .ConfigureDevice(device => device.Identifiers.Add("weather-hardware"))
            .PublishStateAndAttributesTogether()
            .Build("weather", "initialization");
        await manager.FlushAll();
        client.Messages.Clear();

        entity.SetAttribute("quality", "pending");
        MqttFlushResult attributesOnly = await manager.FlushAll();

        Assert.Equal(MqttFlushStatus.Completed, attributesOnly.Status);
        Assert.Empty(client.Messages);
        Assert.True(entity.GetAttributesSender().Dirty);

        entity.SetValue(HassTopicKind.State, null);
        MqttFlushResult initialized = await manager.FlushAll();

        Assert.Equal(1, initialized.Values);
        MqttApplicationMessage message = Assert.Single(client.Messages);
        JsonObject payload = JsonNode.Parse(message.ConvertPayloadToString())!.AsObject();
        Assert.True(payload.ContainsKey("state"));
        Assert.Null(payload["state"]);
        Assert.Equal("pending", (string)payload["attributes"]?["quality"]);
        Assert.False(entity.GetValueSender(HassTopicKind.State).Dirty);
        Assert.False(entity.GetAttributesSender().Dirty);
    }

    [Fact]
    public async Task Combined_plan_rejection_leaves_both_sources_dirty()
    {
        FakeMqttClient client = new FakeMqttClient();
        HassMqttManager manager = CreateManager(client);
        IHassMqttEntity entity = manager.CreateEntity<MqttSensor>()
            .ConfigureDevice(device => device.Identifiers.Add("weather-hardware"))
            .PublishStateAndAttributesTogether()
            .Build("weather", "rejected-combined");
        await manager.FlushAll();

        entity.SetValue(HassTopicKind.State, "pending");
        entity.SetAttribute("quality", "pending");
        client.PublishOverride = _ => Task.FromResult(new MqttClientPublishResult(
            null,
            MqttClientPublishReasonCode.UnspecifiedError,
            "rejected",
            Array.Empty<MqttUserProperty>()));

        MqttFlushResult result = await manager.FlushAll();

        Assert.Equal(MqttFlushStatus.BrokerRejected, result.Status);
        Assert.True(entity.GetValueSender(HassTopicKind.State).Dirty);
        Assert.True(entity.GetAttributesSender().Dirty);
    }

    [Fact]
    public async Task Combined_plan_preserves_state_conversion_and_publishes_attribute_removal()
    {
        FakeMqttClient client = new FakeMqttClient();
        HassMqttManager manager = CreateManager(client);
        IHassMqttEntity entity = manager.CreateEntity<MqttSensor>()
            .ConfigureDevice(device => device.Identifiers.Add("weather-hardware"))
            .PublishStateAndAttributesTogether()
            .Build("weather", "conversion");
        DateTimeOffset state = new DateTimeOffset(2026, 7, 19, 14, 30, 0, TimeSpan.FromHours(2));

        entity.SetValue(HassTopicKind.State, state);
        await manager.FlushAll();

        string stateOnlyPayload = client.Messages[^1].ConvertPayloadToString();
        JsonObject stateOnly = JsonNode.Parse(stateOnlyPayload)!.AsObject();
        Assert.Contains("\"state\":\"2026-07-19T14:30:00.0000000+02:00\"", stateOnlyPayload);
        Assert.Empty(stateOnly["attributes"]!.AsObject());

        entity.SetAttribute("quality", "good");
        await manager.FlushAll();
        entity.GetAttributesSender().RemoveAttribute("quality");
        await manager.FlushAll();

        string removedPayload = client.Messages[^1].ConvertPayloadToString();
        JsonObject removed = JsonNode.Parse(removedPayload)!.AsObject();
        Assert.Contains("\"state\":\"2026-07-19T14:30:00.0000000+02:00\"", removedPayload);
        Assert.Empty(removed["attributes"]!.AsObject());
    }

    [Fact]
    public async Task Combined_plan_changes_during_publish_remain_dirty_together()
    {
        FakeMqttClient client = new FakeMqttClient();
        HassMqttManager manager = CreateManager(client);
        IHassMqttEntity entity = manager.CreateEntity<MqttSensor>()
            .ConfigureDevice(device => device.Identifiers.Add("weather-hardware"))
            .PublishStateAndAttributesTogether()
            .Build("weather", "concurrent-combined");
        await manager.FlushAll();

        entity.SetValue(HassTopicKind.State, "first");
        entity.SetAttribute("quality", "first");
        TaskCompletionSource started = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        TaskCompletionSource<MqttClientPublishResult> pending = new TaskCompletionSource<MqttClientPublishResult>(TaskCreationOptions.RunContinuationsAsynchronously);
        client.PublishOverride = _ =>
        {
            started.TrySetResult();
            return pending.Task;
        };

        Task<MqttFlushResult> flush = manager.FlushAll();
        await started.Task.WaitAsync(TimeSpan.FromSeconds(5));
        entity.SetValue(HassTopicKind.State, "second");
        entity.SetAttribute("quality", "second");
        pending.SetResult(Success());

        Assert.Equal(MqttFlushStatus.Completed, (await flush).Status);
        Assert.True(entity.GetValueSender(HassTopicKind.State).Dirty);
        Assert.True(entity.GetAttributesSender().Dirty);

        client.PublishOverride = null;
        await manager.FlushAll();
        JsonObject payload = JsonNode.Parse(client.Messages[^1].ConvertPayloadToString())!.AsObject();
        Assert.Equal("second", (string)payload["state"]);
        Assert.Equal("second", (string)payload["attributes"]?["quality"]);
        Assert.False(entity.GetValueSender(HassTopicKind.State).Dirty);
        Assert.False(entity.GetAttributesSender().Dirty);
    }

    [Fact]
    public async Task Combined_plan_reconnect_republishes_only_initialized_state()
    {
        FakeMqttClient client = new FakeMqttClient();
        HassMqttManager manager = CreateManager(client);
        IHassMqttEntity untouched = manager.CreateEntity<MqttSensor>()
            .ConfigureDevice(device => device.Identifiers.Add("weather-hardware"))
            .PublishStateAndAttributesTogether()
            .Build("weather", "untouched-combined");
        IHassMqttEntity initialized = manager.CreateEntity<MqttSensor>()
            .ConfigureDevice(device => device.Identifiers.Add("weather-hardware"))
            .PublishStateAndAttributesTogether()
            .Build("weather", "initialized-combined");
        initialized.SetValue(HassTopicKind.State, "ready");
        initialized.SetAttribute("quality", "good");
        await manager.FlushAll();
        client.Messages.Clear();

        manager.MarkAllValuesDirty();
        MqttFlushResult result = await manager.FlushAll();

        Assert.Equal(2, result.DiscoveryDocuments);
        Assert.Equal(1, result.Values);
        Assert.Equal(3, client.Messages.Count);
        Assert.DoesNotContain(client.Messages, message => message.Topic == untouched.GetValueSender(HassTopicKind.State).PublishTopic);
        Assert.Single(client.Messages, message => message.Topic == initialized.GetValueSender(HassTopicKind.State).PublishTopic);
    }

    [Fact]
    public async Task Combined_marker_is_idempotent_and_does_not_contaminate_builder_branches()
    {
        FakeMqttClient client = new FakeMqttClient();
        HassMqttManager manager = CreateManager(client);
        IEntityBuilder<MqttSensor> common = manager.CreateEntity<MqttSensor>()
            .ConfigureDevice(device => device.Identifiers.Add("weather-hardware"));
        IEntityBuilder<MqttSensor> combined = common
            .PublishStateAndAttributesTogether()
            .PublishStateAndAttributesTogether();
        IEntityBuilder<MqttSensor> separate = common.ConfigureTopics(HassTopicKind.State, HassTopicKind.JsonAttributes);

        combined.Build("weather", "combined-branch");
        separate.Build("weather", "separate-branch");
        await manager.FlushAll();

        JsonObject combinedDiscovery = DiscoveryPayload(client, "homeassistant/sensor/weather/combined-branch/config");
        JsonObject separateDiscovery = DiscoveryPayload(client, "homeassistant/sensor/weather/separate-branch/config");
        Assert.Equal((string)combinedDiscovery["state_topic"], (string)combinedDiscovery["json_attributes_topic"]);
        Assert.NotEqual((string)separateDiscovery["state_topic"], (string)separateDiscovery["json_attributes_topic"]);
        Assert.Null(separateDiscovery["value_template"]);
        Assert.Null(separateDiscovery["json_attributes_template"]);
    }

    [Fact]
    public async Task Combined_plan_interruption_leaves_both_sources_dirty()
    {
        FakeMqttClient client = new FakeMqttClient();
        HassMqttManager manager = CreateManager(client);
        IHassMqttEntity entity = manager.CreateEntity<MqttSensor>()
            .ConfigureDevice(device => device.Identifiers.Add("weather-hardware"))
            .PublishStateAndAttributesTogether()
            .Build("weather", "interrupted-combined");
        await manager.FlushAll();

        entity.SetValue(HassTopicKind.State, "pending");
        entity.SetAttribute("quality", "pending");
        using CancellationTokenSource cancellation = new CancellationTokenSource();
        client.PublishOverride = _ =>
        {
            cancellation.Cancel();
            return Task.FromCanceled<MqttClientPublishResult>(cancellation.Token);
        };

        await Assert.ThrowsAnyAsync<OperationCanceledException>(() => manager.FlushAll(cancellation.Token));

        Assert.True(entity.GetValueSender(HassTopicKind.State).Dirty);
        Assert.True(entity.GetAttributesSender().Dirty);
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

    private static JsonObject DiscoveryPayload(FakeMqttClient client, string topic)
    {
        MqttApplicationMessage message = Assert.Single(client.Messages, candidate => candidate.Topic == topic);
        return JsonNode.Parse(message.ConvertPayloadToString())!.AsObject();
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
