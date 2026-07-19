#nullable enable
using System;
using System.Globalization;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;
using MBW.HassMQTT.Serialization;

namespace MBW.HassMQTT.Tests.DiscoverySerialization;

internal static class DiscoveryJsonRoundTrip
{
    public static void Assert(Type modelType, string json) =>
        Assert(modelType.Name, modelType, json);

    public static void Assert(Type modelType, string json, bool _) =>
        Assert(modelType, json);

    public static void Assert(string caseName, Type modelType, string json)
    {
        JsonNode source = JsonNode.Parse(json)!;
        object? document = DiscoveryJsonSerializer.Deserialize(JsonSerializer.SerializeToUtf8Bytes(source), modelType);
        Xunit.Assert.NotNull(document);

        JsonNode actual = JsonNode.Parse(DiscoveryJsonSerializer.Serialize(document))!;
        JsonNode expected = source.DeepClone();
        JsonNode canonicalExpected = Canonicalize(expected);
        JsonNode canonicalActual = Canonicalize(actual);
        Xunit.Assert.True(
            JsonNode.DeepEquals(canonicalExpected, canonicalActual),
            $"Discovery JSON changed after a {modelType.Name} round trip for '{caseName}'.{Environment.NewLine}" +
            $"Expected:{Environment.NewLine}{expected}{Environment.NewLine}Actual:{Environment.NewLine}{actual}");
    }

    public static void Assert(string caseName, Type modelType, string json, bool _) =>
        Assert(caseName, modelType, json);

    private static JsonNode Canonicalize(JsonNode token)
    {
        if (token is JsonObject obj)
        {
            JsonObject result = new();
            foreach ((string name, JsonNode? child) in obj.OrderBy(property => property.Key, StringComparer.Ordinal))
            {
                JsonNode? normalized = child == null ? null : Canonicalize(child);
                if (name == "identifiers" && normalized is JsonArray { Count: 1 } identifiers)
                    normalized = identifiers[0]?.DeepClone();
                result[name] = normalized;
            }
            return result;
        }
        if (token is JsonArray array)
            return new JsonArray(array.Select(item => item == null ? null : Canonicalize(item)).ToArray());
        if (token is JsonValue value && value.TryGetValue<JsonElement>(out JsonElement element) && element.ValueKind == JsonValueKind.Number &&
            decimal.TryParse(element.GetRawText(), NumberStyles.Float, CultureInfo.InvariantCulture, out decimal number))
            return JsonValue.Create(number)!;
        return token.DeepClone();
    }
}
