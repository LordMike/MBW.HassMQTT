#nullable enable

namespace MBW.HassMQTT.DiscoveryModels.Interfaces;

public interface IHasEncoding
{
    /// <summary>
    /// The encoding of received and published payloads. Set to an empty string to disable decoding of incoming payloads.
    /// </summary>
    /// <remarks>The Home Assistant default is <c>utf-8</c>.</remarks>
    public string? Encoding { get; set; }
}
