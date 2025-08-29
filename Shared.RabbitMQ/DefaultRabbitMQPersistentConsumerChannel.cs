using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Shared.EventBus;
using System;
using System.Text;
using System.Text.Json;
using System.Threading.Channels;

namespace Shared.RabbitMQ
{
    internal sealed class DefaultRabbitMQPersistentConsumerChannel : RabbitMQPersistentChannel
    {
        private readonly RabbitMQExchangeInfo _calcExchangeInfo;
        private readonly IRabbitMQSubscriptionsManager _subscriptionsManager;
        private readonly IServiceScopeFactory _scopeFactory;

        public DefaultRabbitMQPersistentConsumerChannel(
            IServiceScopeFactory scopeFactory,
            IRabbitMQSubscriptionsManager subscriptionsManager,
            ILogger<DefaultRabbitMQPersistentConsumerChannel> logger,
            IRabbitMQPersistentConnection persistentConnection,
            RabbitMQOptions options)
            : base(logger, persistentConnection)
        {
            _scopeFactory = scopeFactory;
            _subscriptionsManager = subscriptionsManager;
            _calcExchangeInfo = options.CalculatorExchange ?? throw new InvalidOperationException("CalculatorExchange info required.");
        }

        public async Task BindAsync<E>() where E : IntegrationEvent
        {
            if (Channel is not null && IsOpen)
            {
                await Channel.QueueBindAsync(
                    queue: _calcExchangeInfo.Queue,
                    exchange: _calcExchangeInfo.Name,
                    routingKey: GetEventTypeName<E>());
            }
            else
            {
                // TODO: log
            }
        }

        public async Task UnbindAsync(Type eventType)
        {
            if (Channel is not null && IsOpen)
            {
                await Channel.QueueUnbindAsync(
                    queue: _calcExchangeInfo.Queue,
                    exchange: _calcExchangeInfo.Name,
                    routingKey: eventType.Name);
            }
            else
            {
                // TODO: log
            }
        }

        protected override async Task ConfigureChannelAsync(IChannel channel)
        {
            Logger.LogTrace("Declaring RabbitMQ exchange.");

            await channel.ExchangeDeclareAsync(
                exchange: _calcExchangeInfo.Name,
                type: _calcExchangeInfo.Type);

            await channel.QueueDeclareAsync(
                queue: _calcExchangeInfo.Queue,
                durable: true,
                exclusive: false,
                autoDelete: false);

            Logger.LogTrace("Starting RabbitMQ consume.");

            var consumer = new AsyncEventingBasicConsumer(channel);
            consumer.ReceivedAsync += Consumer_Received;

            await channel.BasicConsumeAsync(
                queue: _calcExchangeInfo.Queue,
                autoAck: false,
                consumer: consumer);
        }

        private async Task ProcessEventAsync(string eventName, string message)
        {
            Logger.LogTrace("Processing RabbitMQ event: {EventName}", eventName);

            var eventType = _subscriptionsManager.GetEventTypeByName(eventName);
            if (eventType is not null)
            {
                using (var scope = _scopeFactory.CreateScope())
                {
                    var subscriptions = _subscriptionsManager.GetSubscriptions(eventType);
                    if (subscriptions is not null)
                    {
                        var jsonOptions = new JsonSerializerOptions()
                        {
                            PropertyNameCaseInsensitive = true
                        };

                        foreach (var subscription in subscriptions)
                        {
                            var handler = ActivatorUtilities.CreateInstance(scope.ServiceProvider, subscription.EventHandlerType);
                            var integrationEvent = JsonSerializer.Deserialize(message, eventType, jsonOptions);
                            var concreteType = typeof(IIntegrationEventHandler<>).MakeGenericType(eventType);

                            await Task.Yield();
                            await (Task)concreteType.GetMethod("Handle")!.Invoke(handler, [integrationEvent])!;
                        }
                    }
                    else
                    {
                        Logger.LogWarning("No event handlers was found for the event: '{}', the event handler collection may be empty or was unsubscribed from the current event while attempting to handle it.", eventName);
                    }
                }
            }
            else
            {
                Logger.LogWarning("No subscription for RabbitMQ event: {EventName}", eventName);
            }
        }

        #region EventHandler

        private async Task Consumer_Received(object sender, BasicDeliverEventArgs args)
        {
            var eventName = args.RoutingKey;
            var message = Encoding.UTF8.GetString(args.Body.ToArray());

            try
            {
                await ProcessEventAsync(eventName, message);
            }
            catch (Exception ex)
            {
                Logger.LogWarning(ex, "Cannot process event message. Event: '{EventName}' Message: '{Message}'", eventName, message);
            }

            // TODO: Handle message if error has been occurred.

            await Channel!.BasicAckAsync(args.DeliveryTag, multiple: false);
        }

        #endregion

        #region Static

        private static string GetEventTypeName<E>() where E : IntegrationEvent
        {
            return typeof(E).Name;
        }

        #endregion
    }
}
