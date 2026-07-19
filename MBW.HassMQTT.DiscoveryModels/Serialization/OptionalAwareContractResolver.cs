#nullable enable

using System;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace MBW.HassMQTT.DiscoveryModels.Serialization;

/// <summary>
/// Omits unset <see cref="Optional{T}" /> properties while preserving explicitly supplied JSON null values.
/// </summary>
public class OptionalAwareContractResolver : DefaultContractResolver
{
    /// <inheritdoc />
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
