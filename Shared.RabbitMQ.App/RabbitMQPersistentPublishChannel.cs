using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using Shared.EventBus;
using System.Text.Json;

namespace Shared.RabbitMQ.App
{
    internal sealed class RabbitMQPersistentPublishChannel : RabbitMQPersistentChannel
    {
        private readonly RabbitMQExchangeInfo _calcExchangeInfo;

        public RabbitMQPersistentPublishChannel(
            ILogger<RabbitMQPersistentPublishChannel> logger,
            IRabbitMQPersistentConnection persistentConnection,
            RabbitMQAppOptions options)
            : base(logger, persistentConnection)
        {
            _calcExchangeInfo = options.CalculatorExchange ?? throw new InvalidOperationException("CalculatorExchange info required.");
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

            var properties = new BasicProperties
            {
                DeliveryMode = DeliveryModes.Persistent
            };
            await Channel!.BasicPublishAsync(
                exchange: _calcExchangeInfo.Name,
                routingKey: eventName,
                mandatory: true,
                basicProperties: properties,
                body: message);
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
