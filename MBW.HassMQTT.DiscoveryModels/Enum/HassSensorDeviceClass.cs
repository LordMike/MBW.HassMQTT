using System.Runtime.Serialization;

namespace MBW.HassMQTT.DiscoveryModels.Enum
{
    public enum HassSensorDeviceClass
    {
        /// <summary>
        /// Generic sensor. This is the default and doesn’t need to be set.
        ///</summary>
        None,

        /// <summary>
        /// Apparent power in VA
        ///</summary>
        [EnumMember(Value = "apparent_power")]
        ApparentPower,

        /// <summary>
        /// Air Quality Index
        ///</summary>
        [EnumMember(Value = "aqi")]
        AirQualityIndex,

        /// <summary>
        /// Percentage of battery that is left
        ///</summary>
        [EnumMember(Value = "battery")]
        Battery,

        /// <summary>
        /// Carbon Dioxide in CO2 (Smoke)
        ///</summary>
        [EnumMember(Value = "carbon_dioxide")]
        CarbonDioxide,

        /// <summary>
        /// Carbon Monoxide in CO (Gas CNG/LPG)
        ///</summary>
        [EnumMember(Value = "carbon_monoxide")]
        CarbonMonoxide,

        /// <summary>
        /// Current in A
        ///</summary>
        [EnumMember(Value = "current")]
        Current,

        /// <summary>
        /// Date string (ISO 8601)
        ///</summary>
        [EnumMember(Value = "date")]
        Date,

        /// <summary>
        /// Duration in days, hours, minutes or seconds
        ///</summary>
        [EnumMember(Value = "duration")]
        Duration,

        /// <summary>
        /// Energy in Wh or kWh
        ///</summary>
        [EnumMember(Value = "energy")]
        Energy,

        /// <summary>
        /// Frequency in Hz, kHz, MHz or GHz
        ///</summary>
        [EnumMember(Value = "frequency")]
        Frequency,

        /// <summary>
        /// Gasvolume in m³
        ///</summary>
        [EnumMember(Value = "gas")]
        Gas,

        /// <summary>
        /// Percentage of humidity in the air
        ///</summary>
        [EnumMember(Value = "humidity")]
        Humidity,

        /// <summary>
        /// The current light level in lx or lm
        ///</summary>
        [EnumMember(Value = "illuminance")]
        Illuminance,

        /// <summary>
        /// The monetary value
        ///</summary>
        [EnumMember(Value = "monetary")]
        Monetary,

        /// <summary>
        /// Concentration of Nitrogen Dioxide in µg/m³
        ///</summary>
        [EnumMember(Value = "nitrogen_dioxide")]
        NitrogenDioxide,

        /// <summary>
        /// Concentration of Nitrogen Monoxide in µg/m³
        ///</summary>
        [EnumMember(Value = "nitrogen_monoxide")]
        NitrogenMonoxide,

        /// <summary>
        /// Concentration of Nitrous Oxide in µg/m³
        ///</summary>
        [EnumMember(Value = "nitrous_oxide")]
        NitrousOxide,

        /// <summary>
        /// Concentration of Ozone in µg/m³
        ///</summary>
        [EnumMember(Value = "ozone")]
        Ozone,

        /// <summary>
        /// Concentration of particulate matter less than 1 micrometer in µg/m³
        ///</summary>
        [EnumMember(Value = "pm1")]
        Pm1,

        /// <summary>
        /// Concentration of particulate matter less than 10 micrometers in µg/m³
        ///</summary>
        [EnumMember(Value = "pm10")]
        Pm10,

        /// <summary>
        /// Concentration of particulate matter less than 2.5 micrometers in µg/m³
        ///</summary>
        [EnumMember(Value = "pm25")]
        Pm25,

        /// <summary>
        /// Power factor in %
        ///</summary>
        [EnumMember(Value = "power_factor")]
        PowerFactor,

        /// <summary>
        /// Power in W or kW
        ///</summary>
        [EnumMember(Value = "power")]
        Power,

        /// <summary>
        /// Pressure in hPa or mbar
        ///</summary>
        [EnumMember(Value = "pressure")]
        Pressure,

        /// <summary>
        /// Reactive power in var
        ///</summary>
        [EnumMember(Value = "reactive_power")]
        ReactivePower,

        /// <summary>
        /// Signal strength in dB or dBm
        ///</summary>
        [EnumMember(Value = "signal_strength")]
        SignalStrength,

        /// <summary>
        /// Concentration of sulphur dioxide in µg/m³
        ///</summary>
        [EnumMember(Value = "sulphur_dioxide")]
        SulphurDioxide,

        /// <summary>
        /// Temperature in °C or °F
        ///</summary>
        [EnumMember(Value = "temperature")]
        Temperature,

        /// <summary>
        /// Datetime object or timestamp string (ISO 8601)
        ///</summary>
        [EnumMember(Value = "timestamp")]
        Timestamp,

        /// <summary>
        /// Concentration of volatile organic compounds in µg/m³
        ///</summary>
        [EnumMember(Value = "volatile_organic_compounds")]
        VolatileOrganicCompounds,

        /// <summary>
        /// Voltage in V.
        ///</summary>
        [EnumMember(Value = "voltage")]
        Voltage,
    }
}