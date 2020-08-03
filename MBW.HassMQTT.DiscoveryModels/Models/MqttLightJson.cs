using JetBrains.Annotations;
using MBW.HassMQTT.DiscoveryModels.Enum;
using MBW.HassMQTT.DiscoveryModels.Metadata;

namespace MBW.HassMQTT.DiscoveryModels.Models
{
    /// <summary>
    /// https://www.home-assistant.io/integrations/light.mqtt/#json-schema
    /// </summary>
    [DeviceType(HassDeviceType.Light)]
    public class MqttLightJson : MqttEntitySensorDiscoveryBase
    {
        public MqttLightJson(string discoveryTopic, string uniqueId) : base(discoveryTopic, uniqueId)
        {
        }

        /// <summary>
        /// Flag that defines if the light supports brightness.
        /// </summary>
        [PublicAPI]
        public bool Brightness
        {
            get => GetValue<bool>("brightness", default);
            set => SetValue("brightness", value);
        }

        /// <summary>
        /// Defines the maximum brightness value (i.e., 100%) of the MQTT device.
        /// </summary>
        [PublicAPI]
        public int BrightnessScale
        {
            get => GetValue<int>("brightness_scale", default);
            set => SetValue("brightness_scale", value);
        }

        /// <summary>
        /// Flag that defines if the light supports color temperature.
        /// </summary>
        [PublicAPI]
        public bool ColorTemp
        {
            get => GetValue<bool>("color_temp", default);
            set => SetValue("color_temp", value);
        }

        /// <summary>
        /// The MQTT topic to publish commands to change the light’s state.
        /// </summary>
        [PublicAPI]
        public string CommandTopic
        {
            get => GetValue<string>("command_topic", default);
            set => SetValue("command_topic", value);
        }

        /// <summary>
        /// Flag that defines if the light supports effects.
        /// </summary>
        [PublicAPI]
        public bool Effect
        {
            get => GetValue<bool>("effect", default);
            set => SetValue("effect", value);
        }

        /// <summary>
        /// The list of effects the light supports.
        /// </summary>
        [PublicAPI]
        public string[] EffectList
        {
            get => GetValue<string[]>("effect_list", default);
            set => SetValue("effect_list", value);
        }

        /// <summary>
        /// The duration, in seconds, of a “long” flash.
        /// </summary>
        [PublicAPI]
        public int FlashTimeLong
        {
            get => GetValue<int>("flash_time_long", default);
            set => SetValue("flash_time_long", value);
        }

        /// <summary>
        /// The duration, in seconds, of a “short” flash.
        /// </summary>
        [PublicAPI]
        public int FlashTimeShort
        {
            get => GetValue<int>("flash_time_short", default);
            set => SetValue("flash_time_short", value);
        }

        /// <summary>
        /// Flag that defines if the light supports HS colors.
        /// </summary>
        [PublicAPI]
        public bool Hs
        {
            get => GetValue<bool>("hs", default);
            set => SetValue("hs", value);
        }

        /// <summary>
        /// The maximum color temperature in mireds.
        /// </summary>
        [PublicAPI]
        public int MaxMireds
        {
            get => GetValue<int>("max_mireds", default);
            set => SetValue("max_mireds", value);
        }

        /// <summary>
        /// The minimum color temperature in mireds.
        /// </summary>
        [PublicAPI]
        public int MinMireds
        {
            get => GetValue<int>("min_mireds", default);
            set => SetValue("min_mireds", value);
        }

        /// <summary>
        /// The name of the light.
        /// </summary>
        [PublicAPI]
        public string Name
        {
            get => GetValue<string>("name", default);
            set => SetValue("name", value);
        }

        /// <summary>
        /// Flag that defines if the light works in optimistic mode.
        /// </summary>
        [PublicAPI]
        public bool Optimistic
        {
            get => GetValue<bool>("optimistic", default);
            set => SetValue("optimistic", value);
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
        /// If the published message should have the retain flag on or not.
        /// </summary>
        [PublicAPI]
        public bool Retain
        {
            get => GetValue<bool>("retain", default);
            set => SetValue("retain", value);
        }

        /// <summary>
        /// Flag that defines if the light supports RGB colors.
        /// </summary>
        [PublicAPI]
        public bool Rgb
        {
            get => GetValue<bool>("rgb", default);
            set => SetValue("rgb", value);
        }

        /// <summary>
        /// The schema to use. Must be `json` to select the JSON schema.
        /// </summary>
        [PublicAPI]
        public string Schema
        {
            get => GetValue<string>("schema", default);
            set => SetValue("schema", value);
        }

        /// <summary>
        /// The MQTT topic subscribed to receive state updates.
        /// </summary>
        [PublicAPI]
        public string StateTopic
        {
            get => GetValue<string>("state_topic", default);
            set => SetValue("state_topic", value);
        }

        /// <summary>
        /// Flag that defines if the light supports white values.
        /// </summary>
        [PublicAPI]
        public bool WhiteValue
        {
            get => GetValue<bool>("white_value", default);
            set => SetValue("white_value", value);
        }

        /// <summary>
        /// Flag that defines if the light supports XY colors.
        /// </summary>
        [PublicAPI]
        public bool Xy
        {
            get => GetValue<bool>("xy", default);
            set => SetValue("xy", value);
        }
    }
}