namespace MBW.HassMQTT.DiscoveryModels.Enum;

public enum HassButtonDeviceClass
{
    /// <summary>
    /// Generic button. This is the default and doesn’t need to be set.
    ///</summary>
    None,

    /// <summary>
    /// The button restarts the device.
    /// </summary>
    Restart,

    /// <summary>
    /// The button updates the software of the device.
    /// </summary>
    Update
}