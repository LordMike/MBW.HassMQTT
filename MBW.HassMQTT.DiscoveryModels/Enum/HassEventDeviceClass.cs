using System.Runtime.Serialization;

namespace MBW.HassMQTT.DiscoveryModels.Enum;

/// <summary>Device classes supported by Home Assistant event entities.</summary>
public enum HassEventDeviceClass
{
    /// <summary>A generic event.</summary>
    None,

    /// <summary>An event emitted by a remote-control button.</summary>
    [EnumMember(Value = "button")]
    Button,

    /// <summary>A doorbell event.</summary>
    [EnumMember(Value = "doorbell")]
    Doorbell,

    /// <summary>A motion-detection event.</summary>
    [EnumMember(Value = "motion")]
    Motion,
}
