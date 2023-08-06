using System.Runtime.Serialization;

namespace MBW.HassMQTT.DiscoveryModels.Enum;

public enum HassTextMode
{
    [EnumMember(Value = "text")]
    Text,
    
    [EnumMember(Value = "password")]
    Password
}