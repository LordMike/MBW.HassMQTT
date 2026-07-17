using System.Collections.Generic;
using Xunit;

namespace MBW.HassMQTT.Tests;

public class MqttValueRevisionTests
{
    [Fact]
    public void Existing_attribute_is_replaced_and_revision_stays_dirty_until_published()
    {
        MqttAttributesTopic topic = new MqttAttributesTopic("test/attributes");
        topic.SetAttribute("value", 1);
        long firstRevision = topic.Revision;
        topic.MarkPublished(firstRevision);

        topic.SetAttribute("value", 2);

        Dictionary<string, object> snapshot = Assert.IsType<Dictionary<string, object>>(topic.GetSerializedValue());
        Assert.Equal(2, snapshot["value"]);
        Assert.True(topic.Dirty);

        topic.MarkPublished(topic.Revision);
        Assert.False(topic.Dirty);
    }

    [Fact]
    public void Publishing_an_old_revision_does_not_clear_a_new_value()
    {
        MqttStateValueTopic topic = new MqttStateValueTopic("test/state") { Value = "first" };
        long firstRevision = topic.Revision;

        topic.Value = "second";
        topic.MarkPublished(firstRevision);

        Assert.True(topic.Dirty);
        Assert.Equal("second", topic.GetSerializedValue());
    }
}
