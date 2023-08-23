using System;
using System.Security.Authentication;

namespace MBW.HassMQTT.CommonServices;

public class CommonMqttConfiguration
{
    public string Server { get; set; } = "localhost";

    public int Port { get; set; } = 1883;

    public string Username { get; set; }

    public string Password { get; set; }

    public string ClientId { get; set; } = "HASSMQTT-unset";

    public TimeSpan ReconnectInterval { get; set; } = TimeSpan.FromSeconds(5);

    public TimeSpan? KeepAlivePeriod { get; set; }

    public bool EnableTls { get; set; }
    public SslProtocols TlsProtocols { get; set; } = SslProtocols.Default;
    public bool TlsAllowUntrustedCertificates { get; set; }
    public bool TlsIgnoreCertificateChainErrors { get; set; }
    public bool TlsIgnoreCertificateRevocationErrors { get; set; }
}