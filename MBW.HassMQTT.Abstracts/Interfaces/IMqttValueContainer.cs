namespace MBW.HassMQTT.Abstracts.Interfaces
{
    public interface IMqttValueContainer
    {
        string PublishTopic { get; }
        bool Dirty { get; }

        void SetDirty();

        object GetSerializedValue(bool resetDirty);
    }
}
