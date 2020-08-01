using MBW.HassMQTT.DiscoveryModels.Enum;

namespace MBW.HassMQTT.DiscoveryModels.Models
{
    /// <summary>
    /// https://www.home-assistant.io/integrations/light.mqtt/#json-schema
    /// </summary>
    [DeviceType(HassDeviceType.Light)]
    public class MqttLightJson : MqttEntitySensorDiscoveryBase
    {
        public MqttLightJson(string topic, string uniqueId) : base(topic, uniqueId)
        {
        }

        /// <summary>
        /// Flag that defines if the light supports brightness.
        /// </summary>
        public bool Brightness
        {
            get => GetValue<bool>("brightness", default);
            set => SetValue("brightness", value);
        }

        /// <summary>
        /// Defines the maximum brightness value (i.e., 100%) of the MQTT device.
        /// </summary>
        public int BrightnessScale
        {
            get => GetValue<int>("brightness_scale", default);
            set => SetValue("brightness_scale", value);
        }

        /// <summary>
        /// Flag that defines if the light supports color temperature.
        /// </summary>
        public bool ColorTemp
        {
            get => GetValue<bool>("color_temp", default);
            set => SetValue("color_temp", value);
        }

        /// <summary>
        /// The MQTT topic to publish commands to change the light’s state.
        /// </summary>
        public string CommandTopic
        {
            get => GetValue<string>("command_topic", default);
            set => SetValue("command_topic", value);
        }

        /// <summary>
        /// Flag that defines if the light supports effects.
        /// </summary>
        public bool Effect
        {
            get => GetValue<bool>("effect", default);
            set => SetValue("effect", value);
        }

        /// <summary>
        /// The list of effects the light supports.
        /// </summary>
        public string[] EffectList
        {
            get => GetValue<string[]>("effect_list", default);
            set => SetValue("effect_list", value);
        }

        /// <summary>
        /// The duration, in seconds, of a “long” flash.
        /// </summary>
        public int FlashTimeLong
        {
            get => GetValue<int>("flash_time_long", default);
            set => SetValue("flash_time_long", value);
        }

        /// <summary>
        /// The duration, in seconds, of a “short” flash.
        /// </summary>
        public int FlashTimeShort
        {
            get => GetValue<int>("flash_time_short", default);
            set => SetValue("flash_time_short", value);
        }

        /// <summary>
        /// Flag that defines if the light supports HS colors.
        /// </summary>
        public bool Hs
        {
            get => GetValue<bool>("hs", default);
            set => SetValue("hs", value);
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
        /// The maximum color temperature in mireds.
        /// </summary>
        public int MaxMireds
        {
            get => GetValue<int>("max_mireds", default);
            set => SetValue("max_mireds", value);
        }

        /// <summary>
        /// The minimum color temperature in mireds.
        /// </summary>
        public int MinMireds
        {
            get => GetValue<int>("min_mireds", default);
            set => SetValue("min_mireds", value);
        }

        /// <summary>
        /// The name of the light.
        /// </summary>
        public string Name
        {
            get => GetValue<string>("name", default);
            set => SetValue("name", value);
        }

        /// <summary>
        /// Flag that defines if the light works in optimistic mode.
        /// </summary>
        public bool Optimistic
        {
            get => GetValue<bool>("optimistic", default);
            set => SetValue("optimistic", value);
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
        /// If the published message should have the retain flag on or not.
        /// </summary>
        public bool Retain
        {
            get => GetValue<bool>("retain", default);
            set => SetValue("retain", value);
        }

        /// <summary>
        /// Flag that defines if the light supports RGB colors.
        /// </summary>
        public bool Rgb
        {
            get => GetValue<bool>("rgb", default);
            set => SetValue("rgb", value);
        }

        /// <summary>
        /// The schema to use. Must be `json` to select the JSON schema.
        /// </summary>
        public string Schema
        {
            get => GetValue<string>("schema", default);
            set => SetValue("schema", value);
        }

        /// <summary>
        /// The MQTT topic subscribed to receive state updates.
        /// </summary>
        public string StateTopic
        {
            get => GetValue<string>("state_topic", default);
            set => SetValue("state_topic", value);
        }

        /// <summary>
        /// Flag that defines if the light supports white values.
        /// </summary>
        public bool WhiteValue
        {
            get => GetValue<bool>("white_value", default);
            set => SetValue("white_value", value);
        }

        /// <summary>
        /// Flag that defines if the light supports XY colors.
        /// </summary>
        public bool Xy
        {
            get => GetValue<bool>("xy", default);
            set => SetValue("xy", value);
        }
    }
}