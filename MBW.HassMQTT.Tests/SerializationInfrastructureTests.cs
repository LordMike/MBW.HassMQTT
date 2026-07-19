using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;
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
    public void SnapshotRestorationReplacesPrepopulatedWritableCollections()
    {
        MqttSelect source = new(string.Empty, "source");
        source.Options.Add("snapshot-option");
        byte[] snapshot = DiscoverySnapshotSerializer.Capture(source);

        MqttSelect target = new(string.Empty, "target");
        target.Options.Add("constructor-option");

        MqttSelect restored = DiscoverySnapshotSerializer.Restore(snapshot, () => target);

        Assert.Same(target, restored);
        Assert.Equal(new[] { "snapshot-option" }, restored.Options);
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
    public void RuntimeObservableStringCollectionsRemainArrays()
    {
        ObservableCollection<string> values = new() { "only-value" };

        JsonArray json = JsonNode.Parse(PayloadSerializer.Serialize(values))!.AsArray();

        Assert.Equal("only-value", Assert.Single(json)!.GetValue<string>());
    }

    [Fact]
    public void RuntimePayloadsIncludePublicFields()
    {
        PublicFieldPayload payload = new() { SampleValue = 42 };

        JsonObject json = JsonNode.Parse(PayloadSerializer.Serialize(payload))!.AsObject();

        Assert.Equal(42, json["sample_value"]!.GetValue<int>());
    }

    [Fact]
    public void RuntimeEnumerablePropertiesRemainPresentWhenEmpty()
    {
        EnumerablePayload payload = new() { Values = Array.Empty<int>() };

        JsonObject json = JsonNode.Parse(PayloadSerializer.Serialize(payload))!.AsObject();

        Assert.True(json.ContainsKey("values"));
        Assert.Empty(json["values"]!.AsArray());
    }

    [Fact]
    public void RuntimeFlagEnumsAndEnumDictionaryKeysRoundTrip()
    {
        RuntimeEnumPayload payload = new()
        {
            Access = RuntimeAccess.Read | RuntimeAccess.Write,
            Values = new Dictionary<RuntimeAccess, int> { [RuntimeAccess.Read] = 42 }
        };

        byte[] bytes = PayloadSerializer.Serialize(payload);
        JsonObject json = JsonNode.Parse(bytes)!.AsObject();

        Assert.Equal("read, write", json["access"]!.GetValue<string>());
        Assert.Equal(42, json["values"]!["read"]!.GetValue<int>());

        RuntimeEnumPayload restored = JsonSerializer.Deserialize<RuntimeEnumPayload>(bytes, HassJson.RuntimeOptions)!;
        Assert.Equal(payload.Access, restored.Access);
        Assert.Equal(42, restored.Values[RuntimeAccess.Read]);
    }

    [Fact]
    public void ShippingAssembliesDoNotReferenceNewtonsoftJson()
    {
        Assert.DoesNotContain(typeof(MqttSensor).Assembly.GetReferencedAssemblies(), IsNewtonsoft);
        Assert.DoesNotContain(typeof(HassJson).Assembly.GetReferencedAssemblies(), IsNewtonsoft);
    }

    private static bool IsNewtonsoft(System.Reflection.AssemblyName reference) =>
        string.Equals(reference.Name, "Newtonsoft.Json", StringComparison.Ordinal);

    private sealed class PublicFieldPayload
    {
        public int SampleValue;
    }

    private sealed class EnumerablePayload
    {
        public IEnumerable<int> Values { get; set; } = Array.Empty<int>();
    }

    private sealed class RuntimeEnumPayload
    {
        public RuntimeAccess Access { get; set; }
        public Dictionary<RuntimeAccess, int> Values { get; set; } = new();
    }

    [Flags]
    private enum RuntimeAccess
    {
        [EnumMember(Value = "read")]
        Read = 1,
        [EnumMember(Value = "write")]
        Write = 2
    }
}
