using System;
using System.Threading;

namespace MBW.HassMQTT;

public class MqttStateValueTopic
{
    private readonly object _syncRoot = new object();
    private MqttValue _value;
    private long _revision;
    private long _publishedRevision;
    private bool _initialized;

    public string PublishTopic { get; }
    public bool Dirty => Revision != Interlocked.Read(ref _publishedRevision);
    public long Revision => Interlocked.Read(ref _revision);
    internal bool Initialized
    {
        get
        {
            lock (_syncRoot)
                return _initialized;
        }
    }

    public MqttValue Value
    {
        get
        {
            lock (_syncRoot)
                return _value;
        }
        set
        {
            lock (_syncRoot)
            {
                if (_initialized && value == _value)
                    return;

                _value = value;
                _initialized = true;
                Interlocked.Increment(ref _revision);
            }
        }
    }

    public MqttStateValueTopic(string topic)
    {
        PublishTopic = topic;
    }

    public void MarkDirty()
    {
        lock (_syncRoot)
        {
            if (_initialized)
                Interlocked.Increment(ref _revision);
        }
    }

    public void MarkPublished(long revision) => Interlocked.Exchange(ref _publishedRevision, revision);

    internal Snapshot Capture()
    {
        lock (_syncRoot)
            return new Snapshot(_value, _revision, _initialized);
    }

    internal readonly record struct Snapshot(MqttValue Value, long Revision, bool Initialized);
}
