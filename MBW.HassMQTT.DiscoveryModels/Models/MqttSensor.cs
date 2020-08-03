using EnumsNET;
using JetBrains.Annotations;
using MBW.HassMQTT.DiscoveryModels.Enum;
using MBW.HassMQTT.DiscoveryModels.Metadata;

namespace MBW.HassMQTT.DiscoveryModels.Models
{
    /// <summary>
    /// https://www.home-assistant.io/integrations/sensor.mqtt/
    /// </summary>
    [DeviceType(HassDeviceType.Sensor)]
    public class MqttSensor : MqttEntitySensorDiscoveryBase
    {
        public MqttSensor(string discoveryTopic, string uniqueId) : base(discoveryTopic, uniqueId)
        {
        }

        /// <summary>
        /// The [type/class](/integrations/sensor/#device-class) of the sensor to set the icon in the frontend.
        /// </summary>
        [PublicAPI]
        public HassDeviceClass DeviceClass
        {
            get => Enums.Parse<HassDeviceClass>(GetValue<string>("device_class", default), true,
                EnumFormat.EnumMemberValue);
            set => SetValue("device_class", value.AsString(EnumFormat.EnumMemberValue));
        }

        /// <summary>
        /// Defines the number of seconds after the value expires if it's not updated.
        /// </summary>
        [PublicAPI]
        public int ExpireAfter
        {
            get => GetValue<int>("expire_after", default);
            set => SetValue("expire_after", value);
        }

        /// <summary>
        /// Sends update events even if the value hasn't changed. Useful if you want to have meaningful value graphs in history.
        /// </summary>
        [PublicAPI]
        public bool ForceUpdate
        {
            get => GetValue<bool>("force_update", default);
            set => SetValue("force_update", value);
        }

        /// <summary>
        /// The icon for the sensor.
        /// </summary>
        [PublicAPI]
        public string Icon
        {
            get => GetValue<string>("icon", default);
            set => SetValue("icon", value);
        }

        /// <summary>
        /// The name of the MQTT sensor.
        /// </summary>
        [PublicAPI]
        public string Name
        {
            get => GetValue<string>("name", default);
            set => SetValue("name", value);
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
        /// The MQTT topic subscribed to receive sensor values.
        /// </summary>
        [PublicAPI]
        public string StateTopic
        {
            get => GetValue<string>("state_topic", default);
            set => SetValue("state_topic", value);
        }

        /// <summary>
        /// Defines the units of measurement of the sensor, if any.
        /// </summary>
        [PublicAPI]
        public string UnitOfMeasurement
        {
            get => GetValue<string>("unit_of_measurement", default);
            set => SetValue("unit_of_measurement", value);
        }

        /// <summary>
        /// Defines a [template](/docs/configuration/templating/#processing-incoming-data) to extract the value.
        /// </summary>
        [PublicAPI]
        public string ValueTemplate
        {
            get => GetValue<string>("value_template", default);
            set => SetValue("value_template", value);
        }
    }
}