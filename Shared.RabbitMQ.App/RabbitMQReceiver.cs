using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Shared.EventBus;
using System.Text;

namespace Shared.RabbitMQ.App
{
    internal sealed class RabbitMQReceiver : RabbitMQReceiverBase
    {
        private readonly IIntegrationEventProcessor _eventProcessor;
        private readonly RabbitMQExchangeInfo _exchangeInfo;

        public RabbitMQReceiver(
            IIntegrationEventProcessor eventProcessor,
            IEventBusSubscriptionsManager subscriptionsManager,
            IRabbitMQPersistentConnection persistentConnection,
            ILogger<RabbitMQReceiver> logger,
            RabbitMQAppOptions options)
            : base(subscriptionsManager, persistentConnection, logger)
        {
            _eventProcessor = eventProcessor;
            _exchangeInfo = options.CalculatorExchange ?? throw new InvalidOperationException("CalculatorExchange info required.");
        }

        #region Protected

        protected override async Task BindQueueAsync<E>()
        {
            if (!IsOpen)
            {
                Logger.LogWarning("Cannot bind queue to closed channel.");
                return;
            }

            await Channel!.QueueBindAsync(
                queue: _exchangeInfo.Queue,
                exchange: _exchangeInfo.Name,
                routingKey: GetEventTypeName<E>());
        }

        protected override async Task UnbindQueueAsync(Type eventType)
        {
            if (!IsOpen)
            {
                Logger.LogWarning("Cannot unbind queue to closed channel.");
                return;
            }

            await Channel!.QueueUnbindAsync(
                queue: _exchangeInfo.Queue,
                exchange: _exchangeInfo.Name,
                routingKey: eventType.Name);
        }

        protected override async Task ConfigureChannelAsync(IChannel channel)
        {
            Logger.LogTrace("Declaring RabbitMQ exchange.");

            await channel.ExchangeDeclareAsync(
                exchange: _exchangeInfo.Name,
                type: _exchangeInfo.Type);

            await channel.QueueDeclareAsync(
                queue: _exchangeInfo.Queue,
                durable: true,
                exclusive: false,
                autoDelete: false);

            Logger.LogTrace("Starting RabbitMQ consume.");

            var consumer = new AsyncEventingBasicConsumer(channel);
            consumer.ReceivedAsync += Consumer_Received;

            await channel.BasicConsumeAsync(
                queue: _exchangeInfo.Queue,
                autoAck: false,
                consumer: consumer);
        }

        #endregion        

        #region EventHandler

        private async Task Consumer_Received(object sender, BasicDeliverEventArgs args)
        {
            var eventName = args.RoutingKey;
            var message = Encoding.UTF8.GetString(args.Body.ToArray());

            try
            {
                await _eventProcessor.ProcessEventAsync(eventName, message);
            }
            catch (Exception ex)
            {
                Logger.LogWarning(ex, "Cannot process event message. Event: '{EventName}' Message: '{Message}'", eventName, message);
            }

            // TODO: Handle message if error has been occurred (Dead message exchange).

            await Channel!.BasicAckAsync(args.DeliveryTag, multiple: false);
        }

        #endregion               
    }
}
