using JetBrains.Annotations;
using MBW.HassMQTT.DiscoveryModels.Enum;
using MBW.HassMQTT.DiscoveryModels.Metadata;

namespace MBW.HassMQTT.DiscoveryModels.Models
{
    /// <summary>
    /// https://www.home-assistant.io/integrations/device_trigger.mqtt/
    /// </summary>
    [DeviceType(HassDeviceType.DeviceTrigger)]
    [PublicAPI]
    public class MqttDeviceTrigger : MqttSensorDiscoveryBase
    {
        public MqttDeviceTrigger(string discoveryTopic, string uniqueId) : base(discoveryTopic, uniqueId)
        {
        }

        /// <summary>
        /// The type of automation, must be 'trigger'.
        /// </summary>
        public string AutomationType { get; set; }

        /// <summary>
        /// Optional payload to match the payload being sent over the topic.
        /// </summary>
        public string Payload { get; set; }

        /// <summary>
        /// The maximum QoS level to be used when receiving messages.
        /// </summary>
        public MqttQosLevel Qos { get; set; }

        /// <summary>
        /// The MQTT topic subscribed to receive trigger events.
        /// </summary>
        public string Topic { get; set; }

        /// <summary>
        /// The type of the trigger, e.g. `button_short_press`. Entries supported by the frontend: `button_short_press`, `button_short_release`, `button_long_press`, `button_long_release`, `button_double_press`, `button_triple_press`, `button_quadruple_press`, `button_quintuple_press`. If set to an unsupported value, will render as `subtype type`, e.g. `First button spammed` with `type` set to `spammed` and `subtype` set to `button_1`
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// The subtype of the trigger, e.g. `button_1`. Entries supported by the frontend: `turn_on`, `turn_off`, `button_1`, `button_2`, `button_3`, `button_4`, `button_5`, `button_6`. If set to an unsupported value, will render as `subtype type`, e.g. `left_button pressed` with `type` set to `button_short_press` and `subtype` set to `left_button`
        /// </summary>
        public string Subtype { get; set; }
    }
}