using JetBrains.Annotations;
using MBW.HassMQTT.DiscoveryModels.Enum;
using MBW.HassMQTT.DiscoveryModels.Metadata;

namespace MBW.HassMQTT.DiscoveryModels.Models
{
    /// <summary>
    /// https://www.home-assistant.io/integrations/switch.mqtt
    /// </summary>
    [DeviceType(HassDeviceType.Switch)]
    [PublicAPI]
    public class MqttSwitch : MqttEntitySensorDiscoveryBase
    {
        public MqttSwitch(string discoveryTopic, string uniqueId) : base(discoveryTopic, uniqueId)
        {
        }

        /// <summary>
        /// The MQTT topic to publish commands to change the switch state.
        /// </summary>
        public string CommandTopic { get; set; }

        /// <summary>
        /// Icon for the switch.
        /// </summary>
        public string Icon { get; set; }

        /// <summary>
        /// The name to use when displaying this switch.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Flag that defines if switch works in optimistic mode.
        /// </summary>
        public bool Optimistic { get; set; }

        /// <summary>
        /// The payload that represents `off` state. If specified, will be used for both comparing to the value in the `state_topic` (see `value_template` and `state_off` for details) and sending as `off` command to the `command_topic`.
        /// </summary>
        public string PayloadOff { get; set; }

        /// <summary>
        /// The payload that represents `on` state. If specified, will be used for both comparing to the value in the `state_topic` (see `value_template` and `state_on`  for details) and sending as `on` command to the `command_topic`.
        /// </summary>
        public string PayloadOn { get; set; }

        /// <summary>
        /// The maximum QoS level of the state topic. Default is 0 and will also be used to publishing messages.
        /// </summary>
        public MqttQosLevel Qos { get; set; }

        /// <summary>
        /// If the published message should have the retain flag on or not.
        /// </summary>
        public bool Retain { get; set; }

        /// <summary>
        /// The payload that represents the `off` state. Used when value that represents `off` state in the `state_topic` is different from value that should be sent to the `command_topic` to turn the device `off`.
        /// </summary>
        public string StateOff { get; set; }

        /// <summary>
        /// The payload that represents the `on` state. Used when value that represents `on` state in the `state_topic` is different from value that should be sent to the `command_topic` to turn the device `on`.
        /// </summary>
        public string StateOn { get; set; }

        /// <summary>
        /// The MQTT topic subscribed to receive state updates.
        /// </summary>
        public string StateTopic { get; set; }

        /// <summary>
        /// Defines a [template](/docs/configuration/templating/#processing-incoming-data) to extract device's state from the `state_topic`. To determine the switches's state result of this template will be compared to `state_on` and `state_off`.
        /// </summary>
        public string ValueTemplate { get; set; }
    }
}