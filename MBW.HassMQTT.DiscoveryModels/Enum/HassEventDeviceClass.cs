using System.Runtime.Serialization;

namespace MBW.HassMQTT.DiscoveryModels.Enum;

public enum HassEventDeviceClass
{
    /// <summary>
    /// Generic number. This is the default and doesn’t need to be set.
    ///</summary>
    [EnumMember(Value = "null")]
    Null,
    
    /// <summary>
    /// Temperature in °C or °F
    /// </summary>
    [EnumMember(Value = "device_class")]
    DeviceClass
}