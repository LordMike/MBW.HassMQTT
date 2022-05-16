namespace MBW.HassMQTT.DiscoveryModels.Interfaces;

public enum EntityCategory
{
    /// <summary>
    /// Set to config for an entity which allows changing the configuration of a device, for example a switch entity making it possible to turn the background illumination of a switch on and off.
    /// </summary>
    Config,

    /// <summary>
    /// Set to diagnostic for an entity exposing some configuration parameter or diagnostics of a device but does not allow changing it, for example a sensor showing RSSI or MAC-address.
    /// </summary>
    Diagnostic,

    /// <summary>
    /// Set to system for an entity which is not useful for the user to interact with. As an example the auto generated energy cost sensors are not useful on their own because they reset from 0 every time home assistant is restarted or the energy settings are changed and thus have their entity category set to system.
    /// </summary>
    System,
}