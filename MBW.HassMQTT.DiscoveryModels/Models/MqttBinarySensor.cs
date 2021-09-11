#nullable enable
using System.Collections.Generic;
using FluentValidation;
using JetBrains.Annotations;
using MBW.HassMQTT.DiscoveryModels.Availability;
using MBW.HassMQTT.DiscoveryModels.Enum;
using MBW.HassMQTT.DiscoveryModels.Interfaces;
using MBW.HassMQTT.DiscoveryModels.Metadata;

namespace MBW.HassMQTT.DiscoveryModels.Models
{
    /// <summary>
    /// https://www.home-assistant.io/integrations/binary_sensor.mqtt/
    ///
    /// The mqtt binary sensor platform uses an MQTT message received to set the binary sensor’s state to on or off.
    /// 
    /// The state will be updated only after a new message is published on state_topic matching payload_on or payload_off.
    /// If these messages are published with the retain flag set, the binary sensor will receive an instant state update
    /// after subscription and Home Assistant will display the correct state on startup.Otherwise, the initial state
    /// displayed in Home Assistant will be unknown.
    /// 
    /// Stateless devices such as buttons, remote controls etc are better represented by MQTT device triggers than by
    /// binary sensors.
    /// </summary>
    /// <remarks>
    /// The mqtt binary sensor platform optionally supports a list of availability topics to receive online and offline
    /// messages (birth and LWT messages) from the MQTT device. During normal operation, if the MQTT sensor device goes
    /// offline (i.e., publishes payload_not_available to an availability topic), Home Assistant will display the binary
    /// sensor as unavailable. If these messages are published with the retain flag set, the binary sensor will receive
    /// an instant update after subscription and Home Assistant will display the correct availability state of the binary
    /// sensor when Home Assistant starts up. If the retain flag is not set, Home Assistant will display the binary sensor
    /// as unavailable when Home Assistant starts up. If no availability topic is defined, Home Assistant will consider
    /// the MQTT device to be available and will display its state.
    /// </remarks>
    [DeviceType(HassDeviceType.BinarySensor)]
    [PublicAPI]
    public class MqttBinarySensor : MqttSensorDiscoveryBase<MqttBinarySensor, MqttBinarySensor.MqttBinarySensorValidator>, IHasUniqueId, IHasAvailability, IHasQos, IHasJsonAttributes, IHasIcon, IHasEnabledByDefault
    {
        public MqttBinarySensor(string discoveryTopic, string uniqueId) : base(discoveryTopic, uniqueId)
        {
        }

        /// <summary>
        /// Sets the [class of the device](/integrations/binary_sensor/#device-class), changing the device state and icon that is displayed on the frontend.
        /// See https://www.home-assistant.io/integrations/binary_sensor/#device-class
        /// </summary>
        public HassBinarySensorDeviceClass? DeviceClass { get; set; }

        /// <summary>
        /// Defines the number of seconds after the sensor's state expires if it's not updated. After expiry, the sensor's state becomes `unavailable` if `availability_topic` is defined and `unknown` otherwise.
        /// </summary>
        public int? ExpireAfter { get; set; }

        /// <summary>
        /// Sends update events (which results in update of [state object](/docs/configuration/state_object/)'s `last_changed`) even if the sensor's state hasn't changed. Useful if you want to have meaningful value graphs in history or want to create an automation that triggers on *every* incoming state message (not only when the sensor's new state is different to the current one).
        /// </summary>
        public bool? ForceUpdate { get; set; }

        /// <summary>
        /// The name of the binary sensor.
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// For sensors that only send `on` state updates (like PIRs), this variable sets a delay in seconds after which the sensor's state will be updated back to `off`.
        /// </summary>
        public int? OffDelay { get; set; }

        /// <summary>
        /// The string that represents the `off` state. It will be compared to the message in the `state_topic` (see `value_template` for details)
        /// </summary>
        public string? PayloadOff { get; set; }

        /// <summary>
        /// The string that represents the `on` state. It will be compared to the message in the `state_topic` (see `value_template` for details)
        /// </summary>
        public string? PayloadOn { get; set; }

        /// <summary>
        /// The MQTT topic subscribed to receive sensor's state.
        /// </summary>
        public string StateTopic { get; set; }

        /// <summary>
        /// Defines a [template](/docs/configuration/templating/#processing-incoming-data) that returns a string to be compared to `payload_on`/`payload_off`. Available variables: `entity_id`. Remove this option when 'payload_on' and 'payload_off' are sufficient to match your payloads (i.e no pre-processing of original message is required).
        /// </summary>
        public string? ValueTemplate { get; set; }

        public string? UniqueId { get; set; }
        public IList<AvailabilityModel>? Availability { get; set; }
        public AvailabilityMode? AvailabilityMode { get; set; }
        public MqttQosLevel? Qos { get; set; }
        public string? JsonAttributesTemplate { get; set; }
        public string? JsonAttributesTopic { get; set; }
        public string? Icon { get; set; }
        public bool? EnabledByDefault { get; set; }

        public class MqttBinarySensorValidator : MqttSensorDiscoveryBaseValidator<MqttBinarySensor>
        {
            public MqttBinarySensorValidator()
            {
                TopicAndTemplate(s => s.StateTopic, s => s.ValueTemplate);

                RuleFor(s => s.ExpireAfter).GreaterThanOrEqualTo(0);

                RuleFor(s => s.DeviceClass).IsInEnum();
            }
        }
    }
}