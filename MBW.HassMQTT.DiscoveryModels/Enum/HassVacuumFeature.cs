using System.Runtime.Serialization;

namespace MBW.HassMQTT.DiscoveryModels.Enum;

/// <summary>Features supported by an MQTT vacuum.</summary>
public enum HassVacuumFeature
{
    [EnumMember(Value = "start")]
    Start,
    [EnumMember(Value = "stop")]
    Stop,
    [EnumMember(Value = "pause")]
    Pause,
    [EnumMember(Value = "return_home")]
    ReturnHome,
    [EnumMember(Value = "status")]
    Status,
    [EnumMember(Value = "locate")]
    Locate,
    [EnumMember(Value = "clean_spot")]
    CleanSpot,
    [EnumMember(Value = "fan_speed")]
    FanSpeed,
    [EnumMember(Value = "send_command")]
    SendCommand,
}
