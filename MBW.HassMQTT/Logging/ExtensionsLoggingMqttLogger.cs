using System;
using Microsoft.Extensions.Logging;
using MQTTnet.Diagnostics.Logger;

namespace MBW.HassMQTT.Logging;

internal sealed class ExtensionsLoggingMqttLogger : IMqttNetLogger
{
    private readonly ILogger _logger;

    public bool IsEnabled => true;

    public ExtensionsLoggingMqttLogger(ILoggerFactory loggerFactory, string source)
    {
        _logger = loggerFactory.CreateLogger(source);
    }

    public void Publish(MqttNetLogLevel logLevel, string source, string message, object[] parameters, Exception exception)
    {
        LogLevel level = logLevel switch
        {
            MqttNetLogLevel.Verbose => LogLevel.Trace,
            MqttNetLogLevel.Info => LogLevel.Information,
            MqttNetLogLevel.Warning => LogLevel.Warning,
            MqttNetLogLevel.Error => LogLevel.Error,
            _ => throw new ArgumentOutOfRangeException(nameof(logLevel), logLevel, null)
        };

        _logger.Log(level, exception, "{MqttSource}: {MqttMessage}", source, Format(message, parameters));
    }

    private static string Format(string message, object[] parameters) =>
        parameters is { Length: > 0 } ? string.Format(message, parameters) : message;
}
