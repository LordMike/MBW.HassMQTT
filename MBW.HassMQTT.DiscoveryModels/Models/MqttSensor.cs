﻿#nullable enable
using System.Collections.Generic;
using JetBrains.Annotations;
using MBW.HassMQTT.DiscoveryModels.Availability;
using MBW.HassMQTT.DiscoveryModels.Enum;
using MBW.HassMQTT.DiscoveryModels.Interfaces;
using MBW.HassMQTT.DiscoveryModels.Metadata;

namespace MBW.HassMQTT.DiscoveryModels.Models
{
    /// <summary>
    /// https://www.home-assistant.io/integrations/sensor.mqtt/
    ///
    /// This mqtt sensor platform uses the MQTT message payload as the sensor value. If messages in this state_topic
    /// are published with RETAIN flag, the sensor will receive an instant update with last known value. Otherwise,
    /// the initial state will be undefined.
    /// </summary>
    [DeviceType(HassDeviceType.Sensor)]
    [PublicAPI]
    public class MqttSensor : MqttSensorDiscoveryBase, IHasUniqueId, IHasAvailability, IHasQos, IHasJsonAttributes, IHasIcon, IHasEnabledByDefault
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
        /// Defines the number of seconds after the value expires if it's not updated.
        /// </summary>
        public int? ExpireAfter { get; set; }

        /// <summary>
        /// Sends update events even if the value hasn't changed. Useful if you want to have meaningful value graphs in history.
        /// </summary>
        public bool? ForceUpdate { get; set; }

        /// <summary>
        /// The MQTT topic subscribed to receive timestamps for when an accumulating sensor such as an energy meter was reset.
        /// If the sensor never resets, set last_reset_topic to same as state_topic and set the last_reset_value_template to a
        /// constant valid timstamp, for example UNIX epoch 0: 1970-01-01T00:00:00+00:00.
        /// </summary>
        public string? LastResetTopic { get; set; }

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
        public string? StateClass { get; set; }

        /// <summary>
        /// The MQTT topic subscribed to receive sensor values.
        /// </summary>
        public string StateTopic { get; set; }

        /// <summary>
        /// Defines the units of measurement of the sensor, if any.
        /// </summary>
        public string? UnitOfMeasurement { get; set; }

        /// <summary>
        /// Defines a [template](/docs/configuration/templating/#processing-incoming-data) to extract the value.
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
    }
}