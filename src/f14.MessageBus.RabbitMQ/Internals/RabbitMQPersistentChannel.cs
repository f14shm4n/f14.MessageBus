using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace f14.MessageBus.RabbitMQ.Internals
{
    internal abstract class RabbitMQPersistentChannel : IRabbitMQPersistentChannel
    {
        private readonly IRabbitMQPersistentConnection _connection;
        private readonly ILogger<RabbitMQConsumerChannel> _logger;

        private readonly SemaphoreSlim _channelLock = new(1, 1);
        private IChannel? _channel;

        public RabbitMQPersistentChannel(ILogger<RabbitMQConsumerChannel> logger, IRabbitMQPersistentConnection connection)
        {
            _logger = logger;
            _connection = connection;
        }

        public bool IsOpen => _channel is not null && _channel.IsOpen;

        public virtual async ValueTask<bool> TryOpenAsync(CancellationToken cancellationToken = default)
        {
            if (IsOpen)
            {
                return true;
            }

            if (!_connection.IsConnected)
            {
                await _connection.TryConnectAsync(cancellationToken);
            }

            await _channelLock.WaitAsync(cancellationToken);
            try
            {
                _channel = await _connection.CreateChannelAsync(cancellationToken);
                if (IsOpen)
                {
                    // TODO: add metric
                    //_logger.LogTrace("Created new RabbitMQ persistent channel.");
                    _channel.CallbackExceptionAsync += Channel_CallbackExceptionAsync;

                    await OnChannelCreatedAsync(_channel, cancellationToken);
                    return true;
                }
            }
            catch (Exception ex)
            {
                _logger.LogUnableToOpenChannel(ex);
            }
            finally
            {
                _channelLock.Release();
            }
            return false;
        }

        #region Protected

        protected ILogger Logger => _logger;

        protected abstract Task OnChannelCreatedAsync(IChannel channel, CancellationToken cancellationToken = default);

        #endregion

        #region EventHandler

        private async Task Channel_CallbackExceptionAsync(object sender, global::RabbitMQ.Client.Events.CallbackExceptionEventArgs @event)
        {
            _logger.LogChannelCallbackException();

            if (_channel is not null)
            {
                await _channel.DisposeAsync();
            }

            await TryOpenAsync(@event.CancellationToken);
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

        protected virtual void Dispose(bool disposing)
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

        protected virtual async ValueTask DisposeAsyncCore()
        {
            if (_channel is not null)
            {
                await _channel.DisposeAsync();
            }
            _channelLock.Dispose();
        }

        #endregion
    }

    internal static partial class LoggerExtensions
    {
        [LoggerMessage(Level = LogLevel.Critical, Message = "FATAL: Failed to open RabbitMQ channel.")]
        public static partial void LogUnableToOpenChannel(this ILogger logger, Exception exception);

        [LoggerMessage(Level = LogLevel.Warning, Message = "RabbitMQ channel threw an exception. Recreating channel...")]
        public static partial void LogChannelCallbackException(this ILogger logger);
    }
}
