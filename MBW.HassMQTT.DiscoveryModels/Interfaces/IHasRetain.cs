namespace MBW.HassMQTT.DiscoveryModels.Interfaces;

public interface IHasRetain
{
    /// <summary>
    /// Controls whether published command messages have the MQTT retain flag set.
    /// </summary>
    /// <remarks>The Home Assistant default is <see langword="false" />.</remarks>
    public bool? Retain { get; set; }
}
