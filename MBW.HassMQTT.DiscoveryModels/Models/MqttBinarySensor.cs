using JetBrains.Annotations;
using MBW.HassMQTT.DiscoveryModels.Enum;
using MBW.HassMQTT.DiscoveryModels.Metadata;

namespace MBW.HassMQTT.DiscoveryModels.Models
{
    /// <summary>
    /// https://www.home-assistant.io/integrations/binary_sensor.mqtt/
    /// </summary>
    [DeviceType(HassDeviceType.BinarySensor)]
    [PublicAPI]
    public class MqttBinarySensor : MqttEntitySensorDiscoveryBase
    {
        public MqttBinarySensor(string discoveryTopic, string uniqueId) : base(discoveryTopic, uniqueId)
        {
        }

        /// <summary>
        /// Sets the [class of the device](/integrations/binary_sensor/#device-class), changing the device state and icon that is displayed on the frontend.
        /// </summary>
        public HassDeviceClass DeviceClass { get; set; }

        /// <summary>
        /// Defines the number of seconds after the sensor's state expires if it's not updated. After expiry, the sensor's state becomes `unavailable` if `availability_topic` is defined and `unknown` otherwise.
        /// </summary>
        public int ExpireAfter { get; set; }

        /// <summary>
        /// Sends update events (which results in update of [state object](/docs/configuration/state_object/)'s `last_changed`) even if the sensor's state hasn't changed. Useful if you want to have meaningful value graphs in history or want to create an automation that triggers on *every* incoming state message (not only when the sensor's new state is different to the current one).
        /// </summary>
        public bool ForceUpdate { get; set; }

        /// <summary>
        /// The name of the binary sensor.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// For sensors that only send `on` state updates (like PIRs), this variable sets a delay in seconds after which the sensor's state will be updated back to `off`.
        /// </summary>
        public int OffDelay { get; set; }

        /// <summary>
        /// The string that represents the `off` state. It will be compared to the message in the `state_topic` (see `value_template` for details)
        /// </summary>
        public string PayloadOff { get; set; }

        /// <summary>
        /// The string that represents the `on` state. It will be compared to the message in the `state_topic` (see `value_template` for details)
        /// </summary>
        public string PayloadOn { get; set; }

        /// <summary>
        /// The maximum QoS level to be used when receiving messages.
        /// </summary>
        public MqttQosLevel Qos { get; set; }

        /// <summary>
        /// The MQTT topic subscribed to receive sensor's state.
        /// </summary>
        public string StateTopic { get; set; }

        /// <summary>
        /// Defines a [template](/docs/configuration/templating/#processing-incoming-data) that returns a string to be compared to `payload_on`/`payload_off`. Available variables: `entity_id`. Remove this option when 'payload_on' and 'payload_off' are sufficient to match your payloads (i.e no pre-processing of original message is required).
        /// </summary>
        public string ValueTemplate { get; set; }
    }
}