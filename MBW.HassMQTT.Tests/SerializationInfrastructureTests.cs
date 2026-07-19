using System;
using System.Text.Json.Nodes;
using MBW.HassMQTT.DiscoveryModels.Models;
using MBW.HassMQTT.Serialization;
using Xunit;

namespace MBW.HassMQTT.Tests;

public class SerializationInfrastructureTests
{
    [Fact]
    public void BuiltInDiscoveryContractsAreSourceGenerated()
    {
        Assert.NotNull(HassJsonSerializerContext.Default.GetTypeInfo(typeof(MqttSensor)));
        Assert.NotNull(HassJsonSerializerContext.Default.GetTypeInfo(typeof(MqttWaterHeater)));
    }

    [Fact]
    public void RequiredEmptySelectOptionsAreSerialized()
    {
        MqttSelect document = new(string.Empty, "select-id") { CommandTopic = "select/set" };
        JsonObject json = JsonNode.Parse(DiscoveryJsonSerializer.Serialize(document))!.AsObject();

        Assert.True(json.ContainsKey("options"));
        Assert.Empty(json["options"]!.AsArray());
    }

    [Fact]
    public void ShippingAssembliesDoNotReferenceNewtonsoftJson()
    {
        Assert.DoesNotContain(typeof(MqttSensor).Assembly.GetReferencedAssemblies(), IsNewtonsoft);
        Assert.DoesNotContain(typeof(HassJson).Assembly.GetReferencedAssemblies(), IsNewtonsoft);
    }

    private static bool IsNewtonsoft(System.Reflection.AssemblyName reference) =>
        string.Equals(reference.Name, "Newtonsoft.Json", StringComparison.Ordinal);
}
