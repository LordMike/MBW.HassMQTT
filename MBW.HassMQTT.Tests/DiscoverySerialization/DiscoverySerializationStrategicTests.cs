using System;
using MBW.HassMQTT.DiscoveryModels.Models;
using MBW.HassMQTT.Serialization;
using Xunit;

namespace MBW.HassMQTT.Tests.DiscoverySerialization;

public class DiscoverySerializationStrategicTests
{
    public static TheoryData<string> QosExamples => new()
    {
        """{"state_topic":"example/state","qos":0}""",
        """{"state_topic":"example/state","qos":1}""",
        """{"state_topic":"example/state","qos":2}""",
    };

    [Theory]
    [MemberData(nameof(QosExamples))]
    public void QosLevelsRoundTrip(string json)
    {
        DiscoveryJsonRoundTrip.Assert(typeof(MqttSensor), json);
    }

    [Fact]
    public void UnsupportedQosLevelIsRejected()
    {
        byte[] json = System.Text.Encoding.UTF8.GetBytes("""{"state_topic":"example/state","qos":3}""");
        Assert.ThrowsAny<Exception>(() => DiscoveryJsonSerializer.Deserialize(json, typeof(MqttSensor)));
    }

    [Fact]
    public void SharedNestedStructuresAndExplicitDefaultsRoundTrip()
    {
        DiscoveryJsonRoundTrip.Assert(
            typeof(MqttSensor),
            """
            {
              "state_topic": "example/state",
              "name": "",
              "expire_after": 0,
              "enabled_by_default": false,
              "visible_by_default": false,
              "availability": [
                {
                  "topic": "example/availability",
                  "payload_available": "ready",
                  "payload_not_available": "gone"
                }
              ],
              "device": {
                "identifiers": ["serial-1", "serial-2"],
                "connections": [["mac", "02:5b:26:a8:dc:12"]],
                "manufacturer": "Example",
                "model": "Synthetic"
              }
            }
            """);
    }

    [Fact]
    public void MessageExpiryIntervalRoundTrips()
    {
        DiscoveryJsonRoundTrip.Assert(
            typeof(MqttAlarmControlPanel),
            """
            {
              "state_topic": "alarm/state",
              "command_topic": "alarm/set",
              "message_expiry_interval": {
                "days": 1,
                "hours": 2,
                "minutes": 3,
                "seconds": 4
              }
            }
            """);
    }

    [Fact]
    public void BasicLightAdvancedShapeRoundTrips()
    {
        DiscoveryJsonRoundTrip.Assert(
            typeof(MqttLightDefault),
            """
            {
              "schema": "basic",
              "command_topic": "light/set",
              "state_topic": "light/state",
              "effect_command_topic": "light/effect/set",
              "effect_state_topic": "light/effect/state",
              "effect_list": ["rainbow", "pulse"],
              "hs_command_topic": "light/hs/set",
              "hs_state_topic": "light/hs/state",
              "rgbw_command_topic": "light/rgbw/set",
              "rgbww_command_topic": "light/rgbww/set",
              "xy_command_topic": "light/xy/set",
              "white_command_topic": "light/white/set",
              "white_scale": 100,
              "color_temp_command_topic": "light/temperature/set",
              "color_temp_state_topic": "light/temperature/state",
              "color_temp_kelvin": true,
              "min_kelvin": 2000,
              "max_kelvin": 6535,
              "optimistic": false,
              "retain": false
            }
            """);
    }

    [Fact]
    public void JsonLightAdvancedShapeRoundTrips()
    {
        DiscoveryJsonRoundTrip.Assert(
            typeof(MqttLightJson),
            """
            {
              "schema": "json",
              "command_topic": "light/set",
              "state_topic": "light/state",
              "brightness": true,
              "brightness_scale": 4095,
              "effect": true,
              "effect_list": ["rainbow", "pulse"],
              "flash": true,
              "flash_time_short": 1,
              "flash_time_long": 10,
              "transition": true,
              "supported_color_modes": ["color_temp", "hs", "rgb", "rgbw", "rgbww", "white", "xy"],
              "color_temp_kelvin": true,
              "min_kelvin": 2000,
              "max_kelvin": 6535,
              "white_scale": 100
            }
            """);
    }

    [Fact]
    public void TemplateLightAdvancedShapeRoundTrips()
    {
        DiscoveryJsonRoundTrip.Assert(
            typeof(MqttLightTemplate),
            """
            {
              "schema": "template",
              "command_topic": "light/set",
              "state_topic": "light/state",
              "command_on_template": "{\"state\":\"on\",\"brightness\":{{ brightness | default(0) }}}",
              "command_off_template": "{\"state\":\"off\"}",
              "state_template": "{{ value_json.state }}",
              "brightness_template": "{{ value_json.brightness }}",
              "red_template": "{{ value_json.color[0] }}",
              "green_template": "{{ value_json.color[1] }}",
              "blue_template": "{{ value_json.color[2] }}",
              "effect_template": "{{ value_json.effect }}",
              "effect_list": ["rainbow", "pulse"],
              "color_temp_template": "{{ value_json.color_temp }}",
              "color_temp_kelvin": true,
              "min_kelvin": 2000,
              "max_kelvin": 6535
            }
            """);
    }
}
