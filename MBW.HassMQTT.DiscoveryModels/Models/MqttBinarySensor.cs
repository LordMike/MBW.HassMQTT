using EnumsNET;
using MBW.HassMQTT.DiscoveryModels.Enum;

namespace MBW.HassMQTT.DiscoveryModels.Models
{
    /// <summary>
    /// https://www.home-assistant.io/integrations/binary_sensor.mqtt/
    /// </summary>
    [DeviceType(HassDeviceType.BinarySensor)]
    public class MqttBinarySensor : MqttEntitySensorDiscoveryBase
    {
        public MqttBinarySensor(string topic, string uniqueId) : base(topic, uniqueId)
        {
        }

        /// <summary>
        /// Sets the [class of the device](/integrations/binary_sensor/#device-class), changing the device state and icon that is displayed on the frontend.
        /// </summary>
        public HassDeviceClass DeviceClass
        {
            get => Enums.Parse<HassDeviceClass>(GetValue<string>("device_class", default), true, EnumFormat.EnumMemberValue);
            set => SetValue("device_class", value.AsString(EnumFormat.EnumMemberValue));
        }

        /// <summary>
        /// Defines the number of seconds after the sensor's state expires if it's not updated. After expiry, the sensor's state becomes `unavailable` if `availability_topic` is defined and `unknown` otherwise.
        /// </summary>
        public int ExpireAfter
        {
            get => GetValue<int>("expire_after", default);
            set => SetValue("expire_after", value);
        }

        /// <summary>
        /// Sends update events (which results in update of [state object](/docs/configuration/state_object/)'s `last_changed`) even if the sensor's state hasn't changed. Useful if you want to have meaningful value graphs in history or want to create an automation that triggers on *every* incoming state message (not only when the sensor's new state is different to the current one).
        /// </summary>
        public bool ForceUpdate
        {
            get => GetValue<bool>("force_update", default);
            set => SetValue("force_update", value);
        }

        /// <summary>
        /// Defines a [template](/docs/configuration/templating/#processing-incoming-data) to extract the JSON dictionary from messages received on the `json_attributes_topic`. Usage example can be found in [MQTT sensor](/integrations/sensor.mqtt/#json-attributes-template-configuration) documentation.
        /// </summary>
        public string JsonAttributesTemplate
        {
            get => GetValue<string>("json_attributes_template", default);
            set => SetValue("json_attributes_template", value);
        }

        /// <summary>
        /// The MQTT topic subscribed to receive a JSON dictionary payload and then set as sensor attributes. Usage example can be found in [MQTT sensor](/integrations/sensor.mqtt/#json-attributes-topic-configuration) documentation.
        /// </summary>
        public string JsonAttributesTopic
        {
            get => GetValue<string>("json_attributes_topic", default);
            set => SetValue("json_attributes_topic", value);
        }

        /// <summary>
        /// The name of the binary sensor.
        /// </summary>
        public string Name
        {
            get => GetValue<string>("name", default);
            set => SetValue("name", value);
        }

        /// <summary>
        /// For sensors that only send `on` state updates (like PIRs), this variable sets a delay in seconds after which the sensor's state will be updated back to `off`.
        /// </summary>
        public int OffDelay
        {
            get => GetValue<int>("off_delay", default);
            set => SetValue("off_delay", value);
        }

        /// <summary>
        /// The string that represents the `off` state. It will be compared to the message in the `state_topic` (see `value_template` for details)
        /// </summary>
        public string PayloadOff
        {
            get => GetValue<string>("payload_off", default);
            set => SetValue("payload_off", value);
        }

        /// <summary>
        /// The string that represents the `on` state. It will be compared to the message in the `state_topic` (see `value_template` for details)
        /// </summary>
        public string PayloadOn
        {
            get => GetValue<string>("payload_on", default);
            set => SetValue("payload_on", value);
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
        /// The MQTT topic subscribed to receive sensor's state.
        /// </summary>
        public string StateTopic
        {
            get => GetValue<string>("state_topic", default);
            set => SetValue("state_topic", value);
        }

        /// <summary>
        /// Defines a [template](/docs/configuration/templating/#processing-incoming-data) that returns a string to be compared to `payload_on`/`payload_off`. Available variables: `entity_id`. Remove this option when 'payload_on' and 'payload_off' are sufficient to match your payloads (i.e no pre-processing of original message is required).
        /// </summary>
        public string ValueTemplate
        {
            get => GetValue<string>("value_template", default);
            set => SetValue("value_template", value);
        }
    }
}