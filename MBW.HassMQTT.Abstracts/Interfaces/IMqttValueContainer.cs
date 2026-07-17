namespace MBW.HassMQTT.Abstracts.Interfaces;

public interface IMqttValueContainer
{
    string PublishTopic { get; }
    bool Dirty { get; }
    long Revision { get; }

    void MarkDirty();
    void MarkPublished(long revision);
    object GetSerializedValue();
}
