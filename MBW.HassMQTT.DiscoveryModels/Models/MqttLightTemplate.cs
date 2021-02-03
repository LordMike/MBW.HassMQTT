using JetBrains.Annotations;
using MBW.HassMQTT.DiscoveryModels.Enum;
using MBW.HassMQTT.DiscoveryModels.Metadata;

namespace MBW.HassMQTT.DiscoveryModels.Models
{
    /// <summary>
    /// https://www.home-assistant.io/integrations/light.mqtt/#template-schema
    /// </summary>
    [DeviceType(HassDeviceType.Light)]
    [PublicAPI]
    public class MqttLightTemplate : MqttEntitySensorDiscoveryBase
    {
        public MqttLightTemplate(string discoveryTopic, string uniqueId) : base(discoveryTopic, uniqueId)
        {
        }

        /// <summary>
        /// [Template](/docs/configuration/templating/#processing-incoming-data) to extract blue color from the state payload value.
        /// </summary>
        public string BlueTemplate { get; set; }

        /// <summary>
        /// [Template](/docs/configuration/templating/#processing-incoming-data) to extract brightness from the state payload value.
        /// </summary>
        public string BrightnessTemplate { get; set; }

        /// <summary>
        /// [Template](/docs/configuration/templating/#processing-incoming-data) to extract color temperature from the state payload value.
        /// </summary>
        public string ColorTempTemplate { get; set; }

        /// <summary>
        /// The [template](/docs/configuration/templating/#processing-incoming-data) for *off* state changes. Available variables: `state` and `transition`.
        /// </summary>
        public string CommandOffTemplate { get; set; }

        /// <summary>
        /// The [template](/docs/configuration/templating/#processing-incoming-data) for *on* state changes. Available variables: `state`, `brightness`, `red`, `green`, `blue`, `white_value`, `flash`, `transition` and `effect`.
        /// </summary>
        public string CommandOnTemplate { get; set; }

        /// <summary>
        /// The MQTT topic to publish commands to change the light’s state.
        /// </summary>
        public string CommandTopic { get; set; }

        /// <summary>
        /// List of possible effects.
        /// </summary>
        public string[] EffectList { get; set; }

        /// <summary>
        /// [Template](/docs/configuration/templating/#processing-incoming-data) to extract effect from the state payload value.
        /// </summary>
        public string EffectTemplate { get; set; }

        /// <summary>
        /// [Template](/docs/configuration/templating/#processing-incoming-data) to extract green color from the state payload value.
        /// </summary>
        public string GreenTemplate { get; set; }

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
        public string Optimistic { get; set; }

        /// <summary>
        /// The maximum QoS level of the state topic.
        /// </summary>
        public MqttQosLevel Qos { get; set; }

        /// <summary>
        /// [Template](/docs/configuration/templating/#processing-incoming-data) to extract red color from the state payload value.
        /// </summary>
        public string RedTemplate { get; set; }

        /// <summary>
        /// The schema to use. Must be `template` to select the template schema.
        /// </summary>
        public string Schema { get; set; }

        /// <summary>
        /// [Template](/docs/configuration/templating/#processing-incoming-data) to extract state from the state payload value.
        /// </summary>
        public string StateTemplate { get; set; }

        /// <summary>
        /// The MQTT topic subscribed to receive state updates.
        /// </summary>
        public string StateTopic { get; set; }

        /// <summary>
        /// [Template](/docs/configuration/templating/#processing-incoming-data) to extract white value from the state payload value.
        /// </summary>
        public string WhiteValueTemplate { get; set; }
    }
}