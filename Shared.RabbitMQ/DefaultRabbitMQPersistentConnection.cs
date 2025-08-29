using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Shared.RabbitMQ
{
    internal sealed class DefaultRabbitMQPersistentConnection : IRabbitMQPersistentConnection
    {
        private readonly ILogger<DefaultRabbitMQPersistentConnection> _logger;
        private readonly ConnectionFactory _connectionFactory;
        private readonly SemaphoreSlim _connLock = new(1, 1);
        private IConnection? _connection;

        public DefaultRabbitMQPersistentConnection(
            ILogger<DefaultRabbitMQPersistentConnection> logger,
            RabbitMQOptions options)
        {
            _logger = logger;
            _connectionFactory = new ConnectionFactory() { HostName = options.ConnectionString };
        }

        public bool IsConnected => _connection != null && _connection.IsOpen;

        public Task<IChannel> CreateChannelAsync()
        {
            if (!IsConnected)
            {
                throw new InvalidOperationException("No RabbitMQ connection available.");
            }
            return _connection!.CreateChannelAsync();
        }

        public async ValueTask<bool> TryConnectAsync()
        {
            if (IsConnected)
            {
                return true;
            }

            await _connLock.WaitAsync();
            try
            {
                _connection = await _connectionFactory.CreateConnectionAsync();
                if (IsConnected)
                {
                    _connection.CallbackExceptionAsync += Connection_CallbackExceptionAsync;
                    _connection.ConnectionShutdownAsync += Connection_ConnectionShutdownAsync;
                    _connection.ConnectionBlockedAsync += Connection_ConnectionBlockedAsync;

                    _logger.LogInformation("The RabbitMQ client established a new connection to '{HostName}'.", _connection.Endpoint.HostName);
                    return true;
                }
                else
                {
                    _logger.LogCritical("FATAL: Cannot create and open new RabbitMQ connection.");
                    return false;
                }
            }
            finally
            {
                _connLock.Release();
            }
        }

        #region EventHandlers

        private async Task Connection_ConnectionBlockedAsync(object sender, ConnectionBlockedEventArgs @event)
        {
            _logger.LogWarning("RabbitMQ connection blocked. Reconnecting...");

            await TryConnectAsync();
        }

        private async Task Connection_ConnectionShutdownAsync(object sender, ShutdownEventArgs @event)
        {
            _logger.LogWarning("RabbitMQ connection shutdown. Reconnecting...");

            await TryConnectAsync();
        }

        private async Task Connection_CallbackExceptionAsync(object sender, CallbackExceptionEventArgs @event)
        {
            _logger.LogWarning("RabbitMQ connection threw an exception. Reconnecting...");

            await TryConnectAsync();
        }

        #endregion

        #region IDisposable 

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
                if (_connection is IDisposable disposable)
                {
                    disposable.Dispose();
                    _connection = null;
                }
                _connLock.Dispose();
            }
        }

        private async Task DisposeAsyncCore()
        {
            if (_connection is not null)
            {
                await _connection.DisposeAsync();
                _connection = null;
            }
            _connLock.Dispose();
        }

        #endregion
    }
}
