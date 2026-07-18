using System;
using System.Collections.Generic;
using System.Threading;
using MBW.HassMQTT.Abstracts.Interfaces;
using MBW.HassMQTT.DiscoveryModels.Helpers;

namespace MBW.HassMQTT;

public class MqttAttributesTopic : IMqttValueContainer
{
    private readonly object _syncRoot = new object();
    private readonly Dictionary<string, object> _attributes = new Dictionary<string, object>();
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

    public void SetAttribute(string name, object value)
    {
        lock (_syncRoot)
        {
            if (value == default)
            {
                if (_attributes.Remove(name))
                    Interlocked.Increment(ref _revision);
                return;
            }

            if (_attributes.TryGetValue(name, out object existing))
            {
                try
                {
                    if (ComparisonHelper.IsSameValue(existing, value))
                        return;
                }
                catch (Exception e)
                {
                    throw new InvalidOperationException($"Unable to compare values for '{PublishTopic}'", e);
                }
            }

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

    public object GetSerializedValue()
    {
        lock (_syncRoot)
            return new Dictionary<string, object>(_attributes);
    }
}
