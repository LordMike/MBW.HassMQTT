namespace MBW.HassMQTT.DiscoveryModels.Enum;

public enum HassUpdateDeviceClass
{
    /// <summary>
    /// A generic software update. This is the default and doesn’t need to be set.
    ///</summary>
    None,
    
    /// <summary>
    /// This update integration provides firmwares.
    ///</summary>
    Firmware,
}