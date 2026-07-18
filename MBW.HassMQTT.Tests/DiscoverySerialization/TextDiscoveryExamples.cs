using System;
using MBW.HassMQTT.DiscoveryModels.Models;
using Xunit;

namespace MBW.HassMQTT.Tests.DiscoverySerialization;

public class TextDiscoveryExamples
{
    public static TheoryData<string, Type, string, bool> HaExamples => new()
    {
        {
            "Configuration",
            typeof(MqttText),
            """
            {
              "command_topic": "command-topic"
            }
            """,
            false
        },
        {
            "Examples",
            typeof(MqttText),
            """
            {
              "name": "Remote LCD screen",
              "icon": "mdi:ab-testing",
              "mode": "text",
              "command_topic": "txt/cmd",
              "state_topic": "txt/state",
              "min": 2,
              "max": 20
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
