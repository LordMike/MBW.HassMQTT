using JetBrains.Annotations;
using MBW.HassMQTT.DiscoveryModels.Enum;
using MBW.HassMQTT.DiscoveryModels.Metadata;

namespace MBW.HassMQTT.DiscoveryModels.Models
{
    /// <summary>
    /// https://www.home-assistant.io/integrations/light.mqtt/#json-schema
    /// </summary>
    [DeviceType(HassDeviceType.Light)]
    [PublicAPI]
    public class MqttLightJson : MqttEntitySensorDiscoveryBase
    {
        public MqttLightJson(string discoveryTopic, string uniqueId) : base(discoveryTopic, uniqueId)
        {
        }

        /// <summary>
        /// Flag that defines if the light supports brightness.
        /// </summary>
        public bool Brightness { get; set; }

        /// <summary>
        /// Defines the maximum brightness value (i.e., 100%) of the MQTT device.
        /// </summary>
        public int BrightnessScale { get; set; }

        /// <summary>
        /// Flag that defines if the light supports color temperature.
        /// </summary>
        public bool ColorTemp { get; set; }

        /// <summary>
        /// The MQTT topic to publish commands to change the light’s state.
        /// </summary>
        public string CommandTopic { get; set; }

        /// <summary>
        /// Flag that defines if the light supports effects.
        /// </summary>
        public bool Effect { get; set; }

        /// <summary>
        /// The list of effects the light supports.
        /// </summary>
        public string[] EffectList { get; set; }

        /// <summary>
        /// The duration, in seconds, of a “long” flash.
        /// </summary>
        public int FlashTimeLong { get; set; }

        /// <summary>
        /// The duration, in seconds, of a “short” flash.
        /// </summary>
        public int FlashTimeShort { get; set; }

        /// <summary>
        /// Flag that defines if the light supports HS colors.
        /// </summary>
        public bool Hs { get; set; }

        /// <summary>
        /// The maximum color temperature in mireds.
        /// </summary>
        public int MaxMireds { get; set; }

        /// <summary>
        /// The minimum color temperature in mireds.
        /// </summary>
        public int MinMireds { get; set; }

        /// <summary>
        /// The name of the light.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Flag that defines if the light works in optimistic mode.
        /// </summary>
        public bool Optimistic { get; set; }

        /// <summary>
        /// The maximum QoS level of the state topic.
        /// </summary>
        public MqttQosLevel Qos { get; set; }

        /// <summary>
        /// If the published message should have the retain flag on or not.
        /// </summary>
        public bool Retain { get; set; }

        /// <summary>
        /// Flag that defines if the light supports RGB colors.
        /// </summary>
        public bool Rgb { get; set; }

        /// <summary>
        /// The schema to use. Must be `json` to select the JSON schema.
        /// </summary>
        public string Schema { get; set; }

        /// <summary>
        /// The MQTT topic subscribed to receive state updates.
        /// </summary>
        public string StateTopic { get; set; }

        /// <summary>
        /// Flag that defines if the light supports white values.
        /// </summary>
        public bool WhiteValue { get; set; }

        /// <summary>
        /// Flag that defines if the light supports XY colors.
        /// </summary>
        public bool Xy { get; set; }
    }
}