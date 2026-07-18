using System;
using MBW.HassMQTT.DiscoveryModels.Models;
using Xunit;

namespace MBW.HassMQTT.Tests.DiscoverySerialization;

public class EventDiscoveryExamples
{
    public static TheoryData<string, Type, string, bool> HaExamples => new()
    {
        {
            "Configuration",
            typeof(MqttEvent),
            """
            {
              "state_topic": "home/doorbell/state",
              "event_types": [
                "press"
              ]
            }
            """,
            false
        },
        {
            "Full configuration with JSON data",
            typeof(MqttEvent),
            """
            {
              "state_topic": "home/doorbell/state",
              "event_types": [
                "press",
                "hold"
              ],
              "availability": [
                {
                  "topic": "home/doorbell/available"
                }
              ],
              "qos": 0,
              "device_class": "doorbell"
            }
            """,
            false
        },
        {
            "Example: processing event data using a value template",
            typeof(MqttEvent),
            """
            {
              "name": "Desk button",
              "state_topic": "desk/button/state",
              "event_types": [
                "single",
                "double"
              ],
              "device_class": "button",
              "value_template": "{ {% for key in value_json %}\n{% if value_json[key].get(\"Action\") %}\n\"button\": \"{{ key }}\",\n\"event_type\": \"{{ value_json[key].Action | lower }}\"\n{% endif %}\n{% endfor %} }"
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
