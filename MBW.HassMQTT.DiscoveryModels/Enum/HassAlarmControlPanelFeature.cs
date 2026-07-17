using System.Runtime.Serialization;

namespace MBW.HassMQTT.DiscoveryModels.Enum;

/// <summary>
/// Features an MQTT alarm control panel can expose in Home Assistant.
/// </summary>
public enum HassAlarmControlPanelFeature
{
    [EnumMember(Value = "arm_home")]
    ArmHome,

    [EnumMember(Value = "arm_away")]
    ArmAway,

    [EnumMember(Value = "arm_night")]
    ArmNight,

    [EnumMember(Value = "arm_vacation")]
    ArmVacation,

    [EnumMember(Value = "arm_custom_bypass")]
    ArmCustomBypass,

    [EnumMember(Value = "trigger")]
    Trigger,
}
