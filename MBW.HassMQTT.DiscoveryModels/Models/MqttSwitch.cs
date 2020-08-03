using JetBrains.Annotations;
using MBW.HassMQTT.DiscoveryModels.Enum;
using MBW.HassMQTT.DiscoveryModels.Metadata;

namespace MBW.HassMQTT.DiscoveryModels.Models
{
    /// <summary>
    /// https://www.home-assistant.io/integrations/switch.mqtt
    /// </summary>
    [DeviceType(HassDeviceType.Switch)]
    public class MqttSwitch : MqttEntitySensorDiscoveryBase
    {
        public MqttSwitch(string discoveryTopic, string uniqueId) : base(discoveryTopic, uniqueId)
        {
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
        /// Flag that defines if switch works in optimistic mode.
        /// </summary>
        [PublicAPI]
        public bool Optimistic
        {
            get => GetValue<bool>("optimistic", default);
            set => SetValue("optimistic", value);
        }

        /// <summary>
        /// The payload that represents `off` state. If specified, will be used for both comparing to the value in the `state_topic` (see `value_template` and `state_off` for details) and sending as `off` command to the `command_topic`.
        /// </summary>
        [PublicAPI]
        public string PayloadOff
        {
            get => GetValue<string>("payload_off", default);
            set => SetValue("payload_off", value);
        }

        /// <summary>
        /// The payload that represents `on` state. If specified, will be used for both comparing to the value in the `state_topic` (see `value_template` and `state_on`  for details) and sending as `on` command to the `command_topic`.
        /// </summary>
        [PublicAPI]
        public string PayloadOn
        {
            get => GetValue<string>("payload_on", default);
            set => SetValue("payload_on", value);
        }

        /// <summary>
        /// The maximum QoS level of the state topic. Default is 0 and will also be used to publishing messages.
        /// </summary>
        [PublicAPI]
        public int Qos
        {
            // TODO: Move to base class
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
        /// The payload that represents the `off` state. Used when value that represents `off` state in the `state_topic` is different from value that should be sent to the `command_topic` to turn the device `off`.
        /// </summary>
        [PublicAPI]
        public string StateOff
        {
            get => GetValue<string>("state_off", default);
            set => SetValue("state_off", value);
        }

        /// <summary>
        /// The payload that represents the `on` state. Used when value that represents `on` state in the `state_topic` is different from value that should be sent to the `command_topic` to turn the device `on`.
        /// </summary>
        [PublicAPI]
        public string StateOn
        {
            get => GetValue<string>("state_on", default);
            set => SetValue("state_on", value);
        }

        /// <summary>
        /// The MQTT topic subscribed to receive state updates.
        /// </summary>
        [PublicAPI]
        public string StateTopic
        {
            // TODO: Move to base class
            get => GetValue<string>("state_topic", default);
            set => SetValue("state_topic", value);
        }

        /// <summary>
        /// Defines a [template](/docs/configuration/templating/#processing-incoming-data) to extract device's state from the `state_topic`. To determine the switches's state result of this template will be compared to `state_on` and `state_off`.
        /// </summary>
        [PublicAPI]
        public string ValueTemplate
        {
            get => GetValue<string>("value_template", default);
            set => SetValue("value_template", value);
        }
    }
}