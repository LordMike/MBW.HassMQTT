#nullable enable

using System.Collections.Generic;

namespace MBW.HassMQTT.DiscoveryModels.Interfaces;

public interface IHasGroup
{
    /// <summary>
    /// The unique IDs of the member entities when this entity represents a group.
    /// </summary>
    IList<string>? Group { get; set; }
}
