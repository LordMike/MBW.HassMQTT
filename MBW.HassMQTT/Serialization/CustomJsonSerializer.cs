﻿using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace MBW.HassMQTT.Serialization;

public static class CustomJsonSerializer
{
    public static JsonSerializer Serializer { get; }

    static CustomJsonSerializer()
    {
        SnakeCaseNamingStrategy namingStrategy = new SnakeCaseNamingStrategy();
            
        JsonSerializerSettings settings = new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore,
            DefaultValueHandling = DefaultValueHandling.Ignore,
            ContractResolver = new DoNotSerializeEmptyListsResolver { NamingStrategy = namingStrategy }
        };

        settings.Converters.Add(new QosLevelConverter());
        settings.Converters.Add(new StringEnumConverter(namingStrategy));
        settings.Converters.Add(new ConnectionInfoConverter());

        Serializer = JsonSerializer.Create(settings);
    }
}