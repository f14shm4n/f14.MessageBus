using Microsoft.Extensions.Logging;
using Polly;
using Polly.Retry;
using RabbitMQ.Client;
using RabbitMQ.Client.Exceptions;
using Shared.EventBus;
using System.Net.Sockets;
using System.Text.Json;

namespace Shared.RabbitMQ
{
    public abstract class RabbitMQPublisherBase : IEventBusPublisher
    {
        private const int RetryCount = 5;
        private const int RetryDelay = 2000;

        private readonly ILogger<RabbitMQPublisherBase> _logger;
        private readonly IRabbitMQPersistentConnection _persistentConnection;

        public RabbitMQPublisherBase(ILogger<RabbitMQPublisherBase> logger, IRabbitMQPersistentConnection persistentConnection)
        {
            _logger = logger;
            _persistentConnection = persistentConnection;
        }

        public async Task PublishAsync(IntegrationEvent @event)
        {
            if (!_persistentConnection.IsConnected)
            {
                await _persistentConnection.TryConnectAsync();
            }

            try
            {
                _logger.LogTrace("Creating new RabbitMQ channel for publish.");
                await using (var channel = await _persistentConnection.CreateChannelAsync())
                {
                    _logger.LogTrace("Declaring RabbitMQ exchange.");

                    await ConfigureExchangeDeclareAsync(channel);

                    var eventName = @event.GetType().Name;
                    var message = JsonSerializer.SerializeToUtf8Bytes(@event);

                    _logger.LogTrace("Sending message to RabbitMQ: {EventId}", @event.Id);

                    await ExecuteRetryPolicyAsync(channel, GetRetryPolicy(@event), eventName, message);
                }
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "FATAL: Failed to send RabbitMQ message. EventID: {EID}", @event.Id);
            }
        }

        protected ILogger Logger => _logger;

        protected abstract Task ConfigureExchangeDeclareAsync(IChannel channel);

        protected abstract Task ExecuteRetryPolicyAsync(IChannel channel, AsyncRetryPolicy policy, string eventName, byte[] message);

        protected virtual AsyncRetryPolicy GetRetryPolicy(IntegrationEvent @event)
        {
            return RetryPolicy
                .Handle<SocketException>()
                .Or<BrokerUnreachableException>()
                .WaitAndRetryAsync(RetryCount, retryAttempt => TimeSpan.FromMilliseconds(RetryDelay * retryAttempt), (ex, time) =>
                {
                    _logger.LogWarning(ex, "Failed to publish event: {EID} after {Time} sec. ({Error})", @event.Id, $"{time.TotalSeconds:n0}", ex.Message);
                });
        }
    }
}
