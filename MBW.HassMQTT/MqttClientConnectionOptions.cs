using System;
using MQTTnet;

namespace MBW.HassMQTT;

public sealed class MqttClientConnectionOptions
{
    public MqttClientOptions ClientOptions { get; set; }
    public TimeSpan ConnectTimeout { get; set; } = TimeSpan.FromSeconds(10);
    public TimeSpan ReconnectInterval { get; set; } = TimeSpan.FromSeconds(5);
    public TimeSpan MaximumReconnectInterval { get; set; } = TimeSpan.FromMinutes(1);
    public TimeSpan ReconnectWarningInterval { get; set; } = TimeSpan.FromMinutes(15);
}
