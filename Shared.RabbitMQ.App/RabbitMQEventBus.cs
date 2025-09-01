using Microsoft.Extensions.Logging;
using Shared.EventBus;

namespace Shared.RabbitMQ.App
{
    internal sealed class RabbitMQEventBus : IEventBusPublisher, IEventBusReceiver
    {
        private readonly ILogger<RabbitMQEventBus> _logger;
        private readonly IRabbitMQSubscriptionsManager<SubscriptionInfo> _subscriptionsManager;
        private readonly RabbitMQPersistentPublishChannel _publishChannel;
        private readonly RabbitMQPersistentConsumerChannel _consumerChannel;

        public RabbitMQEventBus(
            ILogger<RabbitMQEventBus> logger,
            IRabbitMQSubscriptionsManager<SubscriptionInfo> subscriptionsManager,
            RabbitMQPersistentPublishChannel publishChannel,
            RabbitMQPersistentConsumerChannel consumerChannel)
        {
            _logger = logger;
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
            if (!_consumerChannel.IsOpen)
            {
                await _consumerChannel.TryOpenChannelAsync();
            }
            await _consumerChannel.BindQueueAsync<E>();

            _logger.LogInformation("Subscribing to event {E} with {H}", typeof(E), typeof(H).Name);

            _subscriptionsManager.AddSubscription<E, H>();
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
            await _consumerChannel.UnbindQueueAsync(args.EventType);
        }

        #endregion
    }
}
