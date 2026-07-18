using System.Collections.Generic;
using MBW.HassMQTT.Internal;
using Xunit;

namespace MBW.HassMQTT.Tests;

public class CompiledPublishingPlanTests
{
    [Fact]
    public void Many_to_one_attempt_acknowledges_exact_captured_revisions()
    {
        MqttStateValueTopic state = new MqttStateValueTopic("input/state") { Value = "first" };
        MqttAttributesTopic attributes = new MqttAttributesTopic("input/attributes");
        attributes.SetAttribute("quality", "good");
        CompiledPublishOperation operation = CompiledPublishOperation.CreateComposition<string, Dictionary<string, object>, Dictionary<string, object>>(
            "output/combined",
            state,
            attributes,
            (stateValue, attributeValues) => new Dictionary<string, object>
            {
                ["state"] = stateValue,
                ["attributes"] = attributeValues
            },
            PublishOperationKind.Value);

        Assert.True(operation.TryCapture(out CompiledPublishOperation.CompiledPublishAttempt attempt));
        Dictionary<string, object> payload = Assert.IsType<Dictionary<string, object>>(attempt.Payload);
        Assert.Equal("first", payload["state"]);

        state.Value = "second";
        attempt.Acknowledge();

        Assert.True(state.Dirty);
        Assert.False(attributes.Dirty);
        Assert.True(operation.TryCapture(out CompiledPublishOperation.CompiledPublishAttempt next));
        Assert.Equal("second", Assert.IsType<Dictionary<string, object>>(next.Payload)["state"]);
    }

    [Fact]
    public void Failed_attempt_leaves_every_source_dirty_until_acknowledged()
    {
        MqttStateValueTopic first = new MqttStateValueTopic("input/first") { Value = 1 };
        MqttStateValueTopic second = new MqttStateValueTopic("input/second") { Value = 2 };
        CompiledPublishOperation operation = CompiledPublishOperation.CreateComposition<int, int, int>(
            "output/sum",
            first,
            second,
            (firstValue, secondValue) => firstValue + secondValue,
            PublishOperationKind.Value);

        Assert.True(operation.TryCapture(out CompiledPublishOperation.CompiledPublishAttempt attempt));
        Assert.Equal(3, attempt.Payload);
        Assert.True(first.Dirty);
        Assert.True(second.Dirty);

        attempt.Acknowledge();
        Assert.False(first.Dirty);
        Assert.False(second.Dirty);
    }
}
