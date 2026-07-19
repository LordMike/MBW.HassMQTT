using System;
using System.Collections.Generic;
using System.Threading;

namespace MBW.HassMQTT;

public class MqttAttributesTopic
{
    private readonly object _syncRoot = new object();
    private readonly Dictionary<string, MqttValue> _attributes = new Dictionary<string, MqttValue>();
    private long _revision;
    private long _publishedRevision;
    private bool _initialized;

    public string PublishTopic { get; }
    public bool Dirty => Revision != Interlocked.Read(ref _publishedRevision);
    public long Revision => Interlocked.Read(ref _revision);

    public MqttAttributesTopic(string topic)
    {
        PublishTopic = topic;
    }

    public void RemoveAttribute(string name)
    {
        lock (_syncRoot)
        {
            if (_attributes.Remove(name))
                Interlocked.Increment(ref _revision);
        }
    }

    public void SetAttribute(string name, MqttValue value)
    {
        lock (_syncRoot)
        {
            if (_attributes.TryGetValue(name, out MqttValue existing) && existing == value)
                return;

            _attributes[name] = value;
            _initialized = true;
            Interlocked.Increment(ref _revision);
        }
    }

    public void Clear()
    {
        lock (_syncRoot)
        {
            if (_attributes.Count == 0)
                return;

            _attributes.Clear();
            Interlocked.Increment(ref _revision);
        }
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
            return new Snapshot(new Dictionary<string, MqttValue>(_attributes), _revision);
    }

    internal readonly record struct Snapshot(
        IReadOnlyDictionary<string, MqttValue> Values,
        long Revision);
}
