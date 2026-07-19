#nullable enable
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using MBW.HassMQTT.DiscoveryModels.Device;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace MBW.HassMQTT.Serialization;

internal class DoNotSerializeEmptyListsResolver : OptionalAwareContractResolver
{
    private static readonly Type GenericCollectionType = typeof(ICollection<>);

    protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
    {
        JsonProperty jsonProperty = base.CreateProperty(member, memberSerialization);

        // Avoid serializing empty lists
        if (jsonProperty.PropertyType != null && IsICollection(jsonProperty.PropertyType))
        {
            jsonProperty.ShouldSerialize = instance =>
            {
                // Value provider should always be non-null
                object? lst = jsonProperty.ValueProvider!.GetValue(instance);
                if (lst == null)
                    return false;

                // We should only be called if we _can_ cast to ICollection. All other cases should throw or be fixed.
                return ((ICollection)lst).Count > 0;
            };
        }
        else if (jsonProperty.PropertyType == typeof(MqttDeviceDocument))
        {
            jsonProperty.ShouldSerialize = instance =>
            {
                MqttDeviceDocument? device = jsonProperty.ValueProvider!.GetValue(instance) as MqttDeviceDocument;
                return device != null && !IsEmpty(device);
            };
        }

        return jsonProperty;
    }

    private static bool IsEmpty(MqttDeviceDocument device)
    {
        return device.Connections.Count == 0 &&
               device.Identifiers.Count == 0 &&
               device.ConfigurationUrl == null &&
               device.Manufacturer == null &&
               device.Model == null &&
               device.ModelId == null &&
               device.Name == null &&
               device.SerialNumber == null &&
               device.SuggestedArea == null &&
               device.SwVersion == null &&
               device.HwVersion == null &&
               device.ViaDevice == null;
    }

    private bool IsICollection(Type type)
    {
        if (typeof(ICollection).IsAssignableFrom(type))
            return true;

        if (type.IsGenericType)
        {
            Type genericType = type.GetGenericTypeDefinition();
            if (genericType == GenericCollectionType)
                return true;
        }

        // ICollection<> does not implement ICollection.. :|
        Type[] interfaces = type.GetInterfaces();
        foreach (Type @interface in interfaces)
        {
            if (@interface.IsGenericType && @interface.GetGenericTypeDefinition() == GenericCollectionType)
                return true;
        }

        return false;
    }
}
