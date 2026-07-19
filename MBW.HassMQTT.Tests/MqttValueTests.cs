#nullable enable

using System;
using System.Runtime.Serialization;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using MBW.HassMQTT.Serialization;
using Xunit;

namespace MBW.HassMQTT.Tests;

public class MqttValueTests
{
    [Theory]
    [InlineData(true, "true")]
    [InlineData(false, "false")]
    [InlineData((byte)1, "1")]
    [InlineData((sbyte)-2, "-2")]
    [InlineData((short)-3, "-3")]
    [InlineData((ushort)4, "4")]
    [InlineData(5, "5")]
    [InlineData(6U, "6")]
    [InlineData(7L, "7")]
    [InlineData(8UL, "8")]
    [InlineData(1.5f, "1.5")]
    [InlineData(2.5d, "2.5")]
    public void Scalar_values_have_stable_standalone_encoding(object input, string expected)
    {
        MqttValue value = input switch
        {
            bool item => item,
            byte item => item,
            sbyte item => item,
            short item => item,
            ushort item => item,
            int item => item,
            uint item => item,
            long item => item,
            ulong item => item,
            float item => item,
            double item => item,
            _ => throw new InvalidOperationException()
        };

        Assert.Equal(expected, Encoding.UTF8.GetString(MqttValueSerializer.SerializeStandalone(value)));
        Assert.Equal(input, value.GetValue());
    }

    [Fact]
    public void Strings_are_raw_standalone_and_escaped_in_json()
    {
        MqttValue value = "grøn \"value\"";

        Assert.Equal("grøn \"value\"", Encoding.UTF8.GetString(MqttValueSerializer.SerializeStandalone(value)));
        Assert.Equal("\"grøn \\\"value\\\"\"", Encoding.UTF8.GetString(MqttValueSerializer.SerializeJsonValue(value)));
    }

    [Fact]
    public void Semantic_null_is_empty_standalone_and_json_null_in_containers()
    {
        MqttValue value = (string?)null;

        Assert.Equal(MqttValue.Null, value);
        Assert.Empty(MqttValueSerializer.SerializeStandalone(value));
        Assert.Equal("null", Encoding.UTF8.GetString(MqttValueSerializer.SerializeJsonValue(value)));
    }

    [Fact]
    public void Decimal_and_temporal_values_preserve_their_intended_clr_value()
    {
        MqttValue decimalValue = 12.50m;
        MqttValue dateValue = new DateTimeOffset(2026, 7, 19, 12, 13, 14, TimeSpan.FromHours(2));

        Assert.Equal(12.50m, decimalValue.GetValue());
        Assert.Equal("12.50", Encoding.UTF8.GetString(MqttValueSerializer.SerializeStandalone(decimalValue)));
        Assert.Equal("2026-07-19T12:13:14.0000000+02:00", dateValue.GetValue());
    }

    [Fact]
    public void Json_values_are_owned_and_keep_json_semantics()
    {
        MqttValue value;
        using (JsonDocument document = JsonDocument.Parse("{\"items\":[1,true]}"))
            value = document.RootElement;

        JsonElement owned = Assert.IsType<JsonElement>(value.GetValue());
        Assert.Equal(2, owned.GetProperty("items").GetArrayLength());
        Assert.Equal("{\"items\":[1,true]}", Encoding.UTF8.GetString(MqttValueSerializer.SerializeStandalone(value)));
    }

    [Fact]
    public void Json_string_and_null_remain_distinct_from_semantic_string_and_null()
    {
        MqttValue jsonString = MqttValue.FromJson("\"value\"");
        MqttValue jsonNull = MqttValue.FromJson("null");

        Assert.Equal("\"value\"", Encoding.UTF8.GetString(MqttValueSerializer.SerializeStandalone(jsonString)));
        Assert.Equal("null", Encoding.UTF8.GetString(MqttValueSerializer.SerializeStandalone(jsonNull)));
        Assert.NotEqual((MqttValue)"value", jsonString);
        Assert.NotEqual(MqttValue.Null, jsonNull);
    }

    [Fact]
    public void Utf8_json_factory_and_combined_payload_support_structured_values()
    {
        MqttValue state = MqttValue.FromJson(Encoding.UTF8.GetBytes("[1,2]"));
        var attributes = new System.Collections.Generic.Dictionary<string, MqttValue>
        {
            ["quality"] = "good",
            ["details"] = MqttValue.FromJson("{\"ok\":true}"),
            ["missing"] = MqttValue.Null
        };

        JsonObject payload = JsonNode.Parse(MqttValueSerializer.SerializeCombined(state, attributes))!.AsObject();
        Assert.Equal(2, payload["state"]!.AsArray().Count);
        Assert.Equal("good", payload["attributes"]!["quality"]!.GetValue<string>());
        Assert.True(payload["attributes"]!["details"]!["ok"]!.GetValue<bool>());
        Assert.Null(payload["attributes"]!["missing"]);
    }

    [Fact]
    public void Enum_factory_uses_enum_member_snake_case_and_flags_wire_names()
    {
        Assert.Equal("custom", MqttValue.FromEnum(SampleEnum.CustomName).GetValue());
        Assert.Equal("plain_name", MqttValue.FromEnum(SampleEnum.PlainName).GetValue());
        Assert.Equal("read, write", MqttValue.FromEnum(SampleFlags.Read | SampleFlags.Write).GetValue());
        Assert.Throws<ArgumentOutOfRangeException>(() => MqttValue.FromEnum((SampleEnum)99));
    }

    [Fact]
    public void Invalid_values_are_rejected_at_construction()
    {
        Assert.ThrowsAny<JsonException>(() => MqttValue.FromJson("{"));
        Assert.Throws<ArgumentException>(() => ConvertJsonElement(default));
        Assert.Throws<ArgumentOutOfRangeException>(() => ConvertDouble(double.NaN));
        Assert.Throws<ArgumentOutOfRangeException>(() => ConvertDouble(double.PositiveInfinity));
        Assert.Throws<ArgumentOutOfRangeException>(() => ConvertFloat(float.NegativeInfinity));
    }

    [Fact]
    public void Numeric_equality_is_based_on_wire_value()
    {
        MqttValue integer = 1;
        MqttValue longInteger = 1L;
        MqttValue differentLexicalValue = 1.0m;

        Assert.Equal(integer, longInteger);
        Assert.Equal(integer.GetHashCode(), longInteger.GetHashCode());
        Assert.NotEqual(integer, differentLexicalValue);
    }

    private static MqttValue ConvertJsonElement(JsonElement value) => value;
    private static MqttValue ConvertDouble(double value) => value;
    private static MqttValue ConvertFloat(float value) => value;

    private enum SampleEnum
    {
        [EnumMember(Value = "custom")]
        CustomName,
        PlainName
    }

    [Flags]
    private enum SampleFlags
    {
        [EnumMember(Value = "read")]
        Read = 1,
        [EnumMember(Value = "write")]
        Write = 2
    }
}
