using System;
using System.Collections.Generic;
using System.Linq;
using MBW.HassMQTT.Abstracts.Interfaces;
using MBW.HassMQTT.DiscoveryModels.Helpers;

namespace MBW.HassMQTT;

public class MqttAttributesTopic : IMqttValueContainer
{
    public string PublishTopic { get; }
    public bool Dirty { get; private set; }

    private readonly Dictionary<string, object> _attributes;

    public MqttAttributesTopic(string topic)
    {
        PublishTopic = topic;
        _attributes = new Dictionary<string, object>();
    }

    public void RemoveAttribute(string name)
    {
        if (_attributes.Remove(name))
            Dirty = true;
    }

    public void SetAttribute(string name, object value)
    {
        if (value == default)
        {
            if (_attributes.Remove(name))
                Dirty = true;

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

            return;
        }

        _attributes[name] = value;
        Dirty = true;
    }

    public void Clear()
    {
        if (!_attributes.Any())
            return;

        _attributes.Clear();
        Dirty = true;
    }

    public void SetDirty(bool dirty = true)
    {
        Dirty = dirty;
    }

    public object GetSerializedValue(bool resetDirty)
    {
        if (resetDirty)
            Dirty = false;

        return _attributes;
    }
}