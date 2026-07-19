#nullable enable
using System.Collections.Generic;
using FluentValidation;
using JetBrains.Annotations;
using MBW.HassMQTT.DiscoveryModels.Availability;
using MBW.HassMQTT.DiscoveryModels.Enum;
using MBW.HassMQTT.DiscoveryModels.Interfaces;
using MBW.HassMQTT.DiscoveryModels.Metadata;
using MBW.HassMQTT.DiscoveryModels.Validation;

namespace MBW.HassMQTT.DiscoveryModels.Models;

/// <summary>
/// https://www.home-assistant.io/integrations/sensor.mqtt/
///
/// This mqtt sensor platform uses the MQTT message payload as the sensor value. If messages in this state_topic
/// are published with RETAIN flag, the sensor will receive an instant update with last known value. Otherwise,
/// the initial state will be undefined.
/// </summary>
[DeviceType(HassDeviceType.Sensor)]
[PublicAPI]
public class MqttSensor : MqttSensorDiscoveryBase<MqttSensor, MqttSensor.MqttSensorValidator>, IHasUniqueId,
    IHasAvailability, IHasAvailabilityPayloads, IHasQos, IHasJsonAttributes, IHasIcon, IHasEnabledByDefault, IHasEntityCategory, IHasDefaultEntityId, IHasEntityPicture, IHasVisibleByDefault, IHasGroup,
    IHasEncoding, IHasName
{
    public MqttSensor(string discoveryTopic, string uniqueId) : base(discoveryTopic, uniqueId)
    {
    }

    /// <summary>
    /// The [type/class](/integrations/sensor/#device-class) of the sensor to set the icon in the frontend.
    /// See https://www.home-assistant.io/integrations/sensor/#device-class
    /// </summary>
    public Optional<HassSensorDeviceClass?> DeviceClass { get; set; }

    /// <summary>
    /// If set, it defines the number of seconds after the sensor's state expires, if it's not updated. After expiry, the sensor's state becomes `unavailable`. Default the sensors state never expires.
    /// </summary>
    public int? ExpireAfter { get; set; }

    /// <summary>
    /// Sends update events even if the value hasn't changed. Useful if you want to have meaningful value graphs in history.
    /// </summary>
    public bool? ForceUpdate { get; set; }

    /// <summary>
    /// Defines a template to extract the last_reset. Available variables: entity_id. The entity_id can be used to reference the
    /// entity’s attributes.
    /// </summary>
    public string? LastResetValueTemplate { get; set; }

    /// <summary>
    /// The allowed state values for an enum sensor. Cannot be empty or be used with a state class or unit.
    /// </summary>
    public IList<string>? Options { get; set; }

    /// <summary>
    /// The state_class of the sensor.
    /// See https://developers.home-assistant.io/docs/core/entity/sensor/#available-state-classes
    /// </summary>
    public HassStateClass? StateClass { get; set; }

    /// <summary>
    /// The number of decimals which should be used in the sensor's state after rounding.
    /// </summary>
    public int? SuggestedDisplayPrecision { get; set; }

    /// <summary>
    /// The MQTT topic subscribed to receive sensor values.
    /// </summary>
    public string StateTopic { get; set; }

    /// <summary>
    /// Defines the units of measurement of the sensor, if any.
    /// </summary>
    public Optional<string?> UnitOfMeasurement { get; set; }

    /// <summary>
    /// Defines a [template](/docs/configuration/templating/#using-templates-with-the-mqtt-integration) to extract the value. If the template throws an error, the current state will be used instead.
    /// </summary>
    public string? ValueTemplate { get; set; }

    /// <inheritdoc />
    public string? JsonAttributesTemplate { get; set; }
    /// <inheritdoc />
    public string? JsonAttributesTopic { get; set; }
    /// <inheritdoc />
    public IList<AvailabilityModel>? Availability { get; set; }
    /// <inheritdoc />
    public AvailabilityMode? AvailabilityMode { get; set; }
    /// <inheritdoc />
    public string? AvailabilityTemplate { get; set; }
    /// <inheritdoc />
    public string? AvailabilityTopic { get; set; }
    /// <inheritdoc />
    public string? PayloadAvailable { get; set; }
    /// <inheritdoc />
    public string? PayloadNotAvailable { get; set; }
    /// <inheritdoc />
    public string? UniqueId { get; set; }
    /// <inheritdoc />
    public MqttQosLevel? Qos { get; set; }
    /// <inheritdoc />
    public string? Icon { get; set; }
    /// <inheritdoc />
    public bool? EnabledByDefault { get; set; }
    /// <inheritdoc />
    public EntityCategory? EntityCategory { get; set; }
    /// <inheritdoc />
    public string? DefaultEntityId { get; set; }
    /// <inheritdoc />
    public string? EntityPicture { get; set; }
    /// <inheritdoc />
    public bool? VisibleByDefault { get; set; }
    /// <inheritdoc />
    public IList<string>? Group { get; set; }
    /// <inheritdoc />
    public string? Encoding { get; set; }
    /// <inheritdoc />
    public Optional<string?> Name { get; set; }

    public class MqttSensorValidator : MqttSensorDiscoveryBaseValidator<MqttSensor>
    {
        public MqttSensorValidator()
        {
            TopicAndTemplate(s => s.StateTopic, s => s.ValueTemplate);

            RuleFor(s => s.DeviceClass).IsInEnumWhenSet();
            RuleFor(s => s.StateClass).IsInEnum().NotEqual(HassStateClass.Unknown);

            RuleFor(s => s.ExpireAfter)
                .GreaterThanOrEqualTo(0);

            RuleFor(s => s.SuggestedDisplayPrecision)
                .GreaterThanOrEqualTo(0);

            RuleFor(s => s.Options)
                .NotEmpty()
                .When(s => s.Options != null);

            RuleFor(s => s.DeviceClass)
                .Must(optional => optional.IsSet && optional.Value == HassSensorDeviceClass.Enum)
                .When(s => s.Options != null)
                .WithMessage("DeviceClass must be Enum when Options is configured");

            RuleFor(s => s.StateClass)
                .Null()
                .When(s => s.Options != null)
                .WithMessage("StateClass cannot be used together with Options");

            RuleFor(s => s.UnitOfMeasurement)
                .Must(optional => !optional.IsSet || optional.Value == null)
                .When(s => s.Options != null)
                .WithMessage("UnitOfMeasurement cannot be used together with Options");
        }
    }
}
