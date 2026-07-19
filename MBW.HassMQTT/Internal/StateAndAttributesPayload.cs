using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace MBW.HassMQTT.Internal;

internal sealed class StateAndAttributesPayload
{
    [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
    public object State { get; }

    [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
    public IReadOnlyDictionary<string, object> Attributes { get; }

    public StateAndAttributesPayload(object state, IReadOnlyDictionary<string, object> attributes)
    {
        State = state;
        Attributes = attributes;
    }
}
