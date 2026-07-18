using System;
using MBW.HassMQTT.DiscoveryModels.Models;
using Xunit;

namespace MBW.HassMQTT.Tests.DiscoverySerialization;

public class UpdateDiscoveryExamples
{
    public static TheoryData<string, Type, string, bool> HaExamples => new()
    {
        {
            "Configuration",
            typeof(MqttUpdate),
            """
            {
              "state_topic": "topic-installed",
              "latest_version_topic": "topic-latest"
            }
            """,
            false
        },
        {
            "Shelly firmware update",
            typeof(MqttUpdate),
            """
            {
              "name": "Shelly Plug S Firmware Update",
              "title": "Shelly Plug S Firmware",
              "release_url": "https://shelly-api-docs.shelly.cloud/gen1/#changelog",
              "entity_picture": "https://brands.home-assistant.io/_/shelly/icon.png",
              "state_topic": "shellies/shellyplug-s-112233/info",
              "value_template": "{{ value_json['update'].old_version }}",
              "latest_version_topic": "shellies/shellyplug-s-112233/info",
              "latest_version_template": "{% if value_json['update'].new_version %}{{ value_json['update'].new_version }}{% else %}{{ value_json['update'].old_version }}{% endif %}",
              "device_class": "firmware",
              "command_topic": "shellies/shellyplug-s-112233/command",
              "payload_install": "update_fw"
            }
            """,
            false
        },
        {
            "Firmware update with separate versions",
            typeof(MqttUpdate),
            """
            {
              "name": "Amazing Device Update",
              "title": "Device Firmware",
              "state_topic": "amazing-device/state-topic",
              "device_class": "firmware",
              "command_topic": "amazing-device/command",
              "payload_install": "install"
            }
            """,
            false
        },
        {
            "Firmware update with combined version template",
            typeof(MqttUpdate),
            """
            {
              "name": "Amazing Device Update",
              "title": "Device Firmware",
              "state_topic": "amazing-device/state-topic",
              "value_template": "{{ {'installed_version': value_json.installed_ver, 'latest_version': value_json.new_ver } | to_json }}",
              "device_class": "firmware",
              "command_topic": "amazing-device/command",
              "payload_install": "install"
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
