namespace MBW.HassMQTT.DiscoveryModels.Enum;

/// <summary>
/// How the sensor should interpreted for long term statistics
/// See https://developers.home-assistant.io/docs/core/entity/sensor#long-term-statistics
/// </summary>
public enum HassStateClass
{
    Unknown,

    /// <summary>
    /// For sensors with state_class total_increasing, a decreasing value is interpreted as the start of a new meter cycle or
    /// the replacement of the meter. It is important that the integration ensures that the value cannot erroneously decrease
    /// in the case of calculating a value from a sensor with measurement noise present. There is some tolerance, a decrease
    /// between state changes of &lt; 10% will not trigger a new meter cycle. This state class is useful for gas meters,
    /// electricity meters, water meters etc. The value when the sensor reading decreases will not be used as zero-point
    /// when calculating sum statistics, instead the zero-point will be set to 0.
    /// </summary>
    TotalIncreasing,

    /// <summary>
    /// For sensors with state_class measurement a total amount is calculated if:
    /// * The sensor's device class is DEVICE_CLASS_ENERGY, DEVICE_CLASS_GAS, or DEVICE_CLASS_MONETARY.
    /// * The sensor's last_reset property is set to a valid datatime. If the time of initialization is unknown and the meter will never reset, set to UNIX epoch 0: homeassistant.util.dt.utc_from_timestamp(0).
    ///
    /// A change of the last_reset attribute is interpreted as the start of a new meter cycle or the replacement of the meter.
    /// The sensor's value when last_reset changes will not be used as zero-point when calculating sum statistics, instead the
    /// zero-point will be set to 0.
    /// </summary>
    Measurement
}