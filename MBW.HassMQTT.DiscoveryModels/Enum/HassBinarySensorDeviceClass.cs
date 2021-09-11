using System.Runtime.Serialization;

namespace MBW.HassMQTT.DiscoveryModels.Enum
{
    public enum HassBinarySensorDeviceClass
    {
        /// <summary>
        /// Generic on/off. This is the default and doesn’t need to be set.
        ///</summary>
        None,

        /// <summary>
        /// on means low, off means normal
        ///</summary>
        [EnumMember(Value = "battery")]
        Battery,

        /// <summary>
        /// on means charging, off means not charging
        ///</summary>
        [EnumMember(Value = "battery_charging")]
        BatteryCharging,

        /// <summary>
        /// on means cold, off means normal
        ///</summary>
        [EnumMember(Value = "cold")]
        Cold,

        /// <summary>
        /// on means connected, off means disconnected
        ///</summary>
        [EnumMember(Value = "connectivity")]
        Connectivity,

        /// <summary>
        /// on means open, off means closed
        ///</summary>
        [EnumMember(Value = "door")]
        Door,

        /// <summary>
        /// on means open, off means closed
        ///</summary>
        [EnumMember(Value = "garage_door")]
        GarageDoor,

        /// <summary>
        /// on means gas detected, off means no gas (clear)
        ///</summary>
        [EnumMember(Value = "gas")]
        Gas,

        /// <summary>
        /// on means hot, off means normal
        ///</summary>
        [EnumMember(Value = "heat")]
        Heat,

        /// <summary>
        /// on means light detected, off means no light
        ///</summary>
        [EnumMember(Value = "light")]
        Light,

        /// <summary>
        /// on means open (unlocked), off means closed (locked)
        ///</summary>
        [EnumMember(Value = "lock")]
        Lock,

        /// <summary>
        /// on means moisture detected (wet), off means no moisture (dry)
        ///</summary>
        [EnumMember(Value = "moisture")]
        Moisture,

        /// <summary>
        /// on means motion detected, off means no motion (clear)
        ///</summary>
        [EnumMember(Value = "motion")]
        Motion,

        /// <summary>
        /// on means moving, off means not moving (stopped)
        ///</summary>
        [EnumMember(Value = "moving")]
        Moving,

        /// <summary>
        /// on means occupied, off means not occupied (clear)
        ///</summary>
        [EnumMember(Value = "occupancy")]
        Occupancy,

        /// <summary>
        /// on means open, off means closed
        ///</summary>
        [EnumMember(Value = "opening")]
        Opening,

        /// <summary>
        /// on means device is plugged in, off means device is unplugged
        ///</summary>
        [EnumMember(Value = "plug")]
        Plug,

        /// <summary>
        /// on means power detected, off means no power
        ///</summary>
        [EnumMember(Value = "power")]
        Power,

        /// <summary>
        /// on means home, off means away
        ///</summary>
        [EnumMember(Value = "presence")]
        Presence,

        /// <summary>
        /// on means problem detected, off means no problem (OK)
        ///</summary>
        [EnumMember(Value = "problem")]
        Problem,

        /// <summary>
        /// on means unsafe, off means safe
        ///</summary>
        [EnumMember(Value = "safety")]
        Safety,

        /// <summary>
        /// on means smoke detected, off means no smoke (clear)
        ///</summary>
        [EnumMember(Value = "smoke")]
        Smoke,

        /// <summary>
        /// on means sound detected, off means no sound (clear)
        ///</summary>
        [EnumMember(Value = "sound")]
        Sound,

        /// <summary>
        /// on means update available, off means up-to-date
        ///</summary>
        [EnumMember(Value = "update")]
        Update,

        /// <summary>
        /// on means vibration detected, off means no vibration (clear)
        ///</summary>
        [EnumMember(Value = "vibration")]
        Vibration,

        /// <summary>
        /// on means open, off means closed
        ///</summary>
        [EnumMember(Value = "window")]
        Window,
    }
}