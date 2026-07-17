using System;
using MBW.HassMQTT.DiscoveryModels.Models;
using Xunit;

namespace MBW.HassMQTT.Tests.DiscoverySerialization;

public class BinarySensorDiscoveryExamples
{
    public static TheoryData<string, Type, string, bool> HaExamples => new()
    {
        {
            "Configuration",
            typeof(MqttBinarySensor),
            """
            {
              "state_topic": "basement/window/contact"
            }
            """,
            false
        },
        {
            "Full configuration with JSON data",
            typeof(MqttBinarySensor),
            """
            {
              "name": "Window Contact Sensor",
              "state_topic": "bedroom/window/contact",
              "payload_on": "ON",
              "availability": [
                {
                  "topic": "bedroom/window/availability",
                  "payload_available": "online",
                  "payload_not_available": "offline"
                }
              ],
              "qos": 0,
              "device_class": "opening",
              "value_template": "{{ value_json.state }}"
            }
            """,
            false
        },
        {
            "Toggle the binary sensor each time a message is received on state_topic",
            typeof(MqttBinarySensor),
            """
            {
              "state_topic": "lab_button/cmnd/POWER",
              "value_template": "{%if is_state(entity_id,\"on\")-%}OFF{%-else-%}ON{%-endif%}"
            }
            """,
            false
        },
        {
            "Get the state of a device with ESPEasy",
            typeof(MqttBinarySensor),
            """
            {
              "name": "Bathroom",
              "state_topic": "home/bathroom/switch/button",
              "payload_on": "1",
              "payload_off": "0"
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
