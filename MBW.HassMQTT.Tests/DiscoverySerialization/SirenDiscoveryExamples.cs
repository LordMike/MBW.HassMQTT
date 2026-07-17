using System;
using MBW.HassMQTT.DiscoveryModels.Models;
using Xunit;

namespace MBW.HassMQTT.Tests.DiscoverySerialization;

public class SirenDiscoveryExamples
{
    public static TheoryData<string, Type, string, bool> HaExamples => new()
    {
        {
            "Configuration",
            typeof(MqttSiren),
            """
            {
              "command_topic": "home/bedroom/siren/set"
            }
            """,
            false
        },
        {
            "Full configuration",
            typeof(MqttSiren),
            """
            {
              "unique_id": "custom_siren",
              "name": "Intrusion siren",
              "state_topic": "home/alarm/siren1",
              "command_topic": "home/alarm/siren1/set",
              "available_tones": [
                "ping",
                "siren"
              ],
              "availability": [
                {
                  "topic": "home/alarm/siren1/available"
                }
              ],
              "payload_on": "ON",
              "payload_off": "OFF",
              "state_on": "ON",
              "state_off": "OFF",
              "optimistic": false,
              "qos": 0,
              "retain": true
            }
            """,
            false
        },
        {
            "On/Off only siren controlling a Tasmota relay",
            typeof(MqttSiren),
            """
            {
              "unique_id": "tasmota_siren",
              "name": "garage",
              "state_topic": "stat/SIREN/RESULT",
              "command_topic": "cmnd/SIREN/POWER",
              "availability_topic": "tele/SIREN/LWT",
              "command_template": "{{ value }}",
              "state_value_template": "{{ value_json.POWER }}",
              "payload_on": "ON",
              "payload_off": "OFF",
              "payload_available": "Online",
              "payload_not_available": "Offline"
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
