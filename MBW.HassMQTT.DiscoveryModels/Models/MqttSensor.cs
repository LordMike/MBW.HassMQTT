using EnumsNET;
using MBW.HassMQTT.DiscoveryModels.Enum;

namespace MBW.HassMQTT.DiscoveryModels.Models
{
    /// <summary>
    /// https://www.home-assistant.io/integrations/sensor.mqtt/
    /// </summary>
    [DeviceType(HassDeviceType.Sensor)]
    public class MqttSensor : MqttEntitySensorDiscoveryBase
    {
        public MqttSensor(string topic, string uniqueId) : base(topic, uniqueId)
        {
        }

        /// <summary>
        /// The [type/class](/integrations/sensor/#device-class) of the sensor to set the icon in the frontend.
        /// </summary>
        public HassDeviceClass DeviceClass
        {
            get => Enums.Parse<HassDeviceClass>(GetValue<string>("device_class", default), true, EnumFormat.EnumMemberValue);
            set => SetValue("device_class", value.AsString(EnumFormat.EnumMemberValue));
        }

        /// <summary>
        /// Defines the number of seconds after the value expires if it's not updated.
        /// </summary>
        public int ExpireAfter
        {
            get => GetValue<int>("expire_after", default);
            set => SetValue("expire_after", value);
        }

        /// <summary>
        /// Sends update events even if the value hasn't changed. Useful if you want to have meaningful value graphs in history.
        /// </summary>
        public bool ForceUpdate
        {
            get => GetValue<bool>("force_update", default);
            set => SetValue("force_update", value);
        }

        /// <summary>
        /// The icon for the sensor.
        /// </summary>
        public string Icon
        {
            get => GetValue<string>("icon", default);
            set => SetValue("icon", value);
        }

        /// <summary>
        /// Defines a [template](/docs/configuration/templating/#processing-incoming-data) to extract the JSON dictionary from messages received on the `json_attributes_topic`.
        /// </summary>
        public string JsonAttributesTemplate
        {
            get => GetValue<string>("json_attributes_template", default);
            set => SetValue("json_attributes_template", value);
        }

        /// <summary>
        /// The MQTT topic subscribed to receive a JSON dictionary payload and then set as sensor attributes. Implies `force_update` of the current sensor state when a message is received on this topic.
        /// </summary>
        public string JsonAttributesTopic
        {
            get => GetValue<string>("json_attributes_topic", default);
            set => SetValue("json_attributes_topic", value);
        }

        /// <summary>
        /// The name of the MQTT sensor.
        /// </summary>
        public string Name
        {
            get => GetValue<string>("name", default);
            set => SetValue("name", value);
        }

        /// <summary>
        /// The maximum QoS level of the state topic.
        /// </summary>
        public int Qos
        {
            get => GetValue<int>("qos", default);
            set => SetValue("qos", value);
        }

        /// <summary>
        /// The MQTT topic subscribed to receive sensor values.
        /// </summary>
        public string StateTopic
        {
            get => GetValue<string>("state_topic", default);
            set => SetValue("state_topic", value);
        }

        /// <summary>
        /// Defines the units of measurement of the sensor, if any.
        /// </summary>
        public string UnitOfMeasurement
        {
            get => GetValue<string>("unit_of_measurement", default);
            set => SetValue("unit_of_measurement", value);
        }

        /// <summary>
        /// Defines a [template](/docs/configuration/templating/#processing-incoming-data) to extract the value.
        /// </summary>
        public string ValueTemplate
        {
            get => GetValue<string>("value_template", default);
            set => SetValue("value_template", value);
        }
    }
}