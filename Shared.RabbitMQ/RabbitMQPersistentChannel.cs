using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace Shared.RabbitMQ
{
    public abstract class RabbitMQPersistentChannel : IAsyncDisposable, IDisposable
    {
        private readonly ILogger<RabbitMQPersistentChannel> _logger;
        private readonly IRabbitMQPersistentConnection _persistentConnection;
        private readonly SemaphoreSlim _channelLock = new(1, 1);
        private IChannel? _channel;

        public RabbitMQPersistentChannel(
            ILogger<RabbitMQPersistentChannel> logger,
            IRabbitMQPersistentConnection persistentConnection,
            RabbitMQOptions options)
        {
            _logger = logger;
            _persistentConnection = persistentConnection;
        }

        public async ValueTask<bool> TryOpenChannelAsync()
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

        #region Properties

        public bool IsOpen => _channel is not null && _channel.IsOpen;

        protected IChannel? Channel => _channel;

        protected IRabbitMQPersistentConnection PersistentConnection => _persistentConnection;

        protected ILogger Logger => _logger;

        #endregion

        #region Abstract

        protected abstract Task ConfigureChannelAsync(IChannel channel);

        #endregion

        #region EventHandlers

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
    }
}
