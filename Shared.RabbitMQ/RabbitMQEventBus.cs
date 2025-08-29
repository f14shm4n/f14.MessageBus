using Microsoft.Extensions.Logging;
using Shared.EventBus;

namespace Shared.RabbitMQ
{
    internal sealed class RabbitMQEventBus : IRabbitMQEventBus
    {
        private readonly ILogger<RabbitMQEventBus> _logger;
        private readonly IRabbitMQSubscriptionsManager _subscriptionsManager;
        private readonly DefaultRabbitMQPersistentPublishChannel _publishChannel;
        private readonly DefaultRabbitMQPersistentConsumerChannel _consumerChannel;

        public RabbitMQEventBus(
            ILoggerFactory loggerFactory,
            IRabbitMQSubscriptionsManager subscriptionsManager,
            DefaultRabbitMQPersistentPublishChannel publishChannel,
            DefaultRabbitMQPersistentConsumerChannel consumerChannel)
        {
            _logger = loggerFactory.CreateLogger<RabbitMQEventBus>();
            _subscriptionsManager = subscriptionsManager;
            _publishChannel = publishChannel;
            _consumerChannel = consumerChannel;

            _subscriptionsManager.OnEventRemoved += SubscriptionsManager_OnEventRemoved;
        }

        public async Task PublishAsync(IntegrationEvent @event)
        {
            if (!_publishChannel.IsOpen)
            {
                await _publishChannel.TryOpenChannelAsync();
            }
            await _publishChannel.PublishAsync(@event);
        }

        public async Task SubscribeAsync<E, H>()
            where E : IntegrationEvent
            where H : IIntegrationEventHandler<E>
        {
            if (!_subscriptionsManager.Contains<E>())
            {
                if (!_consumerChannel.IsOpen)
                {
                    await _consumerChannel.TryOpenChannelAsync();
                }
                await _consumerChannel.BindAsync<E>();

                _logger.LogInformation("Subscribing to event {E} with {H}", typeof(E), typeof(H).Name);

                _subscriptionsManager.AddSubscription<E, H>();
            }
        }

        public Task UnsubscribeAsync<E, H>()
            where E : IntegrationEvent
            where H : IIntegrationEventHandler<E>
        {
            _logger.LogInformation("Unsubscribing from event {E}", typeof(E).Name);

            _subscriptionsManager.RemoveSubscription<E, H>();
            return Task.CompletedTask;
        }

        #region EventHandlers

        private async void SubscriptionsManager_OnEventRemoved(object? sender, EventRemovedEventArgs args)
        {
            if (!_consumerChannel.IsOpen)
            {
                await _consumerChannel.TryOpenChannelAsync();
            }
            await _consumerChannel.UnbindAsync(args.EventType);
        }

        #endregion
    }
}
