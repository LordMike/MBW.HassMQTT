using System.Runtime.Serialization;

namespace MBW.HassMQTT.DiscoveryModels.Enum;

public enum HumidifierDeviceClass
{
    Unknown,

    [EnumMember(Value = "humidifier")]
    Humidifier,

    [EnumMember(Value = "dehumidifier")]
    Dehumidifier
}