using System.Runtime.Serialization;

namespace MBW.HassMQTT.DiscoveryModels.Enum;

/// <summary>Device classes supported by Home Assistant valve entities.</summary>
public enum HassValveDeviceClass
{
    /// <summary>A generic valve.</summary>
    None,

    /// <summary>A valve that controls water flow.</summary>
    [EnumMember(Value = "water")]
    Water,

    /// <summary>A valve that controls gas flow.</summary>
    [EnumMember(Value = "gas")]
    Gas,
}
