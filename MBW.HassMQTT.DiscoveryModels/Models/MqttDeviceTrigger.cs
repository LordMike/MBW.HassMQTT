using System;
using MBW.HassMQTT.DiscoveryModels.Enum;

namespace MBW.HassMQTT.DiscoveryModels.Models
{
    /// <summary>
    /// https://www.home-assistant.io/integrations/device_trigger.mqtt/
    /// </summary>
    [Obsolete("Needs to be re-implemented")]
    [DeviceType(HassDeviceType.DeviceTrigger)]
    public class MqttDeviceTrigger : MqttSensorDiscoveryBase
    {
        public MqttDeviceTrigger(string topic, string uniqueId) : base(topic, uniqueId)
        {
            // This is an odd one, it doesn't behave like other components. 

            throw new NotImplementedException();
        }

        /// <summary>
        /// The type of automation, must be 'trigger'.
        /// </summary>
        public string AutomationType
        {
            get => GetValue<string>("automation_type", default);
            set => SetValue("automation_type", value);
        }

        /// <summary>
        /// Optional payload to match the payload being sent over the topic.
        /// </summary>
        public string Payload
        {
            get => GetValue<string>("payload", default);
            set => SetValue("payload", value);
        }

        /// <summary>
        /// The maximum QoS level to be used when receiving messages.
        /// </summary>
        public int Qos
        {
            get => GetValue<int>("qos", default);
            set => SetValue("qos", value);
        }

        /// <summary>
        /// The MQTT topic subscribed to receive trigger events.
        /// </summary>
        public string Topic
        {
            get => GetValue<string>("topic", default);
            set => SetValue("topic", value);
        }

        /// <summary>
        /// The type of the trigger, e.g. `button_short_press`. Entries supported by the frontend: `button_short_press`, `button_short_release`, `button_long_press`, `button_long_release`, `button_double_press`, `button_triple_press`, `button_quadruple_press`, `button_quintuple_press`. If set to an unsupported value, will render as `subtype type`, e.g. `First button spammed` with `type` set to `spammed` and `subtype` set to `button_1`
        /// </summary>
        public string Type
        {
            get => GetValue<string>("type", default);
            set => SetValue("type", value);
        }

        /// <summary>
        /// The subtype of the trigger, e.g. `button_1`. Entries supported by the frontend: `turn_on`, `turn_off`, `button_1`, `button_2`, `button_3`, `button_4`, `button_5`, `button_6`. If set to an unsupported value, will render as `subtype type`, e.g. `left_button pressed` with `type` set to `button_short_press` and `subtype` set to `left_button`
        /// </summary>
        public string Subtype
        {
            get => GetValue<string>("subtype", default);
            set => SetValue("subtype", value);
        }
    }
}