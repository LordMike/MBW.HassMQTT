using JetBrains.Annotations;
using MBW.HassMQTT.DiscoveryModels.Enum;
using MBW.HassMQTT.DiscoveryModels.Interfaces;
using MBW.HassMQTT.DiscoveryModels.Metadata;

namespace MBW.HassMQTT.DiscoveryModels.Models
{
    /// <summary>
    /// https://www.home-assistant.io/integrations/scene.mqtt/
    /// </summary>
    [DeviceType(HassDeviceType.Scene)]
    public class MqttScene : MqttSensorDiscoveryBase, IHasAvailabilityTopic
    {
        public MqttScene(string discoveryTopic, string uniqueId) : base(discoveryTopic, uniqueId)
        {
        }
        
        /// <inheritdoc cref="IHasAvailabilityTopic.AvailabilityTopic"/>
        public string AvailabilityTopic
        {
            get => GetValue<string>("availability_topic", default);
            set => SetValue("availability_topic", value);
        }

        /// <inheritdoc cref="IHasAvailabilityTopic.PayloadAvailable"/>
        public string PayloadAvailable
        {
            get => GetValue<string>("payload_available", default);
            set => SetValue("payload_available", value);
        }

        /// <inheritdoc cref="IHasAvailabilityTopic.PayloadNotAvailable"/>
        public string PayloadNotAvailable
        {
            get => GetValue<string>("payload_not_available", default);
            set => SetValue("payload_not_available", value);
        }

        /// <summary>
        /// The MQTT topic to publish commands to change the switch state.
        /// </summary>
        [PublicAPI]
        public string CommandTopic
        {
            get => GetValue<string>("command_topic", default);
            set => SetValue("command_topic", value);
        }

        /// <summary>
        /// Icon for the switch.
        /// </summary>
        [PublicAPI]
        public string Icon
        {
            get => GetValue<string>("icon", default);
            set => SetValue("icon", value);
        }

        /// <summary>
        /// The name to use when displaying this switch.
        /// </summary>
        [PublicAPI]
        public string Name
        {
            get => GetValue<string>("name", default);
            set => SetValue("name", value);
        }

        /// <summary>
        /// The payload that represents on state.
        /// If specified, will be used for both comparing to the value in the state_topic (see value_template and state_on for details) and sending as on command to the command_topic.
        /// </summary>
        [PublicAPI]
        public string PayloadOn
        {
            get => GetValue<string>("payload_on", default);
            set => SetValue("payload_on", value);
        }

        /// <summary>
        /// The maximum QoS level to be used when receiving messages.
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
    }
}