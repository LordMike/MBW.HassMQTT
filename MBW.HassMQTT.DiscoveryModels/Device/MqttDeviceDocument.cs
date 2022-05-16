#nullable enable

using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using FluentValidation;
using JetBrains.Annotations;
using MBW.HassMQTT.DiscoveryModels.Metadata;

namespace MBW.HassMQTT.DiscoveryModels.Device;

public class MqttDeviceDocument : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler PropertyChanged;

    public static MqttDeviceDocumentValidator Validator { get; } = new MqttDeviceDocumentValidator();

    [NotifyPropertyChangedInvocator]
    [UsedImplicitly]
    protected virtual void OnPropertyChanged(string propertyName, object before, object after)
    {
        if (propertyName == nameof(Identifiers))
        {
            if (before is string strBefore && after is string strAfter &&
                string.Equals(strBefore, strAfter, StringComparison.Ordinal))
            {
                return;
            }
        }

        if (propertyName == nameof(Connections))
        {
            if (before is ConnectionInfo valBefore && after is ConnectionInfo valAfter &&
                string.Equals(valBefore.Type, valAfter.Type, StringComparison.Ordinal) &&
                string.Equals(valBefore.Value, valAfter.Value, StringComparison.Ordinal))
            {
                return;
            }
        }

        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    internal MqttDeviceDocument()
    {
        Connections = new ObservableCollection<ConnectionInfo>();
        Identifiers = new ObservableCollection<string>();

        Connections.CollectionChanged += (sender, args) =>
        {
            object oldValue = null;
            switch (args.Action)
            {
                case NotifyCollectionChangedAction.Move:
                case NotifyCollectionChangedAction.Remove:
                case NotifyCollectionChangedAction.Replace:
                    oldValue = args.OldItems[0];
                    break;
            }

            OnPropertyChanged(nameof(Connections), oldValue, args.NewItems[0]);
        };
        Identifiers.CollectionChanged += (sender, args) =>
        {
            object oldValue = null;
            switch (args.Action)
            {
                case NotifyCollectionChangedAction.Move:
                case NotifyCollectionChangedAction.Remove:
                case NotifyCollectionChangedAction.Replace:
                    oldValue = args.OldItems[0];
                    break;
            }

            OnPropertyChanged(nameof(Identifiers), oldValue, args.NewItems[0]);
        };
    }

    /// <summary>
    /// A list of connections of the device to the outside world as a list of tuples [connection_type, connection_identifier].
    /// For example the MAC address of a network interface: "connections": [["mac", "02:5b:26:a8:dc:12"]].
    /// </summary>
    public ObservableCollection<ConnectionInfo> Connections { get; }

    /// <summary>
    /// A list of IDs that uniquely identify the device. For example a serial number.
    /// </summary>
    public ObservableCollection<string> Identifiers { get; }

    /// <summary>
    /// A link to the webpage that can manage the configuration of this device. Can be either an HTTP or HTTPS link.
    /// </summary>
    public string? ConfigurationUrl { get; set; }

    /// <summary>
    /// The manufacturer of the device.
    /// </summary>
    public string? Manufacturer { get; set; }

    /// <summary>
    /// The model of the device.
    /// </summary>
    public string? Model { get; set; }

    /// <summary>
    /// The name of the device.
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Suggest an area if the device isn’t in one yet.
    /// </summary>
    public string? SuggestedArea { get; set; }

    /// <summary>
    /// The firmware version of the device.
    /// </summary>
    public string? SwVersion { get; set; }

    /// <summary>
    /// Identifier of a device that routes messages between this device and Home Assistant.
    /// Examples of such devices are hubs, or parent devices of a sub-device.
    ///
    /// This is used to show device topology in Home Assistant.
    /// </summary>
    public string? ViaDevice { get; set; }

    public class MqttDeviceDocumentValidator : AbstractValidator<MqttDeviceDocument>
    {
        public MqttDeviceDocumentValidator()
        {
            // Either Identifiers or Connections must be non-empty
            RuleFor(s => s.Identifiers).NotEmpty().Unless(s => s.Connections.Any());
            RuleFor(s => s.Connections).NotEmpty().Unless(s => s.Identifiers.Any());

            RuleForEach(s => s.Identifiers).NotNull().WithMessage("{PropertyName} must not contain null values");
            RuleForEach(s => s.Connections).SetValidator(ConnectionInfo.Validator);
        }
    }
}