using System;

namespace MBW.HassMQTT.CommonServices
{
    public class CommonMqttConfiguration
    {
        public string Server { get; set; } = "localhost";

        public int Port { get; set; } = 1883;

        public string Username { get; set; }

        public string Password { get; set; }

        public string ClientId { get; set; } = "HASSMQTT-unset";

        public TimeSpan ReconnectInterval { get; set; } = TimeSpan.FromSeconds(30);

        public TimeSpan? KeepAlivePeriod { get; set; }
    }
}
