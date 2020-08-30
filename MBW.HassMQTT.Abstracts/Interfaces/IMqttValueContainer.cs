namespace MBW.HassMQTT.Abstracts.Interfaces
{
    public interface IMqttValueContainer
    {
        string PublishTopic { get; }
        bool Dirty { get; }

        void SetDirty(bool dirty = true);

        object GetSerializedValue(bool resetDirty);
    }
}
