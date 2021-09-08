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
    /// https://www.home-assistant.io/integrations/lock.mqtt/
    ///
    /// The mqtt lock platform lets you control your MQTT enabled locks.
    /// </summary>
    /// <remarks>
    /// In an ideal scenario, the MQTT device will have a state_topic to publish state changes. If these messages
    /// are published with a RETAIN flag, the MQTT lock will receive an instant state update after subscription and
    /// will start with correct state. Otherwise, the initial state of the lock will be false / unlocked.
    /// 
    /// When a state_topic is not available, the lock will work in optimistic mode.In this mode, the lock will
    /// immediately change state after every command.Otherwise, the lock will wait for state confirmation from
    /// the device (message from state_topic).
    /// 
    /// Optimistic mode can be forced, even if state topic is available.Try to enable it, if experiencing incorrect
    /// lock operation.
    /// </remarks>
    [DeviceType(HassDeviceType.Lock)]
    [PublicAPI]
    public class MqttLock : MqttSensorDiscoveryBase<MqttLock, MqttLock.MqttLockValidator>, IHasUniqueId, IHasAvailability, IHasQos, IHasJsonAttributes, IHasIcon, IHasEnabledByDefault, IHasRetain
    {
        public MqttLock(string discoveryTopic, string uniqueId) : base(discoveryTopic, uniqueId)
        {
        }

        /// <summary>
        /// The MQTT topic to publish commands to change the lock state.
        /// </summary>
        public string CommandTopic { get; set; }

        /// <summary>
        /// The name of the lock.
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// Flag that defines if lock works in optimistic mode.
        /// </summary>
        public bool? Optimistic { get; set; }

        /// <summary>
        /// The payload that represents enabled/locked state.
        /// </summary>
        public string? PayloadLock { get; set; }

        /// <summary>
        /// The payload that represents disabled/unlocked state.
        /// </summary>
        public string? PayloadUnlock { get; set; }

        /// <summary>
        /// The value that represents the lock to be in locked state
        /// </summary>
        public string? StateLocked { get; set; }

        /// <summary>
        /// The MQTT topic subscribed to receive state updates.
        /// </summary>
        public string? StateTopic { get; set; }

        /// <summary>
        /// The value that represents the lock to be in unlocked state
        /// </summary>
        public string? StateUnlocked { get; set; }

        /// <summary>
        /// Defines a [template](/docs/configuration/templating/#processing-incoming-data) to extract a value from the payload.
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
        public bool? Retain { get; set; }

        public class MqttLockValidator : MqttSensorDiscoveryBaseValidator<MqttLock>
        {
            public MqttLockValidator()
            {
                TopicAndTemplate(s => s.StateTopic, s => s.ValueTemplate);
            }
        }
    }
}