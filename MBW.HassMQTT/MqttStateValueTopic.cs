﻿using System;
using MBW.HassMQTT.Abstracts.Interfaces;
using MBW.HassMQTT.DiscoveryModels.Helpers;
using MBW.HassMQTT.Internal;

namespace MBW.HassMQTT;

public class MqttStateValueTopic : IMqttValueContainer
{
    private object _value;
    public string PublishTopic { get; }
    public bool Dirty { get; private set; }

    public object Value
    {
        get => _value;
        set
        {
            try
            {
                if (ComparisonHelper.IsSameValue(value, Value))
                    return;
            }
            catch (Exception e)
            {
                throw new InvalidOperationException("Unable to compare values for '" + PublishTopic + "'", e);
            }

            _value = value;
            Dirty = true;
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

    public void SetDirty(bool dirty = true)
    {
        Dirty = dirty;
    }

    public object GetSerializedValue(bool resetDirty)
    {
        if (resetDirty)
            Dirty = false;

        if (TryConvertStateValue(Value, out string asString))
            return asString;

        return Value;
    }
}