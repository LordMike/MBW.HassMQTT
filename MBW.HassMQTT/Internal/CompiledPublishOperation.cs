using System;
using MBW.HassMQTT.Serialization;

namespace MBW.HassMQTT.Internal;

internal sealed class CompiledPublishOperation
{
    private readonly Func<CompiledPublishAttempt> _capture;

    public string Topic { get; }
    public PublishOperationKind Kind { get; }

    private CompiledPublishOperation(
        string topic,
        PublishOperationKind kind,
        Func<CompiledPublishAttempt> capture)
    {
        Topic = topic;
        Kind = kind;
        _capture = capture;
    }

    public static CompiledPublishOperation CreateState(MqttStateValueTopic source, PublishOperationKind kind) =>
        new(source.PublishTopic, kind, () =>
        {
            if (!source.Dirty)
                return null;

            MqttStateValueTopic.Snapshot snapshot = source.Capture();
            return new CompiledPublishAttempt(
                source.PublishTopic,
                MqttValueSerializer.SerializeStandalone(snapshot.Value),
                kind,
                () => source.MarkPublished(snapshot.Revision));
        });

    public static CompiledPublishOperation CreateAttributes(MqttAttributesTopic source) =>
        new(source.PublishTopic, PublishOperationKind.Attributes, () =>
        {
            if (!source.Dirty)
                return null;

            MqttAttributesTopic.Snapshot snapshot = source.Capture();
            return new CompiledPublishAttempt(
                source.PublishTopic,
                MqttValueSerializer.SerializeAttributes(snapshot.Values),
                PublishOperationKind.Attributes,
                () => source.MarkPublished(snapshot.Revision));
        });

    public static CompiledPublishOperation CreateCombined(
        MqttStateValueTopic state,
        MqttAttributesTopic attributes) =>
        new(state.PublishTopic, PublishOperationKind.Value, () =>
        {
            if (!state.Initialized || (!state.Dirty && !attributes.Dirty))
                return null;

            MqttStateValueTopic.Snapshot stateSnapshot = state.Capture();
            if (!stateSnapshot.Initialized)
                return null;
            MqttAttributesTopic.Snapshot attributesSnapshot = attributes.Capture();

            return new CompiledPublishAttempt(
                state.PublishTopic,
                MqttValueSerializer.SerializeCombined(stateSnapshot.Value, attributesSnapshot.Values),
                PublishOperationKind.Value,
                () =>
                {
                    state.MarkPublished(stateSnapshot.Revision);
                    attributes.MarkPublished(attributesSnapshot.Revision);
                });
        });

    public bool TryCapture(out CompiledPublishAttempt attempt)
    {
        attempt = _capture();
        return attempt != null;
    }

    internal sealed class CompiledPublishAttempt
    {
        private readonly Action _acknowledge;

        public string Topic { get; }
        public byte[] Payload { get; }
        public PublishOperationKind Kind { get; }

        public CompiledPublishAttempt(
            string topic,
            byte[] payload,
            PublishOperationKind kind,
            Action acknowledge)
        {
            Topic = topic;
            Payload = payload;
            Kind = kind;
            _acknowledge = acknowledge;
        }

        public void Acknowledge() => _acknowledge();
    }
}
