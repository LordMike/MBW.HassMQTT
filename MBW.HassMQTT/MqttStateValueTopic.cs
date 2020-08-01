using System;
using MBW.HassMQTT.Abstracts.Interfaces;
using MBW.HassMQTT.DiscoveryModels.Helpers;

namespace MBW.HassMQTT
{
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
                if (ComparisonHelper.IsSameValue(value, Value))
                    return;

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
                    str = asDateTime.ToString("O");
                    return true;
                case string asString:
                    str = asString;
                    return true;
                default:
                    str = null;
                    return false;
            }
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
}