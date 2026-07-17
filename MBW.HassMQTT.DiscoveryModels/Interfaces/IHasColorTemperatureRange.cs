#nullable enable

namespace MBW.HassMQTT.DiscoveryModels.Interfaces;

/// <summary>
/// Describes the color-temperature units and bounds supported by an MQTT light.
/// </summary>
public interface IHasColorTemperatureRange
{
    /// <summary>
    /// Whether color-temperature commands and state values use Kelvin instead of mireds. The documented default is <see langword="false" />.
    /// </summary>
    bool? ColorTempKelvin { get; set; }

    /// <summary>The maximum supported color temperature in Kelvin. The documented default is 6535 K.</summary>
    int? MaxKelvin { get; set; }

    /// <summary>The maximum supported color temperature in mireds.</summary>
    int? MaxMireds { get; set; }

    /// <summary>The minimum supported color temperature in Kelvin. The documented default is 2000 K.</summary>
    int? MinKelvin { get; set; }

    /// <summary>The minimum supported color temperature in mireds.</summary>
    int? MinMireds { get; set; }
}
