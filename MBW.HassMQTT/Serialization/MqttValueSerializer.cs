#nullable enable

using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;

namespace MBW.HassMQTT.Serialization;

internal static class MqttValueSerializer
{
    private static readonly JsonWriterOptions WriterOptions = new()
    {
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
    };

    public static byte[] SerializeStandalone(MqttValue value) => value.Kind switch
    {
        MqttValueKind.Null => Array.Empty<byte>(),
        MqttValueKind.String => Encoding.UTF8.GetBytes((string)value.Value!),
        _ => SerializeJsonValue(value)
    };

    public static byte[] SerializeJsonValue(MqttValue value) => Write(writer => WriteValue(writer, value));

    public static byte[] SerializeAttributes(IReadOnlyDictionary<string, MqttValue> attributes) => Write(writer =>
    {
        writer.WriteStartObject();
        foreach ((string name, MqttValue value) in attributes)
        {
            writer.WritePropertyName(name);
            WriteValue(writer, value);
        }
        writer.WriteEndObject();
    });

    public static byte[] SerializeCombined(MqttValue state, IReadOnlyDictionary<string, MqttValue> attributes) => Write(writer =>
    {
        writer.WriteStartObject();
        writer.WritePropertyName("state");
        WriteValue(writer, state);
        writer.WritePropertyName("attributes");
        writer.WriteStartObject();
        foreach ((string name, MqttValue value) in attributes)
        {
            writer.WritePropertyName(name);
            WriteValue(writer, value);
        }
        writer.WriteEndObject();
        writer.WriteEndObject();
    });

    public static bool JsonValueEquals(MqttValue left, MqttValue right) =>
        SerializeJsonValue(left).SequenceEqual(SerializeJsonValue(right));

    private static byte[] Write(Action<Utf8JsonWriter> write)
    {
        ArrayBufferWriter<byte> buffer = new();
        using Utf8JsonWriter writer = new(buffer, WriterOptions);
        write(writer);
        writer.Flush();
        return buffer.WrittenSpan.ToArray();
    }

    private static void WriteValue(Utf8JsonWriter writer, MqttValue value)
    {
        switch (value.Kind)
        {
            case MqttValueKind.Null:
                writer.WriteNullValue();
                return;
            case MqttValueKind.String:
                writer.WriteStringValue((string)value.Value!);
                return;
            case MqttValueKind.Boolean:
                writer.WriteBooleanValue((bool)value.Value!);
                return;
            case MqttValueKind.Json:
                ((JsonElement)value.Value!).WriteTo(writer);
                return;
            case MqttValueKind.Number:
                WriteNumber(writer, value.Value!);
                return;
            default:
                throw new InvalidOperationException("Unknown MQTT value kind");
        }
    }

    private static void WriteNumber(Utf8JsonWriter writer, object value)
    {
        switch (value)
        {
            case byte number: writer.WriteNumberValue(number); break;
            case sbyte number: writer.WriteNumberValue(number); break;
            case short number: writer.WriteNumberValue(number); break;
            case ushort number: writer.WriteNumberValue(number); break;
            case int number: writer.WriteNumberValue(number); break;
            case uint number: writer.WriteNumberValue(number); break;
            case long number: writer.WriteNumberValue(number); break;
            case ulong number: writer.WriteNumberValue(number); break;
            case float number: writer.WriteNumberValue(number); break;
            case double number: writer.WriteNumberValue(number); break;
            case decimal number: writer.WriteNumberValue(number); break;
            default: throw new InvalidOperationException($"Unsupported numeric MQTT value type {value.GetType().FullName}");
        }
    }
}
