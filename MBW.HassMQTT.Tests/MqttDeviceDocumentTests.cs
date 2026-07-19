using System.Collections.Generic;
using MBW.HassMQTT.DiscoveryModels.Device;
using MBW.HassMQTT.DiscoveryModels.Metadata;
using MBW.HassMQTT.DiscoveryModels.Models;
using Xunit;

namespace MBW.HassMQTT.Tests;

public class MqttDeviceDocumentTests
{
    [Fact]
    public void Collection_removal_and_reset_raise_safe_property_notifications()
    {
        MqttDeviceDocument document = new MqttSensor(string.Empty, string.Empty).Device;
        List<string> changedProperties = new();
        document.PropertyChanged += (_, args) => changedProperties.Add(args.PropertyName!);

        document.Identifiers.Add("one");
        document.Identifiers.Remove("one");
        document.Identifiers.Add("two");
        document.Identifiers.Clear();

        ConnectionInfo connection = new() { Type = "mac", Value = "00:11:22:33:44:55" };
        document.Connections.Add(connection);
        document.Connections.Remove(connection);
        document.Connections.Add(connection);
        document.Connections.Clear();

        Assert.Equal(4, changedProperties.FindAll(name => name == nameof(document.Identifiers)).Count);
        Assert.Equal(4, changedProperties.FindAll(name => name == nameof(document.Connections)).Count);
    }

    [Fact]
    public void Collection_moves_raise_property_notifications()
    {
        MqttDeviceDocument document = new MqttSensor(string.Empty, string.Empty).Device;
        List<string> changedProperties = new();
        document.PropertyChanged += (_, args) => changedProperties.Add(args.PropertyName!);

        document.Identifiers.Add("one");
        document.Identifiers.Add("two");
        changedProperties.Clear();
        document.Identifiers.Move(0, 1);

        Assert.Equal(new[] { nameof(document.Identifiers) }, changedProperties);

        document.Connections.Add(new ConnectionInfo { Type = "mac", Value = "one" });
        document.Connections.Add(new ConnectionInfo { Type = "mac", Value = "two" });
        changedProperties.Clear();
        document.Connections.Move(0, 1);

        Assert.Equal(new[] { nameof(document.Connections) }, changedProperties);
    }
}
