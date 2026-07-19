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

        MqttAttributesTopic.Snapshot snapshot = topic.Capture();
        Assert.Equal(2, Assert.IsType<int>(snapshot.Values["value"].GetValue()));
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
        Assert.Equal("second", topic.Capture().Value.GetValue());
    }

    [Fact]
    public void Null_attribute_is_stored_until_explicitly_removed()
    {
        MqttAttributesTopic topic = new MqttAttributesTopic("test/attributes");

        topic.SetAttribute("value", null);

        Assert.True(topic.Capture().Values.ContainsKey("value"));
        topic.RemoveAttribute("value");
        Assert.False(topic.Capture().Values.ContainsKey("value"));
    }
}
