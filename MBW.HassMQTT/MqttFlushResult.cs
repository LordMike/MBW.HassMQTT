namespace MBW.HassMQTT;

public enum MqttFlushStatus
{
    Completed,
    Busy,
    Disconnected,
    Interrupted,
    BrokerRejected
}

public sealed record MqttFlushResult(
    MqttFlushStatus Status,
    int DiscoveryDocuments = 0,
    int RemovedTopics = 0,
    int Values = 0,
    int Attributes = 0);
