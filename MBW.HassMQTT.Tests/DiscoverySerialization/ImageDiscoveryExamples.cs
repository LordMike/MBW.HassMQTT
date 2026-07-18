using System;
using MBW.HassMQTT.DiscoveryModels.Models;
using Xunit;

namespace MBW.HassMQTT.Tests.DiscoverySerialization;

public class ImageDiscoveryExamples
{
    public static TheoryData<string, Type, string, bool> HaExamples => new()
    {
        {
            "Configuration",
            typeof(MqttImage),
            """
            {
              "url_topic": "mynas/status/url"
            }
            """,
            false
        },
        {
            "Example receiving images from a URL",
            typeof(MqttImage),
            """
            {
              "url_topic": "mynas/status/url"
            }
            """,
            false
        },
        {
            "Example receiving images from a file",
            typeof(MqttImage),
            """
            {
              "image_topic": "mynas/status/file",
              "content_type": "image/png"
            }
            """,
            false
        },
    };

    [Theory]
    [MemberData(nameof(HaExamples))]
    public void DocumentationExamplesRoundTrip(string name, Type modelType, string json, bool normalizeExplicitNulls)
    {
        DiscoveryJsonRoundTrip.Assert(name, modelType, json, normalizeExplicitNulls);
    }
}
