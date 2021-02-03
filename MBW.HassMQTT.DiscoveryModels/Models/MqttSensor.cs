using JetBrains.Annotations;
using MBW.HassMQTT.DiscoveryModels.Enum;
using MBW.HassMQTT.DiscoveryModels.Metadata;

namespace MBW.HassMQTT.DiscoveryModels.Models
{
    /// <summary>
    /// https://www.home-assistant.io/integrations/sensor.mqtt/
    /// </summary>
    [DeviceType(HassDeviceType.Sensor)]
    [PublicAPI]
    public class MqttSensor : MqttEntitySensorDiscoveryBase
    {
        public MqttSensor(string discoveryTopic, string uniqueId) : base(discoveryTopic, uniqueId)
        {
        }

        /// <summary>
        /// The [type/class](/integrations/sensor/#device-class) of the sensor to set the icon in the frontend.
        /// </summary>
        public HassDeviceClass DeviceClass { get; set; }

        /// <summary>
        /// Defines the number of seconds after the value expires if it's not updated.
        /// </summary>
        public int ExpireAfter { get; set; }

        /// <summary>
        /// Sends update events even if the value hasn't changed. Useful if you want to have meaningful value graphs in history.
        /// </summary>
        public bool ForceUpdate { get; set; }

        /// <summary>
        /// The icon for the sensor.
        /// </summary>
        public string Icon { get; set; }

        /// <summary>
        /// The name of the MQTT sensor.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The maximum QoS level of the state topic.
        /// </summary>
        public MqttQosLevel Qos { get; set; }

        /// <summary>
        /// The MQTT topic subscribed to receive sensor values.
        /// </summary>
        public string StateTopic { get; set; }

        /// <summary>
        /// Defines the units of measurement of the sensor, if any.
        /// </summary>
        public string UnitOfMeasurement { get; set; }

        /// <summary>
        /// Defines a [template](/docs/configuration/templating/#processing-incoming-data) to extract the value.
        /// </summary>
        public string ValueTemplate { get; set; }
    }
}