using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MBW.HassMQTT.Interfaces;
using MBW.HassMQTT.Services;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MQTTnet;
using MQTTnet.Exceptions;
using MQTTnet.Packets;

namespace MBW.HassMQTT;

internal sealed class MqttClientLifetimeService : BackgroundService, IHassMqttClient
{
    private readonly IMqttClient _client;
    private readonly MqttClientConnectionOptions _options;
    private readonly MqttEvents _events;
    private readonly MqttMessageDistributor _messages;
    private readonly ILogger<MqttClientLifetimeService> _logger;
    private readonly SemaphoreSlim _operationLock = new SemaphoreSlim(1, 1);
    private readonly ConcurrentDictionary<string, MqttTopicFilter> _subscriptions = new(StringComparer.Ordinal);
    private TaskCompletionSource<MqttClientDisconnectedEventArgs> _disconnected = NewDisconnectSource();
    private CancellationToken _stoppingToken;

    public bool IsConnected => _client.IsConnected;

    public MqttClientLifetimeService(
        IMqttClient client,
        MqttClientConnectionOptions options,
        MqttEvents events,
        MqttMessageDistributor messages,
        ILogger<MqttClientLifetimeService> logger)
    {
        _client = client;
        _options = options;
        _events = events;
        _messages = messages;
        _logger = logger;

        _client.DisconnectedAsync += OnDisconnected;
        _client.ApplicationMessageReceivedAsync += args => _messages.OnMessage(args.ApplicationMessage, _stoppingToken);
    }

    public async Task<MqttClientPublishResult> PublishAsync(MqttApplicationMessage message, CancellationToken token = default)
    {
        await _operationLock.WaitAsync(token);
        try
        {
            if (!_client.IsConnected)
                throw new MqttClientNotConnectedException();

            return await _client.PublishAsync(message, token);
        }
        finally
        {
            _operationLock.Release();
        }
    }

    public async Task SubscribeAsync(MqttClientSubscribeOptions options, CancellationToken token = default)
    {
        foreach (MqttTopicFilter filter in options.TopicFilters)
            _subscriptions[filter.Topic] = filter;

        await _operationLock.WaitAsync(token);
        try
        {
            if (_client.IsConnected)
                await _client.SubscribeAsync(options, token);
        }
        finally
        {
            _operationLock.Release();
        }
    }

    public async Task UnsubscribeAsync(MqttClientUnsubscribeOptions options, CancellationToken token = default)
    {
        foreach (string filter in options.TopicFilters)
            _subscriptions.TryRemove(filter, out _);

        await _operationLock.WaitAsync(token);
        try
        {
            if (_client.IsConnected)
                await _client.UnsubscribeAsync(options, token);
        }
        finally
        {
            _operationLock.Release();
        }
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _stoppingToken = stoppingToken;
        TimeSpan reconnectDelay = _options.ReconnectInterval;
        DateTimeOffset nextWarning = DateTimeOffset.MinValue;
        int attempts = 0;

        while (!stoppingToken.IsCancellationRequested)
        {
            MqttClientDisconnectedEventArgs disconnectedArgs = null;
            try
            {
                _disconnected = NewDisconnectSource();
                MqttClientConnectResult connectResult = await Connect(stoppingToken);
                attempts = 0;
                reconnectDelay = _options.ReconnectInterval;
                nextWarning = DateTimeOffset.MinValue;

                await RestoreSubscriptions(stoppingToken);
                await _events.InvokeConnectHandler(new MqttClientConnectedEventArgs(connectResult), stoppingToken);

                disconnectedArgs = await _disconnected.Task.WaitAsync(stoppingToken);
                await _events.InvokeDisconnectHandler(disconnectedArgs, stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception exception)
            {
                attempts++;
                DateTimeOffset now = DateTimeOffset.UtcNow;
                if (now >= nextWarning)
                {
                    _logger.LogWarning(exception, "Unable to connect to MQTT broker after {Attempts} attempt(s); retrying in {Delay}", attempts, reconnectDelay);
                    nextWarning = now + _options.ReconnectWarningInterval;
                }
                else
                {
                    _logger.LogDebug(exception, "Unable to connect to MQTT broker; retrying in {Delay}", reconnectDelay);
                }
            }

            if (disconnectedArgs != null)
                _logger.LogWarning(disconnectedArgs.Exception, "MQTT connection closed: {Reason} {ReasonString}", disconnectedArgs.Reason, disconnectedArgs.ReasonString);

            try
            {
                double jitter = 0.8 + Random.Shared.NextDouble() * 0.4;
                await Task.Delay(TimeSpan.FromMilliseconds(reconnectDelay.TotalMilliseconds * jitter), stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }

            reconnectDelay = TimeSpan.FromMilliseconds(Math.Min(reconnectDelay.TotalMilliseconds * 2, _options.MaximumReconnectInterval.TotalMilliseconds));
        }
    }

    private async Task<MqttClientConnectResult> Connect(CancellationToken stoppingToken)
    {
        using CancellationTokenSource timeout = CancellationTokenSource.CreateLinkedTokenSource(stoppingToken);
        timeout.CancelAfter(_options.ConnectTimeout);

        await _operationLock.WaitAsync(stoppingToken);
        try
        {
            MqttClientConnectResult result = await _client.ConnectAsync(_options.ClientOptions, timeout.Token);
            _logger.LogInformation("Connected to MQTT broker with result {ResultCode}; session present: {SessionPresent}", result.ResultCode, result.IsSessionPresent);
            return result;
        }
        finally
        {
            _operationLock.Release();
        }
    }

    private async Task RestoreSubscriptions(CancellationToken token)
    {
        MqttTopicFilter[] filters = _subscriptions.Values.ToArray();
        if (filters.Length == 0)
            return;

        await _operationLock.WaitAsync(token);
        try
        {
            if (_client.IsConnected)
                await _client.SubscribeAsync(new MqttClientSubscribeOptions { TopicFilters = filters.ToList() }, token);
        }
        finally
        {
            _operationLock.Release();
        }
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        await _events.InvokeStoppingHandler(cancellationToken);
        await base.StopAsync(cancellationToken);

        await _operationLock.WaitAsync(cancellationToken);
        try
        {
            if (_client.IsConnected)
                await _client.DisconnectAsync(new MqttClientDisconnectOptions(), cancellationToken);
        }
        finally
        {
            _operationLock.Release();
            _client.Dispose();
        }
    }

    private Task OnDisconnected(MqttClientDisconnectedEventArgs args)
    {
        _disconnected.TrySetResult(args);
        return Task.CompletedTask;
    }

    private static TaskCompletionSource<MqttClientDisconnectedEventArgs> NewDisconnectSource() =>
        new(TaskCreationOptions.RunContinuationsAsynchronously);
}
