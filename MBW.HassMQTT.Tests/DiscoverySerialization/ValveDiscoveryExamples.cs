using System;
using MBW.HassMQTT.DiscoveryModels.Models;
using Xunit;

namespace MBW.HassMQTT.Tests.DiscoverySerialization;

public class ValveDiscoveryExamples
{
    public static TheoryData<string, Type, string, bool> HaExamples => new()
    {
        {
            "Valve controlled by states",
            typeof(MqttValve),
            """
            {
              "command_topic": "heater/valve/set",
              "state_topic": "heater/valve/state"
            }
            """,
            false
        },
        {
            "Valve controlled by position",
            typeof(MqttValve),
            """
            {
              "command_topic": "heater/valve/set",
              "state_topic": "heater/valve/state",
              "reports_position": true
            }
            """,
            false
        },
        {
            "Full configuration for a valve that does not report position",
            typeof(MqttValve),
            """
            {
              "name": "MQTT valve",
              "command_template": "{\"x\": {{ value }} }",
              "command_topic": "heater/valve/set",
              "state_topic": "heater/valve/state",
              "availability": [
                {
                  "topic": "heater/valve/availability"
                }
              ],
              "qos": 0,
              "reports_position": false,
              "retain": true,
              "payload_open": "OPEN",
              "payload_close": "CLOSE",
              "payload_stop": "STOP",
              "state_open": "open",
              "state_opening": "opening",
              "state_closed": "closed",
              "state_closing": "closing",
              "payload_available": "online",
              "payload_not_available": "offline",
              "optimistic": false,
              "value_template": "{{ value_json.x }}"
            }
            """,
            false
        },
        {
            "Sample configuration of a valve that reports the position",
            typeof(MqttValve),
            """
            {
              "name": "MQTT valve",
              "command_template": "{\"x\": {{ value }} }",
              "command_topic": "heater/valve/set",
              "state_topic": "heater/valve/state",
              "availability": [
                {
                  "topic": "heater/valve/availability"
                }
              ],
              "reports_position": true,
              "value_template": "{{ value_json.x }}"
            }
            """,
            false
        },
        {
            "Configuration for disabling valve commands",
            typeof(MqttValve),
            """
            {
              "payload_open": "on",
              "payload_close": null,
              "payload_stop": "on"
            }
            """,
            true
        },
    };

    [Theory]
    [MemberData(nameof(HaExamples))]
    public void DocumentationExamplesRoundTrip(string name, Type modelType, string json, bool normalizeExplicitNulls)
    {
        DiscoveryJsonRoundTrip.Assert(name, modelType, json, normalizeExplicitNulls);
    }
}
