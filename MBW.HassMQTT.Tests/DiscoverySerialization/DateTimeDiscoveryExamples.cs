using System;
using MBW.HassMQTT.DiscoveryModels.Models;
using Xunit;

namespace MBW.HassMQTT.Tests.DiscoverySerialization;

public class DateTimeDiscoveryExamples
{
    public static TheoryData<string, Type, string, bool> HaExamples => new()
    {
        {
            "Configuration",
            typeof(MqttDateTime),
            """
            {
              "command_topic": "command-topic"
            }
            """,
            false
        },
        {
            "Examples",
            typeof(MqttDateTime),
            """
            {
              "name": "Scheduled task",
              "icon": "mdi:ab-testing",
              "command_topic": "date_selector/set",
              "state_topic": "date_selector/state"
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
