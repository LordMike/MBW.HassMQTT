#nullable enable

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Text.Encodings.Web;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using MBW.HassMQTT.DiscoveryModels.Device;
using MBW.HassMQTT.DiscoveryModels.Metadata;
using MBW.HassMQTT.DiscoveryModels.Models;
using MBW.HassMQTT.Internal;

namespace MBW.HassMQTT.Serialization;

internal static class HassJson
{
    public static JsonSerializerOptions WireOptions { get; } = CreateWireOptions();
    public static JsonSerializerOptions RuntimeOptions { get; } = CreateRuntimeOptions();
    public static JsonSerializerOptions DeserializationOptions { get; } = CreateDeserializationOptions();
    public static JsonSerializerOptions SnapshotOptions { get; } = CreateSnapshotOptions();

    public static JsonSerializerOptions CreateSnapshotOptions(Type? rootType = null, Func<object>? rootFactory = null)
    {
        JsonSerializerOptions options = CreateBaseOptions(JsonIgnoreCondition.Never);
        ConfigureResolver(options, typeInfo => ModifyContract(typeInfo, false, true, rootType, rootFactory), false);
        return options;
    }

    private static JsonSerializerOptions CreateWireOptions()
    {
        JsonSerializerOptions options = CreateBaseOptions(JsonIgnoreCondition.WhenWritingDefault);
        ConfigureResolver(options, typeInfo => ModifyContract(typeInfo, true, false, null, null), true);
        return options;
    }

    private static JsonSerializerOptions CreateRuntimeOptions()
    {
        JsonSerializerOptions options = CreateBaseOptions(JsonIgnoreCondition.WhenWritingDefault);
        ConfigureResolver(options, ModifyRuntimeContract, true);
        return options;
    }

    private static JsonSerializerOptions CreateDeserializationOptions()
    {
        JsonSerializerOptions options = CreateBaseOptions(JsonIgnoreCondition.WhenWritingDefault);
        ConfigureResolver(options, typeInfo => ModifyContract(typeInfo, false, false, null, null), false);
        return options;
    }

    private static JsonSerializerOptions CreateBaseOptions(JsonIgnoreCondition ignoreCondition)
    {
        JsonSerializerOptions options = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
            PropertyNameCaseInsensitive = true,
            IncludeFields = true,
            DefaultIgnoreCondition = ignoreCondition,
            PreferredObjectCreationHandling = JsonObjectCreationHandling.Populate,
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        };
        options.Converters.Add(new QosLevelConverter());
        options.Converters.Add(new EnumMemberJsonConverterFactory());
        options.Converters.Add(new ConnectionInfoConverter());

        // TODO #16: register the Optional<T> converter here once Optional<T> lands.
        return options;
    }

    private static void ConfigureResolver(JsonSerializerOptions options, Action<JsonTypeInfo> modifier, bool useGeneratedContracts)
    {
        DefaultJsonTypeInfoResolver reflection = new();
        reflection.Modifiers.Add(modifier);
        if (!useGeneratedContracts)
        {
            options.TypeInfoResolver = reflection;
            return;
        }

        options.TypeInfoResolver = JsonTypeInfoResolver.Combine(
            HassJsonSerializerContext.Default.WithAddedModifier(modifier),
            reflection);
    }

    private static void ModifyContract(
        JsonTypeInfo typeInfo,
        bool omitEmptyCollections,
        bool replaceWritableCollections,
        Type? rootType,
        Func<object>? rootFactory)
    {
        if (rootType == typeInfo.Type && rootFactory != null)
            typeInfo.CreateObject = rootFactory;
        else if (TryCreateDiscoveryFactory(typeInfo.Type, out Func<object>? factory))
            typeInfo.CreateObject = factory;
        else if (typeInfo.Type == typeof(MqttDeviceDocument))
            typeInfo.CreateObject = () => Activator.CreateInstance(typeof(MqttDeviceDocument), true)!;

        if (typeInfo.Type == typeof(MqttDeviceDocument) && typeInfo.Kind == JsonTypeInfoKind.Object)
        {
            JsonPropertyInfo? identifiers = typeInfo.Properties.FirstOrDefault(property => property.Name == "identifiers");
            if (identifiers != null)
            {
                identifiers.CustomConverter = new IdentifierCollectionConverter();
                identifiers.ObjectCreationHandling = JsonObjectCreationHandling.Replace;
                identifiers.Set = (instance, value) => ReplaceCollection(
                    ((MqttDeviceDocument)instance).Identifiers,
                    (ObservableCollection<string>)value!);
            }

            JsonPropertyInfo? connections = typeInfo.Properties.FirstOrDefault(property => property.Name == "connections");
            if (connections != null)
            {
                connections.ObjectCreationHandling = JsonObjectCreationHandling.Replace;
                connections.Set = (instance, value) => ReplaceCollection(
                    ((MqttDeviceDocument)instance).Connections,
                    (ObservableCollection<ConnectionInfo>)value!);
            }
        }

        if (replaceWritableCollections && typeInfo.Kind == JsonTypeInfoKind.Object)
        {
            foreach (JsonPropertyInfo property in typeInfo.Properties)
            {
                if (property.Set != null && IsEnumerableType(property.PropertyType))
                    property.ObjectCreationHandling = JsonObjectCreationHandling.Replace;
            }
        }

        // TODO #16: customize Optional<T> property contracts here after its final API is known.
        if (!omitEmptyCollections || typeInfo.Kind != JsonTypeInfoKind.Object)
            return;

        foreach (JsonPropertyInfo property in typeInfo.Properties)
        {
            if (property.PropertyType == typeof(MqttDeviceDocument))
            {
                Func<object, object?>? deviceGetter = property.Get;
                if (deviceGetter != null)
                    property.ShouldSerialize = (instance, _) => deviceGetter(instance) is MqttDeviceDocument device && !IsEmpty(device);
                continue;
            }
            if (!IsEnumerableType(property.PropertyType))
                continue;
            if (typeInfo.Type == typeof(MqttSelect) && property.Name == "options")
                continue;
            if (typeInfo.Type == typeof(StateAndAttributesPayload) && property.Name == "attributes")
                continue;

            Func<object, object?>? getter = property.Get;
            if (getter != null)
                property.ShouldSerialize = (instance, _) => getter(instance) is IEnumerable values && values.GetEnumerator().MoveNext();
        }
    }

    private static void ModifyRuntimeContract(JsonTypeInfo typeInfo)
    {
        if (typeInfo.Kind != JsonTypeInfoKind.Object)
            return;

        foreach (JsonPropertyInfo property in typeInfo.Properties)
        {
            if (!IsCountedCollectionType(property.PropertyType))
                continue;

            Func<object, object?>? getter = property.Get;
            if (getter != null)
                property.ShouldSerialize = (instance, _) => HasItems(getter(instance));
        }
    }

    private static bool IsEnumerableType(Type type) =>
        type != typeof(string) && typeof(IEnumerable).IsAssignableFrom(type);

    private static bool IsCountedCollectionType(Type type)
    {
        if (typeof(ICollection).IsAssignableFrom(type))
            return true;
        if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(ICollection<>))
            return true;
        return type.GetInterfaces().Any(@interface =>
            @interface.IsGenericType && @interface.GetGenericTypeDefinition() == typeof(ICollection<>));
    }

    private static bool HasItems(object? value)
    {
        if (value is ICollection collection)
            return collection.Count > 0;
        if (value == null)
            return false;

        Type? collectionInterface = value.GetType().GetInterfaces().FirstOrDefault(@interface =>
            @interface.IsGenericType && @interface.GetGenericTypeDefinition() == typeof(ICollection<>));
        return collectionInterface?.GetProperty(nameof(ICollection<object>.Count))?.GetValue(value) is int count && count > 0;
    }

    private static bool IsEmpty(MqttDeviceDocument device) =>
        device.Connections.Count == 0 && device.Identifiers.Count == 0 &&
        device.ConfigurationUrl == null && device.Manufacturer == null && device.Model == null &&
        device.ModelId == null && device.Name == null && device.SerialNumber == null &&
        device.SuggestedArea == null && device.SwVersion == null && device.HwVersion == null &&
        device.ViaDevice == null;

    private static void ReplaceCollection<T>(ObservableCollection<T> target, ObservableCollection<T> source)
    {
        while (target.Count > 0)
            target.RemoveAt(target.Count - 1);
        foreach (T item in source)
            target.Add(item);
    }

    private static bool TryCreateDiscoveryFactory(Type type, out Func<object>? factory)
    {
        if (type.Namespace == typeof(MqttSensor).Namespace && type.Name.StartsWith("Mqtt", StringComparison.Ordinal))
        {
            ConstructorInfo? constructor = type.GetConstructor(new[] { typeof(string), typeof(string) });
            if (constructor != null)
            {
                factory = () => constructor.Invoke(new object?[] { null, null });
                return true;
            }
        }
        factory = null;
        return false;
    }
}
