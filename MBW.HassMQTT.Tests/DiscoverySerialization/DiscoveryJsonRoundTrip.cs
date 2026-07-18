#nullable enable
using System;
using System.Globalization;
using System.Linq;
using MBW.HassMQTT.Serialization;
using Newtonsoft.Json.Linq;

namespace MBW.HassMQTT.Tests.DiscoverySerialization;

internal static class DiscoveryJsonRoundTrip
{
    public static void Assert(Type modelType, string json, bool normalizeExplicitNulls = false)
    {
        Assert(modelType.Name, modelType, json, normalizeExplicitNulls);
    }

    public static void Assert(string caseName, Type modelType, string json, bool normalizeExplicitNulls = false)
    {
        JToken source = JToken.Parse(json);
        object? document = source.ToObject(modelType, CustomJsonSerializer.Serializer);
        Xunit.Assert.NotNull(document);

        JToken actual = JToken.FromObject(document, CustomJsonSerializer.Serializer);
        JToken expected = source.DeepClone();

        if (normalizeExplicitNulls)
        {
            // https://github.com/LordMike/MBW.HassMQTT/issues/16
            // The current model cannot distinguish an absent nullable property from an explicitly null one.
            RemoveNullProperties(expected);
        }

        JToken canonicalExpected = Canonicalize(expected);
        JToken canonicalActual = Canonicalize(actual);

        Xunit.Assert.True(
            JToken.DeepEquals(canonicalExpected, canonicalActual),
            $"Discovery JSON changed after a {modelType.Name} round trip for '{caseName}'.{Environment.NewLine}" +
            $"Expected:{Environment.NewLine}{expected}{Environment.NewLine}" +
            $"Actual:{Environment.NewLine}{actual}");
    }

    private static void RemoveNullProperties(JToken token)
    {
        if (token is JObject obj)
        {
            foreach (JProperty property in obj.Properties().ToList())
            {
                if (property.Value.Type == JTokenType.Null)
                    property.Remove();
                else
                    RemoveNullProperties(property.Value);
            }
        }
        else if (token is JArray array)
        {
            foreach (JToken item in array)
                RemoveNullProperties(item);
        }
    }

    private static JToken Canonicalize(JToken token)
    {
        return token switch
        {
            JObject obj => new JObject(
                obj.Properties()
                    .OrderBy(property => property.Name, StringComparer.Ordinal)
                    .Select(property => new JProperty(property.Name, CanonicalizeProperty(property)))),
            JArray array => new JArray(array.Select(Canonicalize)),
            JValue value when value.Type is JTokenType.Integer or JTokenType.Float => CanonicalizeNumber(value),
            _ => token.DeepClone()
        };
    }

    private static JToken CanonicalizeProperty(JProperty property)
    {
        // Device identifiers use Home Assistant's minimal scalar-or-list form on serialization.
        if (property.Name == "identifiers" && property.Value is JArray { Count: 1 } identifiers)
            return Canonicalize(identifiers[0]);

        return Canonicalize(property.Value);
    }

    private static JToken CanonicalizeNumber(JValue value)
    {
        string? text = Convert.ToString(value.Value, CultureInfo.InvariantCulture);
        return decimal.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture, out decimal number)
            ? new JValue(number)
            : value.DeepClone();
    }
}
