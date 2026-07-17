#nullable enable

namespace MBW.HassMQTT.DiscoveryModels.Interfaces;

public interface IHasJsonAttributes
{
    /// <summary>
    /// A template used to extract the JSON dictionary from messages received on <see cref="JsonAttributesTopic" />.
    /// </summary>
    string? JsonAttributesTemplate { get; set; }

    /// <summary>
    /// The MQTT topic subscribed to receive a JSON dictionary whose members are added as entity attributes.
    /// </summary>
    string? JsonAttributesTopic { get; set; }
}
