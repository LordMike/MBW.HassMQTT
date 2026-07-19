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
    public static void Assert(Type modelType, string json, bool normalizeExplicitNulls = false) =>
        Assert(modelType.Name, modelType, json, normalizeExplicitNulls);

    public static void Assert(string caseName, Type modelType, string json, bool normalizeExplicitNulls = false)
    {
        JsonNode source = JsonNode.Parse(json)!;
        object? document = DiscoveryJsonSerializer.Deserialize(JsonSerializer.SerializeToUtf8Bytes(source), modelType);
        Xunit.Assert.NotNull(document);

        JsonNode actual = JsonNode.Parse(DiscoveryJsonSerializer.Serialize(document))!;
        JsonNode expected = source.DeepClone();
        if (normalizeExplicitNulls)
            RemoveNullProperties(expected);

        JsonNode canonicalExpected = Canonicalize(expected);
        JsonNode canonicalActual = Canonicalize(actual);
        Xunit.Assert.True(
            JsonNode.DeepEquals(canonicalExpected, canonicalActual),
            $"Discovery JSON changed after a {modelType.Name} round trip for '{caseName}'.{Environment.NewLine}" +
            $"Expected:{Environment.NewLine}{expected}{Environment.NewLine}Actual:{Environment.NewLine}{actual}");
    }

    private static void RemoveNullProperties(JsonNode token)
    {
        if (token is JsonObject obj)
        {
            foreach (string name in obj.Where(property => property.Value is null).Select(property => property.Key).ToList())
                obj.Remove(name);
            foreach ((_, JsonNode? value) in obj)
                if (value != null) RemoveNullProperties(value);
        }
        else if (token is JsonArray array)
        {
            foreach (JsonNode? item in array)
                if (item != null) RemoveNullProperties(item);
        }
    }

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
