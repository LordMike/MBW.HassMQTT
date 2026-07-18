using System;
using JetBrains.Annotations;
using MBW.HassMQTT.DiscoveryModels.Enum;

namespace MBW.HassMQTT.DiscoveryModels.Metadata;

[MeansImplicitUse]
[AttributeUsage(AttributeTargets.Class)]
public class DeviceTypeAttribute : Attribute
{
    /// <summary>The MQTT discovery component type represented by the decorated model.</summary>
    public HassDeviceType DeviceType { get; }

    public DeviceTypeAttribute(HassDeviceType deviceType)
    {
        DeviceType = deviceType;
    }
}
