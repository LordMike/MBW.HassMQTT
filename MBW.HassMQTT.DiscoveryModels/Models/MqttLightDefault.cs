using JetBrains.Annotations;
using MBW.HassMQTT.DiscoveryModels.Enum;
using MBW.HassMQTT.DiscoveryModels.Metadata;

namespace MBW.HassMQTT.DiscoveryModels.Models
{
    /// <summary>
    /// https://www.home-assistant.io/integrations/light.mqtt/#default-schema
    /// </summary>
    [DeviceType(HassDeviceType.Light)]
    public class MqttLightDefault : MqttEntitySensorDiscoveryBase
    {
        public MqttLightDefault(string discoveryTopic, string uniqueId) : base(discoveryTopic, uniqueId)
        {
        }

        /// <summary>
        /// The MQTT topic to publish commands to change the light’s brightness.
        /// </summary>
        [PublicAPI]
        public string BrightnessCommandTopic
        {
            get => GetValue<string>("brightness_command_topic", default);
            set => SetValue("brightness_command_topic", value);
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
        /// The MQTT topic subscribed to receive brightness state updates.
        /// </summary>
        [PublicAPI]
        public string BrightnessStateTopic
        {
            get => GetValue<string>("brightness_state_topic", default);
            set => SetValue("brightness_state_topic", value);
        }

        /// <summary>
        /// Defines a [template](/docs/configuration/templating/#processing-incoming-data) to extract the brightness value.
        /// </summary>
        [PublicAPI]
        public string BrightnessValueTemplate
        {
            get => GetValue<string>("brightness_value_template", default);
            set => SetValue("brightness_value_template", value);
        }

        /// <summary>
        /// Defines a [template](/docs/configuration/templating/) to compose message which will be sent to `color_temp_command_topic`. Available variables: `value`.
        /// </summary>
        [PublicAPI]
        public string ColorTempCommandTemplate
        {
            get => GetValue<string>("color_temp_command_template", default);
            set => SetValue("color_temp_command_template", value);
        }

        /// <summary>
        /// The MQTT topic to publish commands to change the light’s color temperature state. The color temperature command slider has a range of 153 to 500 mireds (micro reciprocal degrees).
        /// </summary>
        [PublicAPI]
        public string ColorTempCommandTopic
        {
            get => GetValue<string>("color_temp_command_topic", default);
            set => SetValue("color_temp_command_topic", value);
        }

        /// <summary>
        /// The MQTT topic subscribed to receive color temperature state updates.
        /// </summary>
        [PublicAPI]
        public string ColorTempStateTopic
        {
            get => GetValue<string>("color_temp_state_topic", default);
            set => SetValue("color_temp_state_topic", value);
        }

        /// <summary>
        /// Defines a [template](/docs/configuration/templating/#processing-incoming-data) to extract the color temperature value.
        /// </summary>
        [PublicAPI]
        public string ColorTempValueTemplate
        {
            get => GetValue<string>("color_temp_value_template", default);
            set => SetValue("color_temp_value_template", value);
        }

        /// <summary>
        /// The MQTT topic to publish commands to change the switch state.
        /// </summary>
        [PublicAPI]
        public string CommandTopic
        {
            get => GetValue<string>("command_topic", default);
            set => SetValue("command_topic", value);
        }

        /// <summary>
        /// The MQTT topic to publish commands to change the light's effect state.
        /// </summary>
        [PublicAPI]
        public string EffectCommandTopic
        {
            get => GetValue<string>("effect_command_topic", default);
            set => SetValue("effect_command_topic", value);
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
        /// The MQTT topic subscribed to receive effect state updates.
        /// </summary>
        [PublicAPI]
        public string EffectStateTopic
        {
            get => GetValue<string>("effect_state_topic", default);
            set => SetValue("effect_state_topic", value);
        }

        /// <summary>
        /// Defines a [template](/docs/configuration/templating/#processing-incoming-data) to extract the effect value.
        /// </summary>
        [PublicAPI]
        public string EffectValueTemplate
        {
            get => GetValue<string>("effect_value_template", default);
            set => SetValue("effect_value_template", value);
        }

        /// <summary>
        /// The MQTT topic to publish commands to change the light's color state in HS format (Hue Saturation). Brightness is sent separately in the `brightness_command_topic`. Range for Hue: 0° .. 360°, Range of Saturation: 0..100.
        /// </summary>
        [PublicAPI]
        public string HsCommandTopic
        {
            get => GetValue<string>("hs_command_topic", default);
            set => SetValue("hs_command_topic", value);
        }

        /// <summary>
        /// The MQTT topic subscribed to receive color state updates in HS format. Brightness is received separately in the `brightness_state_topic`.
        /// </summary>
        [PublicAPI]
        public string HsStateTopic
        {
            get => GetValue<string>("hs_state_topic", default);
            set => SetValue("hs_state_topic", value);
        }

        /// <summary>
        /// Defines a [template](/docs/configuration/templating/#processing-incoming-data) to extract the HS value.
        /// </summary>
        [PublicAPI]
        public string HsValueTemplate
        {
            get => GetValue<string>("hs_value_template", default);
            set => SetValue("hs_value_template", value);
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
        /// Defines when on the payload_on is sent. Using `last` (the default) will send any style (brightness, color, etc) topics first and then a `payload_on` to the `command_topic`. Using `first` will send the `payload_on` and then any style topics. Using `brightness` will only send brightness commands instead of the `payload_on` to turn the light on.
        /// </summary>
        [PublicAPI]
        public string OnCommandType
        {
            get => GetValue<string>("on_command_type", default);
            set => SetValue("on_command_type", value);
        }

        /// <summary>
        /// Flag that defines if switch works in optimistic mode.
        /// </summary>
        [PublicAPI]
        public bool Optimistic
        {
            get => GetValue<bool>("optimistic", default);
            set => SetValue("optimistic", value);
        }

        /// <summary>
        /// The payload that represents disabled state.
        /// </summary>
        [PublicAPI]
        public string PayloadOff
        {
            get => GetValue<string>("payload_off", default);
            set => SetValue("payload_off", value);
        }

        /// <summary>
        /// The payload that represents enabled state.
        /// </summary>
        [PublicAPI]
        public string PayloadOn
        {
            get => GetValue<string>("payload_on", default);
            set => SetValue("payload_on", value);
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
        /// Defines a [template](/docs/configuration/templating/) to compose message which will be sent to `rgb_command_topic`. Available variables: `red`, `green` and `blue`.
        /// </summary>
        [PublicAPI]
        public string RgbCommandTemplate
        {
            get => GetValue<string>("rgb_command_template", default);
            set => SetValue("rgb_command_template", value);
        }

        /// <summary>
        /// The MQTT topic to publish commands to change the light's RGB state. Please note that the color value sent by Home Assistant is normalized to full brightness if `brightness_command_topic` is set. Brightness information is in this case sent separately in the `brightness_command_topic`. This will cause a light that expects an absolute color value (including brightness) to flicker.
        /// </summary>
        [PublicAPI]
        public string RgbCommandTopic
        {
            get => GetValue<string>("rgb_command_topic", default);
            set => SetValue("rgb_command_topic", value);
        }

        /// <summary>
        /// The MQTT topic subscribed to receive RGB state updates. The expected payload is the RGB values separated by commas, for example, `255,0,127`. Please note that the color value received by Home Assistant is normalized to full brightness. Brightness information is received separately in the `brightness_state_topic`.
        /// </summary>
        [PublicAPI]
        public string RgbStateTopic
        {
            get => GetValue<string>("rgb_state_topic", default);
            set => SetValue("rgb_state_topic", value);
        }

        /// <summary>
        /// Defines a [template](/docs/configuration/templating/#processing-incoming-data) to extract the RGB value.
        /// </summary>
        [PublicAPI]
        public string RgbValueTemplate
        {
            get => GetValue<string>("rgb_value_template", default);
            set => SetValue("rgb_value_template", value);
        }

        /// <summary>
        /// The schema to use. Must be `default` or omitted to select the default schema.
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
        /// Defines a [template](/docs/configuration/templating/#processing-incoming-data) to extract the state value. The template should match the payload `on` and `off` values, so if your light uses `power on` to turn on, your `state_value_template` string should return `power on` when the switch is on. For example if the message is just `on`, your `state_value_template` should be `power {{ value }}`.
        /// </summary>
        [PublicAPI]
        public string StateValueTemplate
        {
            get => GetValue<string>("state_value_template", default);
            set => SetValue("state_value_template", value);
        }

        /// <summary>
        /// The MQTT topic to publish commands to change the light's white value.
        /// </summary>
        [PublicAPI]
        public string WhiteValueCommandTopic
        {
            get => GetValue<string>("white_value_command_topic", default);
            set => SetValue("white_value_command_topic", value);
        }

        /// <summary>
        /// Defines the maximum white value (i.e., 100%) of the MQTT device.
        /// </summary>
        [PublicAPI]
        public int WhiteValueScale
        {
            get => GetValue<int>("white_value_scale", default);
            set => SetValue("white_value_scale", value);
        }

        /// <summary>
        /// The MQTT topic subscribed to receive white value updates.
        /// </summary>
        [PublicAPI]
        public string WhiteValueStateTopic
        {
            get => GetValue<string>("white_value_state_topic", default);
            set => SetValue("white_value_state_topic", value);
        }

        /// <summary>
        /// Defines a [template](/docs/configuration/templating/#processing-incoming-data) to extract the white value.
        /// </summary>
        [PublicAPI]
        public string WhiteValueTemplate
        {
            get => GetValue<string>("white_value_template", default);
            set => SetValue("white_value_template", value);
        }

        /// <summary>
        /// The MQTT topic to publish commands to change the light's XY state.
        /// </summary>
        [PublicAPI]
        public string XyCommandTopic
        {
            get => GetValue<string>("xy_command_topic", default);
            set => SetValue("xy_command_topic", value);
        }

        /// <summary>
        /// The MQTT topic subscribed to receive XY state updates.
        /// </summary>
        [PublicAPI]
        public string XyStateTopic
        {
            get => GetValue<string>("xy_state_topic", default);
            set => SetValue("xy_state_topic", value);
        }

        /// <summary>
        /// Defines a [template](/docs/configuration/templating/#processing-incoming-data) to extract the XY value.
        /// </summary>
        [PublicAPI]
        public string XyValueTemplate
        {
            get => GetValue<string>("xy_value_template", default);
            set => SetValue("xy_value_template", value);
        }
    }
}