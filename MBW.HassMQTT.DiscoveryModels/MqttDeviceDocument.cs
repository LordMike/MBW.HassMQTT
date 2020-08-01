using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using MBW.HassMQTT.DiscoveryModels.Helpers;
using Newtonsoft.Json.Linq;

namespace MBW.HassMQTT.DiscoveryModels
{
    public class MqttDeviceDocument
    {
        private readonly JObject _deviceRef;
        private readonly Action _onUpdated;
        private ObservableCollection<ValueTuple<string, string>> _connections;

        internal MqttDeviceDocument(JObject deviceRef, Action onUpdated)
        {
            _deviceRef = deviceRef;
            _onUpdated = onUpdated;
            _connections = new ObservableCollection<ValueTuple<string, string>>();
            _connections.CollectionChanged += ConnectionsOnCollectionChanged;
        }

        private void ConnectionsOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            // Redo list in discovery doc
            _deviceRef["connections"] = JToken.FromObject(_connections.Select(s => new[] { s.Item1, s.Item2 }).ToArray());
            _onUpdated();
        }

        /// <summary>
        /// A list of connections of the device to the outside world as a list of tuples [connection_type, connection_identifier].
        /// For example the MAC address of a network interface: "connections": [["mac", "02:5b:26:a8:dc:12"]].
        /// </summary>
        public ICollection<ValueTuple<string, string>> Connections => _connections;

        /// <summary>
        /// A list of IDs that uniquely identify the device. For example a serial number.
        /// </summary>
        public string[] Identifiers
        {
            get => _deviceRef.GetOrDefault<string[]>("identifiers", null);
            set => _deviceRef.SetIfChanged("identifiers", value, _onUpdated);
        }

        /// <summary>
        /// The manufacturer of the device.
        /// </summary>
        public string Manufacturer
        {
            get => _deviceRef.GetOrDefault<string>("manufacturer", null);
            set => _deviceRef.SetIfChanged("manufacturer", value, _onUpdated);
        }

        /// <summary>
        /// The model of the device.
        /// </summary>
        public string Model
        {
            get => _deviceRef.GetOrDefault<string>("model", null);
            set => _deviceRef.SetIfChanged("model", value, _onUpdated);
        }

        /// <summary>
        /// The name of the device.
        /// </summary>
        public string Name
        {
            get => _deviceRef.GetOrDefault<string>("name", null);
            set => _deviceRef.SetIfChanged("name", value, _onUpdated);
        }

        /// <summary>
        /// The firmware version of the device.
        /// </summary>
        public string SwVersion
        {
            get => _deviceRef.GetOrDefault<string>("sw_version", null);
            set => _deviceRef.SetIfChanged("sw_version", value, _onUpdated);
        }

        /// <summary>
        /// Identifier of a device that routes messages between this device and Home Assistant.
        /// Examples of such devices are hubs, or parent devices of a sub-device.
        ///
        /// This is used to show device topology in Home Assistant.
        /// </summary>
        public string ViaDevice
        {
            get => _deviceRef.GetOrDefault<string>("via_device", null);
            set => _deviceRef.SetIfChanged("via_device", value, _onUpdated);
        }
    }
}