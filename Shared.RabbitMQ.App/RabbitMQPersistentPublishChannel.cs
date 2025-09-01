using Microsoft.Extensions.Logging;
using Polly;
using Polly.Retry;
using RabbitMQ.Client;
using RabbitMQ.Client.Exceptions;
using Shared.EventBus;
using System.Net.Sockets;
using System.Text.Json;

namespace Shared.RabbitMQ.App
{
    internal sealed class RabbitMQPersistentPublishChannel : RabbitMQPersistentChannel
    {
        private readonly RabbitMQExchangeInfo _calcExchangeInfo;
        private readonly RabbitMQRetryPolicyOptions _retryPolicyOptions;
        private readonly BasicProperties _basicProperties;

        public RabbitMQPersistentPublishChannel(
            ILogger<RabbitMQPersistentPublishChannel> logger,
            IRabbitMQPersistentConnection persistentConnection,
            RabbitMQAppOptions options)
            : base(logger, persistentConnection, options)
        {
            _retryPolicyOptions = options.PublishRetryPolicy;
            _calcExchangeInfo = options.CalculatorExchange ?? throw new InvalidOperationException("CalculatorExchange info required.");

            // Can be refactord as IBasicPropertiesProvider and injected
            _basicProperties = new BasicProperties
            {
                DeliveryMode = DeliveryModes.Persistent
            };
        }

        public async Task PublishAsync(IntegrationEvent @event)
        {
            if (!IsOpen)
            {
                Logger.LogWarning("Cannot publish message to closed channel.");
                return;
            }

            var eventName = @event.GetType().Name;
            var message = JsonSerializer.SerializeToUtf8Bytes(@event);

            Logger.LogTrace("Send message to RabbitMQ: {EventId}", @event.Id);

            var policy = RetryPolicy
                .Handle<SocketException>()
                .Or<BrokerUnreachableException>()
                .WaitAndRetryAsync(_retryPolicyOptions.RetryCount, _retryPolicyOptions.CalculateDelay, (ex, time) =>
                {
                    Logger.LogWarning(ex, "Failed to publish event: {EID} after {Time} sec. ({Error})", @event.Id, $"{time.TotalSeconds:n0}", ex.Message);
                });

            await policy.ExecuteAsync(async () =>
            {
                await Channel!.BasicPublishAsync(
                    exchange: _calcExchangeInfo.Name,
                    routingKey: eventName,
                    mandatory: true,
                    basicProperties: _basicProperties,
                    body: message);
            });
        }

        protected override async Task ConfigureChannelAsync(IChannel channel)
        {
            Logger.LogTrace("Declaring RabbitMQ exchange.");

            await channel.ExchangeDeclareAsync(
                exchange: _calcExchangeInfo.Name,
                type: _calcExchangeInfo.Type);
        }
    }
}
