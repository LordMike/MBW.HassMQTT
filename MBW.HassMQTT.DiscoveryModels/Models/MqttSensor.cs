#nullable enable
using System.Collections.Generic;
using FluentValidation;
using JetBrains.Annotations;
using MBW.HassMQTT.DiscoveryModels.Availability;
using MBW.HassMQTT.DiscoveryModels.Enum;
using MBW.HassMQTT.DiscoveryModels.Interfaces;
using MBW.HassMQTT.DiscoveryModels.Metadata;

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
public class MqttSensor : MqttSensorDiscoveryBase<MqttSensor, MqttSensor.MqttSensorValidator>, IHasUniqueId, IHasAvailability, IHasQos, IHasJsonAttributes, IHasIcon, IHasEnabledByDefault, IHasEntityCategory, IHasObjectId, IHasEncoding
{
    public MqttSensor(string discoveryTopic, string uniqueId) : base(discoveryTopic, uniqueId)
    {
    }

    /// <summary>
    /// The [type/class](/integrations/sensor/#device-class) of the sensor to set the icon in the frontend.
    /// See https://www.home-assistant.io/integrations/sensor/#device-class
    /// </summary>
    public HassSensorDeviceClass? DeviceClass { get; set; }

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
    /// The name of the MQTT sensor.
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// The state_class of the sensor.
    /// See https://developers.home-assistant.io/docs/core/entity/sensor/#available-state-classes
    /// </summary>
    public HassStateClass? StateClass { get; set; }

    /// <summary>
    /// The MQTT topic subscribed to receive sensor values.
    /// </summary>
    public string StateTopic { get; set; }

    /// <summary>
    /// Defines the units of measurement of the sensor, if any.
    /// </summary>
    public string? UnitOfMeasurement { get; set; }

    /// <summary>
    /// Defines a [template](/docs/configuration/templating/#using-templates-with-the-mqtt-integration) to extract the value. Available variables: `entity_id`. The `entity_id` can be used to reference the entity's attributes. If the template throws an error, the current state will be used instead.
    /// </summary>
    public string? ValueTemplate { get; set; }

    public string? JsonAttributesTemplate { get; set; }
    public string? JsonAttributesTopic { get; set; }
    public IList<AvailabilityModel>? Availability { get; set; }
    public AvailabilityMode? AvailabilityMode { get; set; }
    public string? UniqueId { get; set; }
    public MqttQosLevel? Qos { get; set; }
    public string? Icon { get; set; }
    public bool? EnabledByDefault { get; set; }
    public EntityCategory? EntityCategory { get; set; }
    public string? ObjectId { get; set; }
    public string? Encoding { get; set; }

    public class MqttSensorValidator : MqttSensorDiscoveryBaseValidator<MqttSensor>
    {
        public MqttSensorValidator()
        {
            TopicAndTemplate(s => s.StateTopic, s => s.ValueTemplate);

            RuleFor(s => s.DeviceClass).IsInEnum();
            RuleFor(s => s.StateClass).IsInEnum().NotEqual(HassStateClass.Unknown);

            RuleFor(s => s.ExpireAfter)
                .GreaterThanOrEqualTo(0);
        }
    }
}