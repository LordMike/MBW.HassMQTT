#nullable enable

using System;
using System.Collections.Concurrent;
using System.Reflection;
using MBW.HassMQTT.DiscoveryModels;
using Newtonsoft.Json;

namespace MBW.HassMQTT.Serialization;

internal sealed class OptionalJsonConverter : JsonConverter
{
    private static readonly ConcurrentDictionary<Type, Func<object?, object>> Factories = new();

    public override bool CanConvert(Type objectType)
    {
        return objectType.IsGenericType && objectType.GetGenericTypeDefinition() == typeof(Optional<>);
    }

    public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
    {
        if (value == null)
            throw new JsonSerializationException("An Optional<T> value cannot be null.");

        Type optionalType = value.GetType();
        bool isSet = (bool)optionalType.GetProperty(nameof(Optional<object>.IsSet))!.GetValue(value)!;
        if (!isSet)
            throw new JsonSerializationException("An unset Optional<T> must be omitted by the contract resolver.");

        Type valueType = optionalType.GetGenericArguments()[0];
        object? innerValue = optionalType.GetProperty(nameof(Optional<object>.Value))!.GetValue(value);
        serializer.Serialize(writer, innerValue, valueType);
    }

    public override object ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
    {
        Type valueType = objectType.GetGenericArguments()[0];
        if (reader.TokenType == JsonToken.Null && valueType.IsValueType && Nullable.GetUnderlyingType(valueType) == null)
            throw new JsonSerializationException($"Cannot assign null to {objectType} because {valueType} is not nullable.");

        object? innerValue = serializer.Deserialize(reader, valueType);
        return Factories.GetOrAdd(valueType, CreateFactory)(innerValue);
    }

    private static Func<object?, object> CreateFactory(Type valueType)
    {
        MethodInfo method = typeof(OptionalJsonConverter)
            .GetMethod(nameof(CreateFactoryCore), BindingFlags.NonPublic | BindingFlags.Static)!
            .MakeGenericMethod(valueType);

        return (Func<object?, object>)method.Invoke(null, null)!;
    }

    private static Func<object?, object> CreateFactoryCore<T>()
    {
        return value => Optional<T>.FromValue((T)value!);
    }
}
