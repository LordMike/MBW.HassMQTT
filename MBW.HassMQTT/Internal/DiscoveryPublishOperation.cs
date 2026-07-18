using System.Threading;

namespace MBW.HassMQTT.Internal;

internal sealed class DiscoveryPublishOperation
{
    private long _revision = 1;
    private long _publishedRevision;

    public string Topic { get; }
    public byte[] Payload { get; }
    public bool Dirty => Revision != Interlocked.Read(ref _publishedRevision);
    public long Revision => Interlocked.Read(ref _revision);

    public DiscoveryPublishOperation(string topic, byte[] payload)
    {
        Topic = topic;
        Payload = payload;
    }

    public void MarkDirty() => Interlocked.Increment(ref _revision);

    public void MarkPublished(long revision) => Interlocked.Exchange(ref _publishedRevision, revision);
}
