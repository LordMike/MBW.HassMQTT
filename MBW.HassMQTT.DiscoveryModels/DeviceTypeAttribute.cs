using System;
using MBW.HassMQTT.DiscoveryModels.Enum;

namespace MBW.HassMQTT.DiscoveryModels
{
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