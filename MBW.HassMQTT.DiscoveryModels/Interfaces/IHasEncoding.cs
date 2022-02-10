#nullable enable

namespace MBW.HassMQTT.DiscoveryModels.Interfaces
{
    public interface IHasEncoding
    {
        /// <summary>
        /// The encoding of the payloads received and published messages. Set to `""` to disable decoding of incoming payload.
        /// </summary>
        public string? Encoding { get; set; }
    }
}