using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using Shared.EventBus;

namespace Shared.RabbitMQ
{
    public abstract class RabbitMQReceiverBase : IEventBusReceiver, IAsyncDisposable, IDisposable
    {
        private readonly IEventBusSubscriptionsManager _subscriptionsManager;
        private readonly IRabbitMQPersistentConnection _persistentConnection;
        private readonly ILogger<RabbitMQReceiverBase> _logger;

        private readonly SemaphoreSlim _channelLock = new(1, 1);
        private IChannel? _channel;

        public RabbitMQReceiverBase(
            IEventBusSubscriptionsManager subscriptionsManager,
            IRabbitMQPersistentConnection persistentConnection,
            ILogger<RabbitMQReceiverBase> logger)
        {
            _subscriptionsManager = subscriptionsManager;
            _persistentConnection = persistentConnection;
            _logger = logger;

            _subscriptionsManager.OnEventRemoved += SubscriptionsManager_OnEventRemoved;
        }

        #region Public

        public async Task SubscribeAsync<E, H>()
            where E : IntegrationEvent
            where H : IIntegrationEventHandler<E>
        {
            if (!IsOpen)
            {
                await TryOpenChannelAsync();
            }
            await BindQueueAsync<E>();

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

        #endregion

        #region Protected

        protected IChannel? Channel => _channel;

        protected ILogger Logger => _logger;

        protected bool IsOpen => _channel is not null && _channel.IsOpen;

        protected abstract Task BindQueueAsync<E>() where E : IntegrationEvent;

        protected abstract Task UnbindQueueAsync(Type eventType);

        protected abstract Task ConfigureChannelAsync(IChannel channel);

        #endregion

        #region Private        

        private async ValueTask<bool> TryOpenChannelAsync()
        {
            if (IsOpen)
            {
                return true;
            }

            if (!_persistentConnection.IsConnected)
            {
                await _persistentConnection.TryConnectAsync();
            }

            await _channelLock.WaitAsync();
            try
            {
                _channel = await _persistentConnection.CreateChannelAsync();
                if (IsOpen)
                {
                    _logger.LogTrace("Created new RabbitMQ channel: '{ChannelType}'.", GetType().Name);

                    await ConfigureChannelAsync(_channel);
                    _channel.CallbackExceptionAsync += Channel_CallbackExceptionAsync;
                    return true;
                }
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "FATAL: Failed to open RabbitMQ channel.");
            }
            finally
            {
                _channelLock.Release();
            }
            return false;
        }

        #endregion

        #region EventHandler

        private async void SubscriptionsManager_OnEventRemoved(object? sender, EventRemovedEventArgs args)
        {
            if (!IsOpen)
            {
                await TryOpenChannelAsync();
            }
            await UnbindQueueAsync(args.EventType);
        }

        private async Task Channel_CallbackExceptionAsync(object sender, global::RabbitMQ.Client.Events.CallbackExceptionEventArgs @event)
        {
            _logger.LogWarning("RabbitMQ channel threw an exception. Recreating channel...");

            if (_channel is not null)
            {
                await _channel.DisposeAsync();
            }

            await TryOpenChannelAsync();
        }

        #endregion

        #region Disposable

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public async ValueTask DisposeAsync()
        {
            await DisposeAsyncCore();

            Dispose(false);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_channel is IDisposable disposable)
                {
                    disposable.Dispose();
                    _channel = null;
                }
                _channelLock.Dispose();
            }
        }

        private async ValueTask DisposeAsyncCore()
        {
            if (_channel is not null)
            {
                await _channel.DisposeAsync();
            }
            _channelLock.Dispose();
        }

        #endregion

        #region Static

        protected static string GetEventTypeName<E>() where E : IntegrationEvent
        {
            return typeof(E).Name;
        }

        #endregion
    }
}
