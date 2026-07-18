namespace MBW.HassMQTT;

public enum MqttFlushStatus
{
    Completed,
    Busy,
    Disconnected,
    Interrupted,
    BrokerRejected
}