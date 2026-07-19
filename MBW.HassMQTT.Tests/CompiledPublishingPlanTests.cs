using System.Text.Json.Nodes;
using MBW.HassMQTT.Internal;
using Xunit;

namespace MBW.HassMQTT.Tests;

public class CompiledPublishingPlanTests
{
    [Fact]
    public void Combined_attempt_acknowledges_exact_captured_revisions()
    {
        MqttStateValueTopic state = new MqttStateValueTopic("output/combined") { Value = "first" };
        MqttAttributesTopic attributes = new MqttAttributesTopic("output/combined");
        attributes.SetAttribute("quality", "good");
        CompiledPublishOperation operation = CompiledPublishOperation.CreateCombined(state, attributes);

        Assert.True(operation.TryCapture(out CompiledPublishOperation.CompiledPublishAttempt attempt));
        JsonObject payload = JsonNode.Parse(attempt.Payload)!.AsObject();
        Assert.Equal("first", payload["state"]!.GetValue<string>());

        state.Value = "second";
        attempt.Acknowledge();

        Assert.True(state.Dirty);
        Assert.False(attributes.Dirty);
        Assert.True(operation.TryCapture(out CompiledPublishOperation.CompiledPublishAttempt next));
        Assert.Equal("second", JsonNode.Parse(next.Payload)!["state"]!.GetValue<string>());
    }

    [Fact]
    public void Failed_attempt_leaves_every_source_dirty_until_acknowledged()
    {
        MqttStateValueTopic state = new MqttStateValueTopic("output/combined") { Value = 1 };
        MqttAttributesTopic attributes = new MqttAttributesTopic("output/combined");
        attributes.SetAttribute("second", 2);
        CompiledPublishOperation operation = CompiledPublishOperation.CreateCombined(state, attributes);

        Assert.True(operation.TryCapture(out CompiledPublishOperation.CompiledPublishAttempt attempt));
        Assert.Equal(1, JsonNode.Parse(attempt.Payload)!["state"]!.GetValue<int>());
        Assert.True(state.Dirty);
        Assert.True(attributes.Dirty);

        attempt.Acknowledge();
        Assert.False(state.Dirty);
        Assert.False(attributes.Dirty);
    }
}
