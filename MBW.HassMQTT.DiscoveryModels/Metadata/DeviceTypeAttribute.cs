using System;
using JetBrains.Annotations;
using MBW.HassMQTT.DiscoveryModels.Enum;

namespace MBW.HassMQTT.DiscoveryModels.Metadata
{
    [MeansImplicitUse]
    [AttributeUsage(AttributeTargets.Class)]
    public class DeviceTypeAttribute : Attribute
    {
        public HassDeviceType DeviceType { get; }

        public DeviceTypeAttribute(HassDeviceType deviceType)
        {
            DeviceType = deviceType;
        }
    }
}