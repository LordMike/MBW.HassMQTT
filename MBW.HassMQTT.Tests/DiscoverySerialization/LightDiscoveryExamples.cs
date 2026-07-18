using System;
using MBW.HassMQTT.DiscoveryModels.Models;
using Xunit;

namespace MBW.HassMQTT.Tests.DiscoverySerialization;

public class LightDiscoveryExamples
{
    public static TheoryData<string, Type, string, bool> HaExamples => new()
    {
        {
            "Default schema - Configuration",
            typeof(MqttLightDefault),
            """
            {
              "command_topic": "office/rgb1/light/switch"
            }
            """,
            false
        },
        {
            "Default schema - Brightness and RGB support",
            typeof(MqttLightDefault),
            """
            {
              "name": "Office Light RGB",
              "state_topic": "office/rgb1/light/status",
              "command_topic": "office/rgb1/light/switch",
              "brightness_state_topic": "office/rgb1/brightness/status",
              "brightness_command_topic": "office/rgb1/brightness/set",
              "rgb_state_topic": "office/rgb1/rgb/status",
              "rgb_command_topic": "office/rgb1/rgb/set",
              "state_value_template": "{{ value_json.state }}",
              "brightness_value_template": "{{ value_json.brightness }}",
              "rgb_value_template": "{{ value_json.rgb | join(',') }}",
              "qos": 0,
              "payload_on": "ON",
              "payload_off": "OFF",
              "optimistic": false
            }
            """,
            false
        },
        {
            "Default schema - Brightness and no RGB support",
            typeof(MqttLightDefault),
            """
            {
              "name": "Office light",
              "state_topic": "office/light/status",
              "command_topic": "office/light/switch",
              "brightness_state_topic": "office/light/brightness",
              "brightness_command_topic": "office/light/brightness/set",
              "qos": 0,
              "payload_on": "ON",
              "payload_off": "OFF",
              "optimistic": false
            }
            """,
            false
        },
        {
            "Brightness without on commands",
            typeof(MqttLightDefault),
            """
            {
              "name": "Brightness light",
              "state_topic": "office/light/status",
              "command_topic": "office/light/switch",
              "payload_off": "OFF",
              "brightness_state_topic": "office/light/brightness",
              "brightness_command_topic": "office/light/brightness/set",
              "on_command_type": "brightness"
            }
            """,
            false
        },
        {
            "JSON schema - Configuration",
            typeof(MqttLightJson),
            """
            {
              "schema": "json",
              "command_topic": "home/rgb1/set"
            }
            """,
            false
        },
        {
            "JSON schema - Brightness and RGB support",
            typeof(MqttLightJson),
            """
            {
              "schema": "json",
              "name": "mqtt_json_light_1",
              "state_topic": "home/rgb1",
              "command_topic": "home/rgb1/set",
              "brightness": true,
              "supported_color_modes": [
                "rgb"
              ]
            }
            """,
            false
        },
        {
            "JSON schema - Brightness and no RGB support",
            typeof(MqttLightJson),
            """
            {
              "schema": "json",
              "name": "mqtt_json_light_1",
              "state_topic": "home/rgb1",
              "command_topic": "home/rgb1/set",
              "brightness": true,
              "supported_color_modes": [
                "brightness"
              ]
            }
            """,
            false
        },
        {
            "Brightness scaled",
            typeof(MqttLightJson),
            """
            {
              "schema": "json",
              "name": "mqtt_json_light_1",
              "state_topic": "home/light",
              "command_topic": "home/light/set",
              "brightness": true,
              "brightness_scale": 4095,
              "supported_color_modes": [
                "brightness"
              ]
            }
            """,
            false
        },
        {
            "HS color",
            typeof(MqttLightJson),
            """
            {
              "schema": "json",
              "name": "mqtt_json_hs_light",
              "state_topic": "home/light",
              "command_topic": "home/light/set",
              "supported_color_modes": [
                "hs"
              ]
            }
            """,
            false
        },
        {
            "Brightness and RGBW support",
            typeof(MqttLightJson),
            """
            {
              "schema": "json",
              "name": "mqtt_json_light_1",
              "state_topic": "home/rgbw1",
              "command_topic": "home/rgbw1/set",
              "brightness": true,
              "supported_color_modes": [
                "rgbw"
              ]
            }
            """,
            false
        },
        {
            "Template schema - Configuration",
            typeof(MqttLightTemplate),
            """
            {
              "schema": "template",
              "command_topic": "home/rgb1/set",
              "command_on_template": "on",
              "command_off_template": "off"
            }
            """,
            false
        },
        {
            "Simple string payload",
            typeof(MqttLightTemplate),
            """
            {
              "schema": "template",
              "command_topic": "home/rgb1/set",
              "state_topic": "home/rgb1/status",
              "command_on_template": "on,{{ brightness|d }},{{ red|d }}-{{ green|d }}-{{ blue|d }},{{ hue|d }}-{{ sat|d }}",
              "command_off_template": "off",
              "state_template": "{{ value.split(',')[0] }}",
              "brightness_template": "{{ value.split(',')[1] }}",
              "red_template": "{{ value.split(',')[2].split('-')[0] }}",
              "green_template": "{{ value.split(',')[2].split('-')[1] }}",
              "blue_template": "{{ value.split(',')[2].split('-')[2] }}"
            }
            """,
            false
        },
        {
            "JSON payload",
            typeof(MqttLightTemplate),
            """
            {
              "schema": "template",
              "effect_list": [
                "rainbow",
                "colorloop"
              ],
              "command_topic": "home/rgb1/set",
              "state_topic": "home/rgb1/status",
              "command_on_template": "{\"state\": \"on\" {%- if brightness is defined -%} , \"brightness\": {{ brightness }} {%- endif -%} {%- if red is defined and green is defined and blue is defined -%} , \"color\": [{{ red }}, {{ green }}, {{ blue }}] {%- endif -%} {%- if hue is defined and sat is defined -%} , \"huesat\": [{{ hue }}, {{ sat }}] {%- endif -%} {%- if effect is defined -%} , \"effect\": \"{{ effect }}\" {%- endif -%} }\n",
              "command_off_template": "{\"state\": \"off\"}",
              "state_template": "{{ value_json.state }}",
              "brightness_template": "{{ value_json.brightness }}",
              "red_template": "{{ value_json.color[0] }}",
              "green_template": "{{ value_json.color[1] }}",
              "blue_template": "{{ value_json.color[2] }}",
              "effect_template": "{{ value_json.effect }}"
            }
            """,
            false
        },
        {
            "CCT light (brightness and temperature)",
            typeof(MqttLightTemplate),
            """
            {
              "schema": "template",
              "name": "Bulb-white",
              "command_topic": "shellies/bulb/color/0/set",
              "state_topic": "shellies/bulb/color/0/status",
              "availability_topic": "shellies/bulb/online",
              "command_on_template": "{\"turn\": \"on\", \"mode\": \"white\" {%- if brightness is defined -%} , \"brightness\": {{brightness | float | multiply(0.39215686) | round(0)}} {%- endif -%} {%- if color_temp is defined -%} , \"temp\": {{ [[(1000000 / color_temp | float) | round(0), 3000] | max, 6500] | min }} {%- endif -%} }\n",
              "command_off_template": "{\"turn\":\"off\", \"mode\": \"white\"}",
              "state_template": "{% if value_json.ison and value_json.mode == 'white' %}on{% else %}off{% endif %}",
              "brightness_template": "{{ value_json.brightness | float | multiply(2.55) | round(0) }}",
              "color_temp_template": "{{ (1000000 / value_json.temp | float) | round(0) }}",
              "payload_available": "true",
              "payload_not_available": "false",
              "max_mireds": 334,
              "min_mireds": 153,
              "qos": 1,
              "retain": false,
              "optimistic": false
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
