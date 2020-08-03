﻿using JetBrains.Annotations;
using MBW.HassMQTT.DiscoveryModels.Enum;
using MBW.HassMQTT.DiscoveryModels.Metadata;

namespace MBW.HassMQTT.DiscoveryModels.Models
{
    /// <summary>
    /// https://www.home-assistant.io/integrations/light.mqtt/#template-schema
    /// </summary>
    [DeviceType(HassDeviceType.Light)]
    public class MqttLightTemplate : MqttEntitySensorDiscoveryBase
    {
        public MqttLightTemplate(string discoveryTopic, string uniqueId) : base(discoveryTopic, uniqueId)
        {
        }

        /// <summary>
        /// [Template](/docs/configuration/templating/#processing-incoming-data) to extract blue color from the state payload value.
        /// </summary>
        [PublicAPI]
        public string BlueTemplate
        {
            get => GetValue<string>("blue_template", default);
            set => SetValue("blue_template", value);
        }

        /// <summary>
        /// [Template](/docs/configuration/templating/#processing-incoming-data) to extract brightness from the state payload value.
        /// </summary>
        [PublicAPI]
        public string BrightnessTemplate
        {
            get => GetValue<string>("brightness_template", default);
            set => SetValue("brightness_template", value);
        }

        /// <summary>
        /// [Template](/docs/configuration/templating/#processing-incoming-data) to extract color temperature from the state payload value.
        /// </summary>
        [PublicAPI]
        public string ColorTempTemplate
        {
            get => GetValue<string>("color_temp_template", default);
            set => SetValue("color_temp_template", value);
        }

        /// <summary>
        /// The [template](/docs/configuration/templating/#processing-incoming-data) for *off* state changes. Available variables: `state` and `transition`.
        /// </summary>
        [PublicAPI]
        public string CommandOffTemplate
        {
            get => GetValue<string>("command_off_template", default);
            set => SetValue("command_off_template", value);
        }

        /// <summary>
        /// The [template](/docs/configuration/templating/#processing-incoming-data) for *on* state changes. Available variables: `state`, `brightness`, `red`, `green`, `blue`, `white_value`, `flash`, `transition` and `effect`.
        /// </summary>
        [PublicAPI]
        public string CommandOnTemplate
        {
            get => GetValue<string>("command_on_template", default);
            set => SetValue("command_on_template", value);
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
        /// List of possible effects.
        /// </summary>
        [PublicAPI]
        public string[] EffectList
        {
            get => GetValue<string[]>("effect_list", default);
            set => SetValue("effect_list", value);
        }

        /// <summary>
        /// [Template](/docs/configuration/templating/#processing-incoming-data) to extract effect from the state payload value.
        /// </summary>
        [PublicAPI]
        public string EffectTemplate
        {
            get => GetValue<string>("effect_template", default);
            set => SetValue("effect_template", value);
        }

        /// <summary>
        /// [Template](/docs/configuration/templating/#processing-incoming-data) to extract green color from the state payload value.
        /// </summary>
        [PublicAPI]
        public string GreenTemplate
        {
            get => GetValue<string>("green_template", default);
            set => SetValue("green_template", value);
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
        public string Optimistic
        {
            get => GetValue<string>("optimistic", default);
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
        /// [Template](/docs/configuration/templating/#processing-incoming-data) to extract red color from the state payload value.
        /// </summary>
        [PublicAPI]
        public string RedTemplate
        {
            get => GetValue<string>("red_template", default);
            set => SetValue("red_template", value);
        }

        /// <summary>
        /// The schema to use. Must be `template` to select the template schema.
        /// </summary>
        [PublicAPI]
        public string Schema
        {
            get => GetValue<string>("schema", default);
            set => SetValue("schema", value);
        }

        /// <summary>
        /// [Template](/docs/configuration/templating/#processing-incoming-data) to extract state from the state payload value.
        /// </summary>
        [PublicAPI]
        public string StateTemplate
        {
            get => GetValue<string>("state_template", default);
            set => SetValue("state_template", value);
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
        /// [Template](/docs/configuration/templating/#processing-incoming-data) to extract white value from the state payload value.
        /// </summary>
        [PublicAPI]
        public string WhiteValueTemplate
        {
            get => GetValue<string>("white_value_template", default);
            set => SetValue("white_value_template", value);
        }
    }
}