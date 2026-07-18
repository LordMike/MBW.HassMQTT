using System;
using System.Threading;
using MBW.HassMQTT.Abstracts.Interfaces;
using MBW.HassMQTT.DiscoveryModels.Helpers;
using MBW.HassMQTT.Internal;

namespace MBW.HassMQTT;

public class MqttStateValueTopic : IMqttValueContainer
{
    private readonly object _syncRoot = new object();
    private object _value;
    private long _revision;
    private long _publishedRevision;
    private bool _initialized;

    public string PublishTopic { get; }
    public bool Dirty => Revision != Interlocked.Read(ref _publishedRevision);
    public long Revision => Interlocked.Read(ref _revision);

    public object Value
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
                try
                {
                    if (_initialized && ComparisonHelper.IsSameValue(value, _value))
                        return;
                }
                catch (Exception e)
                {
                    throw new InvalidOperationException("Unable to compare values for '" + PublishTopic + "'", e);
                }

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

    private static bool TryConvertStateValue(object val, out string str)
    {
        switch (val)
        {
            case DateTime asDateTime:
                str = asDateTime.ToIso8601();
                return true;
            case DateTimeOffset asDateTimeOffset:
                str = asDateTimeOffset.ToIso8601();
                return true;
            case string asString:
                str = asString;
                return true;
            default:
                str = null;
                return false;
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
        object value;
        lock (_syncRoot)
            value = _value;

        return TryConvertStateValue(value, out string asString) ? asString : value;
    }
}
