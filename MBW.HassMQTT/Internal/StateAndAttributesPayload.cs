using System.Collections.Generic;
using Newtonsoft.Json;

namespace MBW.HassMQTT.Internal;

internal sealed class StateAndAttributesPayload
{
    [JsonProperty("state", NullValueHandling = NullValueHandling.Include, DefaultValueHandling = DefaultValueHandling.Include)]
    public object State { get; }

    [JsonProperty("attributes", NullValueHandling = NullValueHandling.Include, DefaultValueHandling = DefaultValueHandling.Include)]
    public IReadOnlyDictionary<string, object> Attributes { get; }

    public StateAndAttributesPayload(object state, IReadOnlyDictionary<string, object> attributes)
    {
        State = state;
        Attributes = attributes;
    }
}
