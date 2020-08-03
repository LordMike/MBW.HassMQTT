using System.Runtime.Serialization;

namespace MBW.HassMQTT.DiscoveryModels.Enum
{
    public enum HassTopicKind
    {
        [EnumMember(Value = "action")]
        Action,

        [EnumMember(Value = "aux_command")]
        AuxCommand,

        [EnumMember(Value = "aux_state")]
        AuxState,

        [EnumMember(Value = "availability")]
        Availability,

        [EnumMember(Value = "away_mode_command")]
        AwayModeCommand,

        [EnumMember(Value = "away_mode_state")]
        AwayModeState,

        [EnumMember(Value = "battery_level")]
        BatteryLevel,

        [EnumMember(Value = "brightness_command")]
        BrightnessCommand,

        [EnumMember(Value = "brightness_state")]
        BrightnessState,

        [EnumMember(Value = "charging")]
        Charging,

        [EnumMember(Value = "cleaning")]
        Cleaning,

        [EnumMember(Value = "color_temp_command")]
        ColorTempCommand,

        [EnumMember(Value = "color_temp_state")]
        ColorTempState,

        [EnumMember(Value = "command")]
        Command,

        [EnumMember(Value = "current_temperature")]
        CurrentTemperature,

        [EnumMember(Value = "docked")]
        Docked,

        [EnumMember(Value = "effect_command")]
        EffectCommand,

        [EnumMember(Value = "effect_state")]
        EffectState,

        [EnumMember(Value = "error")]
        Error,

        [EnumMember(Value = "fan_mode_command")]
        FanModeCommand,

        [EnumMember(Value = "fan_mode_state")]
        FanModeState,

        [EnumMember(Value = "fan_speed")]
        FanSpeed,

        [EnumMember(Value = "hold_command")]
        HoldCommand,

        [EnumMember(Value = "hold_state")]
        HoldState,

        [EnumMember(Value = "hs_command")]
        HsCommand,

        [EnumMember(Value = "hs_state")]
        HsState,

        [EnumMember(Value = "attributes")]
        JsonAttributes,

        [EnumMember(Value = "mode_command")]
        ModeCommand,

        [EnumMember(Value = "mode_state")]
        ModeState,

        [EnumMember(Value = "oscillation_command")]
        OscillationCommand,

        [EnumMember(Value = "oscillation_state")]
        OscillationState,

        [EnumMember(Value = "position")]
        Position,

        [EnumMember(Value = "power_command")]
        PowerCommand,

        [EnumMember(Value = "rgb_command")]
        RgbCommand,

        [EnumMember(Value = "rgb_state")]
        RgbState,

        [EnumMember(Value = "send_command")]
        SendCommand,

        [EnumMember(Value = "set_fan_speed")]
        SetFanSpeed,

        [EnumMember(Value = "setposition")]
        Setposition,

        [EnumMember(Value = "speed_command")]
        SpeedCommand,

        [EnumMember(Value = "speed_state")]
        SpeedState,

        [EnumMember(Value = "state")]
        State,

        [EnumMember(Value = "swing_mode_command")]
        SwingModeCommand,

        [EnumMember(Value = "swing_mode_state")]
        SwingModeState,

        [EnumMember(Value = "temperature_command")]
        TemperatureCommand,

        [EnumMember(Value = "temperature_high_command")]
        TemperatureHighCommand,

        [EnumMember(Value = "temperature_high_state")]
        TemperatureHighState,

        [EnumMember(Value = "temperature_low_command")]
        TemperatureLowCommand,

        [EnumMember(Value = "temperature_low_state")]
        TemperatureLowState,

        [EnumMember(Value = "temperature_state")]
        TemperatureState,

        [EnumMember(Value = "tilt_command")]
        TiltCommand,

        [EnumMember(Value = "tilt_status")]
        TiltStatus,

        [EnumMember(Value = "white_value_command")]
        WhiteValueCommand,

        [EnumMember(Value = "white_value_state")]
        WhiteValueState,

        [EnumMember(Value = "xy_command")]
        XyCommand,

        [EnumMember(Value = "xy_state")]
        XyState
    }
}