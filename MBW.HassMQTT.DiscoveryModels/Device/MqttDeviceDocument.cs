﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;

namespace MBW.HassMQTT.DiscoveryModels.Device
{
    public class MqttDeviceDocument : INotifyPropertyChanged
    {
        private readonly MqttSensorDiscoveryBase _discoveryDoc;
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged(string propertyName, object before, object after)
        {
            if (propertyName == nameof(Identifiers))
            {
                // Extra check as arrays are special
                if (before is string[] arrBefore && after is string[] arrAfter &&
                    arrBefore.Length == arrAfter.Length &&
                    arrBefore.SequenceEqual(arrAfter, StringComparer.Ordinal))
                {
                    // These arrays are identical
                    return;
                }
            }

            if (propertyName == nameof(Connections))
            {
                // Extra check as arrays are special
                if (before is IList<ValueTuple<string, string>> arrBefore && after is IList<ValueTuple<string, string>> arrAfter &&
                    arrBefore.Count == arrAfter.Count &&
                    arrBefore.SequenceEqual(arrAfter, StringtupleComparer.Instance))
                {
                    // These arrays are identical
                    return;
                }
            }

            _discoveryDoc.SetDirty();
        }

        internal MqttDeviceDocument(MqttSensorDiscoveryBase discoveryDoc)
        {
            _discoveryDoc = discoveryDoc;
            Connections = new List<(string, string)>();
        }

        /// <summary>
        /// A list of connections of the device to the outside world as a list of tuples [connection_type, connection_identifier].
        /// For example the MAC address of a network interface: "connections": [["mac", "02:5b:26:a8:dc:12"]].
        /// </summary>
        public IList<ValueTuple<string, string>> Connections { get; set; }

        /// <summary>
        /// A list of IDs that uniquely identify the device. For example a serial number.
        /// </summary>
        public string[] Identifiers { get; set; }

        /// <summary>
        /// The manufacturer of the device.
        /// </summary>
        public string Manufacturer { get; set; }

        /// <summary>
        /// The model of the device.
        /// </summary>
        public string Model { get; set; }

        /// <summary>
        /// The name of the device.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The firmware version of the device.
        /// </summary>
        public string SwVersion { get; set; }

        /// <summary>
        /// Identifier of a device that routes messages between this device and Home Assistant.
        /// Examples of such devices are hubs, or parent devices of a sub-device.
        ///
        /// This is used to show device topology in Home Assistant.
        /// </summary>
        public string ViaDevice { get; set; }
    }
}