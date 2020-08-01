using System;

namespace MBW.HassMQTT.CommonServices.MqttReconnect
{
    public class MqttReconnectionServiceConfig
    {
        public TimeSpan ReconnectInterval { get; set; } = TimeSpan.FromSeconds(30);
    }
}