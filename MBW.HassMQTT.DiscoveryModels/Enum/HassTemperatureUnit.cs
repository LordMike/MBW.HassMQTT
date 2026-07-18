using System.Runtime.Serialization;

namespace MBW.HassMQTT.DiscoveryModels.Enum;

public enum HassTemperatureUnit
{
    None,

    [EnumMember(Value = "C")]
    Celsius,

    [EnumMember(Value = "F")]
    Fahrenheit
}
