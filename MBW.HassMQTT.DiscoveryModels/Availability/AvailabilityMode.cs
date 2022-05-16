using System.Runtime.Serialization;

namespace MBW.HassMQTT.DiscoveryModels.Availability;

public enum AvailabilityMode
{
    [EnumMember(Value = "all")]
    All,
        
    [EnumMember(Value = "any")]
    Any,
        
    [EnumMember(Value = "latest")]
    Latest
}