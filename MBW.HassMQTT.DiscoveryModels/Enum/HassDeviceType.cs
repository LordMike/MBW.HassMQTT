using System.Runtime.Serialization;

namespace MBW.HassMQTT.DiscoveryModels.Enum;

public enum HassDeviceType
{
    None,

    [EnumMember(Value = "alarm_control_panel")]
    AlarmControlPanel,

    [EnumMember(Value = "binary_sensor")]
    BinarySensor,

    [EnumMember(Value = "camera")]
    Camera,

    [EnumMember(Value = "cover")]
    Cover,

    [EnumMember(Value = "device_automation")]
    DeviceTrigger,

    [EnumMember(Value = "fan")]
    Fan,

    [EnumMember(Value = "climate")]
    Climate,

    [EnumMember(Value = "light")]
    Light,

    [EnumMember(Value = "lock")]
    Lock,

    [EnumMember(Value = "sensor")]
    Sensor,

    [EnumMember(Value = "switch")]
    Switch,

    [EnumMember(Value = "vacuum")]
    Vacuum,

    [EnumMember(Value = "tag")]
    TagScanner,

    [EnumMember(Value = "scene")]
    Scene,

    [EnumMember(Value = "number")]
    Number,

    [EnumMember(Value = "device_tracker")]
    DeviceTracker,

    [EnumMember(Value = "select")]
    Select,

    [EnumMember(Value = "humidifier")]
    Humidifier,

    [EnumMember(Value = "button")]
    Button,

    [EnumMember(Value = "siren")]
    Siren
}