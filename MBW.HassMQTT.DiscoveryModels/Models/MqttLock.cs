using JetBrains.Annotations;
using MBW.HassMQTT.DiscoveryModels.Enum;
using MBW.HassMQTT.DiscoveryModels.Metadata;

namespace MBW.HassMQTT.DiscoveryModels.Models
{
    /// <summary>
    /// https://www.home-assistant.io/integrations/lock.mqtt/
    /// </summary>
    [DeviceType(HassDeviceType.Lock)]
    public class MqttLock : MqttEntitySensorDiscoveryBase
    {
        public MqttLock(string discoveryTopic, string uniqueId) : base(discoveryTopic, uniqueId)
        {
        }

        /// <summary>
        /// The MQTT topic to publish commands to change the lock state.
        /// </summary>
        [PublicAPI]
        public string CommandTopic
        {
            get => GetValue<string>("command_topic", default);
            set => SetValue("command_topic", value);
        }

        /// <summary>
        /// The name of the lock.
        /// </summary>
        [PublicAPI]
        public string Name
        {
            get => GetValue<string>("name", default);
            set => SetValue("name", value);
        }

        /// <summary>
        /// Flag that defines if lock works in optimistic mode.
        /// </summary>
        [PublicAPI]
        public string Optimistic
        {
            get => GetValue<string>("optimistic", default);
            set => SetValue("optimistic", value);
        }

        /// <summary>
        /// The payload that represents enabled/locked state.
        /// </summary>
        [PublicAPI]
        public string PayloadLock
        {
            get => GetValue<string>("payload_lock", default);
            set => SetValue("payload_lock", value);
        }

        /// <summary>
        /// The payload that represents disabled/unlocked state.
        /// </summary>
        [PublicAPI]
        public string PayloadUnlock
        {
            get => GetValue<string>("payload_unlock", default);
            set => SetValue("payload_unlock", value);
        }

        /// <summary>
        /// The maximum QoS level of the state topic.
        /// </summary>
        [PublicAPI]
        public int Qos
        {
            get => GetValue<int>("qos", default);
            set => SetValue("qos", value);
        }

        /// <summary>
        /// If the published message should have the retain flag on or not.
        /// </summary>
        [PublicAPI]
        public bool Retain
        {
            get => GetValue<bool>("retain", default);
            set => SetValue("retain", value);
        }

        /// <summary>
        /// The value that represents the lock to be in locked state
        /// </summary>
        [PublicAPI]
        public string StateLocked
        {
            get => GetValue<string>("state_locked", default);
            set => SetValue("state_locked", value);
        }

        /// <summary>
        /// The MQTT topic subscribed to receive state updates.
        /// </summary>
        [PublicAPI]
        public string StateTopic
        {
            get => GetValue<string>("state_topic", default);
            set => SetValue("state_topic", value);
        }

        /// <summary>
        /// The value that represents the lock to be in unlocked state
        /// </summary>
        [PublicAPI]
        public string StateUnlocked
        {
            get => GetValue<string>("state_unlocked", default);
            set => SetValue("state_unlocked", value);
        }

        /// <summary>
        /// Defines a [template](/docs/configuration/templating/#processing-incoming-data) to extract a value from the payload.
        /// </summary>
        [PublicAPI]
        public string ValueTemplate
        {
            get => GetValue<string>("value_template", default);
            set => SetValue("value_template", value);
        }
    }
}