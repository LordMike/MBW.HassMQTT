#nullable enable

using System;
using System.Reflection;
using MBW.HassMQTT.DiscoveryModels;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace MBW.HassMQTT.Serialization;

internal class OptionalAwareContractResolver : DefaultContractResolver
{
    protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
    {
        JsonProperty jsonProperty = base.CreateProperty(member, memberSerialization);
        Type? propertyType = jsonProperty.PropertyType;
        if (propertyType == null || !propertyType.IsGenericType || propertyType.GetGenericTypeDefinition() != typeof(Optional<>))
            return jsonProperty;

        Predicate<object>? existingShouldSerialize = jsonProperty.ShouldSerialize;
        jsonProperty.ShouldSerialize = instance =>
        {
            if (existingShouldSerialize?.Invoke(instance) == false)
                return false;

            object? optional = jsonProperty.ValueProvider!.GetValue(instance);
            return optional != null && (bool)propertyType.GetProperty(nameof(Optional<object>.IsSet))!.GetValue(optional)!;
        };
        jsonProperty.NullValueHandling = NullValueHandling.Include;

        return jsonProperty;
    }
}
