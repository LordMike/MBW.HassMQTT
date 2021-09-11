#nullable enable

namespace MBW.HassMQTT.DiscoveryModels.Interfaces
{
    public interface IHasJsonAttributes
    {
        /// <summary>
        /// Defines a [template](/docs/configuration/templating/#processing-incoming-data) to extract the JSON dictionary from messages received on the `json_attributes_topic`.
        /// </summary>
        string? JsonAttributesTemplate { get; set; }

        /// <summary>
        /// The MQTT topic subscribed to receive a JSON dictionary payload and then set as sensor attributes.
        /// Implies force_update of the current sensor state when a message is received on this topic.
        /// </summary>
        string? JsonAttributesTopic { get; set; }
    }
}