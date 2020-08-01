using MBW.HassMQTT.DiscoveryModels.Enum;

namespace MBW.HassMQTT.DiscoveryModels.Models
{
    /// <summary>
    /// https://www.home-assistant.io/integrations/cover.mqtt/
    /// </summary>
    [DeviceType(HassDeviceType.Cover)]
    public class MqttCover : MqttEntitySensorDiscoveryBase
    {
        public MqttCover(string topic, string uniqueId) : base(topic, uniqueId)
        {
        }

        /// <summary>
        /// The MQTT topic to publish commands to control the cover.
        /// </summary>
        public string CommandTopic
        {
            get => GetValue<string>("command_topic", default);
            set => SetValue("command_topic", value);
        }

        /// <summary>
        /// Sets the [class of the device](/integrations/cover/), changing the device state and icon that is displayed on the frontend.
        /// </summary>
        public string DeviceClass
        {
            get => GetValue<string>("device_class", default);
            set => SetValue("device_class", value);
        }

        /// <summary>
        /// Defines a [template](/docs/configuration/templating/#processing-incoming-data) to extract the JSON dictionary from messages received on the `json_attributes_topic`. Usage example can be found in [MQTT sensor](/integrations/sensor.mqtt/#json-attributes-template-configuration) documentation.
        /// </summary>
        public string JsonAttributesTemplate
        {
            get => GetValue<string>("json_attributes_template", default);
            set => SetValue("json_attributes_template", value);
        }

        /// <summary>
        /// The MQTT topic subscribed to receive a JSON dictionary payload and then set as sensor attributes. Usage example can be found in [MQTT sensor](/integrations/sensor.mqtt/#json-attributes-topic-configuration) documentation.
        /// </summary>
        public string JsonAttributesTopic
        {
            get => GetValue<string>("json_attributes_topic", default);
            set => SetValue("json_attributes_topic", value);
        }

        /// <summary>
        /// The name of the cover.
        /// </summary>
        public string Name
        {
            get => GetValue<string>("name", default);
            set => SetValue("name", value);
        }

        /// <summary>
        /// Flag that defines if switch works in optimistic mode.
        /// </summary>
        public string Optimistic
        {
            get => GetValue<string>("optimistic", default);
            set => SetValue("optimistic", value);
        }

        /// <summary>
        /// The command payload that closes the cover.
        /// </summary>
        public string PayloadClose
        {
            get => GetValue<string>("payload_close", default);
            set => SetValue("payload_close", value);
        }

        /// <summary>
        /// The command payload that opens the cover.
        /// </summary>
        public string PayloadOpen
        {
            get => GetValue<string>("payload_open", default);
            set => SetValue("payload_open", value);
        }

        /// <summary>
        /// The command payload that stops the cover.
        /// </summary>
        public string PayloadStop
        {
            get => GetValue<string>("payload_stop", default);
            set => SetValue("payload_stop", value);
        }

        /// <summary>
        /// Number which represents closed position.
        /// </summary>
        public int PositionClosed
        {
            get => GetValue<int>("position_closed", default);
            set => SetValue("position_closed", value);
        }

        /// <summary>
        /// Number which represents open position.
        /// </summary>
        public int PositionOpen
        {
            get => GetValue<int>("position_open", default);
            set => SetValue("position_open", value);
        }

        /// <summary>
        /// The MQTT topic subscribed to receive cover position messages. If `position_topic` is set `state_topic` is ignored.
        /// </summary>
        public string PositionTopic
        {
            get => GetValue<string>("position_topic", default);
            set => SetValue("position_topic", value);
        }

        /// <summary>
        /// The maximum QoS level to be used when receiving and publishing messages.
        /// </summary>
        public int Qos
        {
            get => GetValue<int>("qos", default);
            set => SetValue("qos", value);
        }

        /// <summary>
        /// Defines if published messages should have the retain flag set.
        /// </summary>
        public bool Retain
        {
            get => GetValue<bool>("retain", default);
            set => SetValue("retain", value);
        }

        /// <summary>
        /// Defines a [template](/topics/templating/) to define the position to be sent to the `set_position_topic` topic. Incoming position value is available for use in the template `{{position}}`. If no template is defined, the position (0-100) will be calculated according to `position_open` and `position_closed` values.
        /// </summary>
        public string SetPositionTemplate
        {
            get => GetValue<string>("set_position_template", default);
            set => SetValue("set_position_template", value);
        }

        /// <summary>
        /// The MQTT topic to publish position commands to. You need to set position_topic as well if you want to use position topic. Use template if position topic wants different values than within range `position_closed` - `position_open`. If template is not defined and `position_closed != 100` and `position_open != 0` then proper position value is calculated from percentage position.
        /// </summary>
        public string SetpositionTopic
        {
            get => GetValue<string>("set_position_topic", default);
            set => SetValue("set_position_topic", value);
        }

        /// <summary>
        /// The payload that represents the closed state.
        /// </summary>
        public string StateClosed
        {
            get => GetValue<string>("state_closed", default);
            set => SetValue("state_closed", value);
        }

        /// <summary>
        /// The payload that represents the closing state.
        /// </summary>
        public string StateClosing
        {
            get => GetValue<string>("state_closing", default);
            set => SetValue("state_closing", value);
        }

        /// <summary>
        /// The payload that represents the open state.
        /// </summary>
        public string StateOpen
        {
            get => GetValue<string>("state_open", default);
            set => SetValue("state_open", value);
        }

        /// <summary>
        /// The payload that represents the opening state.
        /// </summary>
        public string StateOpening
        {
            get => GetValue<string>("state_opening", default);
            set => SetValue("state_opening", value);
        }

        /// <summary>
        /// The MQTT topic subscribed to receive cover state messages. Use only if not using `position_topic`. State topic can only read open/close state. Cannot read position state. If `position_topic` is set `state_topic` is ignored.
        /// </summary>
        public string StateTopic
        {
            get => GetValue<string>("state_topic", default);
            set => SetValue("state_topic", value);
        }

        /// <summary>
        /// The value that will be sent on a `close_cover_tilt` command.
        /// </summary>
        public int TiltClosedValue
        {
            get => GetValue<int>("tilt_closed_value", default);
            set => SetValue("tilt_closed_value", value);
        }

        /// <summary>
        /// The MQTT topic to publish commands to control the cover tilt.
        /// </summary>
        public string TiltCommandTopic
        {
            get => GetValue<string>("tilt_command_topic", default);
            set => SetValue("tilt_command_topic", value);
        }

        /// <summary>
        /// Flag that determines if open/close are flipped; higher values toward closed and lower values toward open.
        /// </summary>
        public bool TiltInvertState
        {
            get => GetValue<bool>("tilt_invert_state", default);
            set => SetValue("tilt_invert_state", value);
        }

        /// <summary>
        /// The maximum tilt value
        /// </summary>
        public int TiltMax
        {
            get => GetValue<int>("tilt_max", default);
            set => SetValue("tilt_max", value);
        }

        /// <summary>
        /// The minimum tilt value.
        /// </summary>
        public int TiltMin
        {
            get => GetValue<int>("tilt_min", default);
            set => SetValue("tilt_min", value);
        }

        /// <summary>
        /// The value that will be sent on an `open_cover_tilt` command.
        /// </summary>
        public int TiltOpenedValue
        {
            get => GetValue<int>("tilt_opened_value", default);
            set => SetValue("tilt_opened_value", value);
        }

        /// <summary>
        /// Flag that determines if tilt works in optimistic mode.
        /// </summary>
        public bool TiltOptimistic
        {
            get => GetValue<bool>("tilt_optimistic", default);
            set => SetValue("tilt_optimistic", value);
        }

        /// <summary>
        /// Defines a [template](/topics/templating/) that can be used to extract the payload for the `tilt_status_topic` topic. 
        /// </summary>
        public string TiltStatusTemplate
        {
            get => GetValue<string>("tilt_status_template", default);
            set => SetValue("tilt_status_template", value);
        }

        /// <summary>
        /// The MQTT topic subscribed to receive tilt status update values.
        /// </summary>
        public string TiltStatusTopic
        {
            get => GetValue<string>("tilt_status_topic", default);
            set => SetValue("tilt_status_topic", value);
        }

        /// <summary>
        /// Defines a [template](/docs/configuration/templating/#processing-incoming-data) to extract a value from the payload.
        /// </summary>
        public string ValueTemplate
        {
            get => GetValue<string>("value_template", default);
            set => SetValue("value_template", value);
        }
    }
}