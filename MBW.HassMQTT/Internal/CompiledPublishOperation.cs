using System;
using System.Collections.Generic;
using System.Linq;
using MBW.HassMQTT.Abstracts.Interfaces;

namespace MBW.HassMQTT.Internal;

internal enum PublishOperationKind
{
    Value,
    Attributes
}

internal sealed class CompiledPublishOperation
{
    private readonly IReadOnlyList<IMqttValueContainer> _sources;
    private readonly Func<IReadOnlyList<object>, object> _transform;

    public string Topic { get; }
    public PublishOperationKind Kind { get; }

    public CompiledPublishOperation(
        string topic,
        IReadOnlyList<IMqttValueContainer> sources,
        Func<IReadOnlyList<object>, object> transform,
        PublishOperationKind kind)
    {
        Topic = topic;
        _sources = sources;
        _transform = transform;
        Kind = kind;
    }

    public static CompiledPublishOperation CreateTransform<TInput, TOutput>(
        string topic,
        IMqttValueContainer source,
        Func<TInput, TOutput> transform,
        PublishOperationKind kind) =>
        new CompiledPublishOperation(
            topic,
            new[] { source },
            values => transform((TInput)values[0]),
            kind);

    public static CompiledPublishOperation CreateComposition<TFirst, TSecond, TOutput>(
        string topic,
        IMqttValueContainer first,
        IMqttValueContainer second,
        Func<TFirst, TSecond, TOutput> compose,
        PublishOperationKind kind) =>
        new CompiledPublishOperation(
            topic,
            new[] { first, second },
            values => compose((TFirst)values[0], (TSecond)values[1]),
            kind);

    public bool TryCapture(out CompiledPublishAttempt attempt)
    {
        if (!_sources.Any(source => source.Dirty))
        {
            attempt = null;
            return false;
        }

        SourceSnapshot[] snapshots = _sources
            .Select(source => new SourceSnapshot(source, source.Revision, source.GetSerializedValue()))
            .ToArray();

        attempt = new CompiledPublishAttempt(
            Topic,
            _transform(snapshots.Select(snapshot => snapshot.Value).ToArray()),
            snapshots,
            Kind);
        return true;
    }

    internal sealed class CompiledPublishAttempt
    {
        private readonly IReadOnlyList<SourceSnapshot> _snapshots;

        public string Topic { get; }
        public object Payload { get; }
        public PublishOperationKind Kind { get; }

        public CompiledPublishAttempt(
            string topic,
            object payload,
            IReadOnlyList<SourceSnapshot> snapshots,
            PublishOperationKind kind)
        {
            Topic = topic;
            Payload = payload;
            _snapshots = snapshots;
            Kind = kind;
        }

        public void Acknowledge()
        {
            foreach (SourceSnapshot snapshot in _snapshots)
                snapshot.Source.MarkPublished(snapshot.Revision);
        }
    }

    internal sealed record SourceSnapshot(IMqttValueContainer Source, long Revision, object Value);
}
