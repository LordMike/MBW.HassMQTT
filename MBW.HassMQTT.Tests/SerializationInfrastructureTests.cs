using System;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using MBW.HassMQTT.DiscoveryModels.Interfaces;
using MBW.HassMQTT.DiscoveryModels.Metadata;
using MBW.HassMQTT.DiscoveryModels.Models;
using MBW.HassMQTT.Serialization;
using Xunit;

namespace MBW.HassMQTT.Tests;

public class SerializationInfrastructureTests
{
    [Fact]
    public void BuiltInDiscoveryContractsAreSourceGenerated()
    {
        Type[] discoveryTypes = typeof(MqttSensor).Assembly.GetTypes()
            .Where(type => !type.IsAbstract && typeof(IHassDiscoveryDocument).IsAssignableFrom(type))
            .ToArray();

        Assert.NotEmpty(discoveryTypes);
        foreach (Type discoveryType in discoveryTypes)
            Assert.NotNull(HassJsonSerializerContext.Default.GetTypeInfo(discoveryType));
    }

    [Fact]
    public void EmptyDeviceIsOmitted()
    {
        MqttSensor document = new(string.Empty, string.Empty) { StateTopic = "sensor/state" };
        JsonObject json = JsonNode.Parse(DiscoveryJsonSerializer.Serialize(document))!.AsObject();

        Assert.False(json.ContainsKey("device"));
    }

    [Theory]
    [InlineData("\"serial-one\"", "\"serial-one\"")]
    [InlineData("[\"serial-one\"]", "\"serial-one\"")]
    [InlineData("[\"serial-one\",\"serial-two\"]", "[\"serial-one\",\"serial-two\"]")]
    public void IdentifierScalarAndArrayFormsRoundTrip(string input, string expectedOutput)
    {
        byte[] json = Encoding.UTF8.GetBytes("{\"state_topic\":\"sensor/state\",\"device\":{\"identifiers\":" + input + "}}");
        MqttSensor document = DiscoveryJsonSerializer.Deserialize<MqttSensor>(json)!;
        JsonObject output = JsonNode.Parse(DiscoveryJsonSerializer.Serialize(document))!.AsObject();

        Assert.True(JsonNode.DeepEquals(JsonNode.Parse(expectedOutput), output["device"]!["identifiers"]));
    }

    [Theory]
    [InlineData("[]")]
    [InlineData("[\"mac\"]")]
    [InlineData("[\"mac\",\"id\",\"extra\"]")]
    [InlineData("[null,\"id\"]")]
    [InlineData("[\"mac\",null]")]
    public void InvalidConnectionTuplesAreRejected(string connection)
    {
        byte[] json = Encoding.UTF8.GetBytes("{\"state_topic\":\"sensor/state\",\"device\":{\"connections\":[" + connection + "]}}");

        Assert.Throws<JsonException>(() => DiscoveryJsonSerializer.Deserialize<MqttSensor>(json));
    }

    [Theory]
    [InlineData("-1")]
    [InlineData("3")]
    [InlineData("\"1\"")]
    [InlineData("1.5")]
    public void InvalidQosRepresentationsAreRejected(string qos)
    {
        byte[] json = Encoding.UTF8.GetBytes("{\"state_topic\":\"sensor/state\",\"qos\":" + qos + "}");

        Assert.Throws<JsonException>(() => DiscoveryJsonSerializer.Deserialize<MqttSensor>(json));
    }

    [Fact]
    public void SnapshotRestorationReplacesPrepopulatedDeviceCollections()
    {
        MqttSensor source = new(string.Empty, "source");
        source.Device.Identifiers.Add("snapshot-id");
        source.Device.Connections.Add(new ConnectionInfo { Type = "mac", Value = "snapshot-mac" });
        byte[] snapshot = DiscoverySnapshotSerializer.Capture(source);

        MqttSensor target = new(string.Empty, "target");
        target.Device.Identifiers.Add("constructor-id");
        target.Device.Connections.Add(new ConnectionInfo { Type = "mac", Value = "constructor-mac" });

        MqttSensor restored = DiscoverySnapshotSerializer.Restore(snapshot, () => target);

        Assert.Same(target, restored);
        Assert.Equal(new[] { "snapshot-id" }, restored.Device.Identifiers);
        ConnectionInfo connection = Assert.Single(restored.Device.Connections);
        Assert.Equal("mac", connection.Type);
        Assert.Equal("snapshot-mac", connection.Value);
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
