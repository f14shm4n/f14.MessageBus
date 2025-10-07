using f14.RetryPolly;
using Microsoft.Extensions.Logging;
using Polly;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;
using System.Net.Sockets;

namespace f14.MessageBus.RabbitMQ.Internals
{
    internal sealed class RabbitMQPersistentConnection : IRabbitMQPersistentConnection
    {
        private readonly ILogger<RabbitMQPersistentConnection> _logger;
        private readonly IConnectionFactory _connectionFactory;
        private readonly SemaphoreSlim _connLock = new(1, 1);
        private readonly IRabbitMQPersistentConnectionConfiguration _config;
        private IConnection? _connection;

        public RabbitMQPersistentConnection(
            ILogger<RabbitMQPersistentConnection> logger,
            IConnectionFactoryProvider connectionFactoryProvider,
            IRabbitMQPersistentConnectionConfiguration config)
        {
            _logger = logger;
            _config = config;
            _connectionFactory = connectionFactoryProvider.GetConnectionFactory();
        }

        public bool IsConnected => _connection != null && _connection.IsOpen;

        public Task<IChannel> CreateChannelAsync(CancellationToken cancellationToken = default)
        {
            if (!IsConnected)
            {
                ThrowHelper.NoAvailableConnection();
            }
            return _connection!.CreateChannelAsync(cancellationToken: cancellationToken);
        }

        public async ValueTask<bool> TryConnectAsync(CancellationToken cancellationToken = default)
        {
            if (IsConnected)
            {
                return true;
            }

            await _connLock.WaitAsync(cancellationToken);
            try
            {
                var polly = Policy
                    .Handle<SocketException>()
                    .Or<BrokerUnreachableException>()
                    .WaitAndRetryAsync(_config.RetryPolicy.RetryCount, _config.RetryPolicy.CalculateDelay, (ex, time) =>
                    {
                        _logger.LogRetryFailedToEstablishConnection(time.TotalSeconds, ex.Message);
                    });

                await polly.ExecuteAsync(async ct => _connection = await _connectionFactory.CreateConnectionAsync(ct), cancellationToken);

                if (!IsConnected)
                {
                    _logger.LogCannotCreateOrOpenConnection();
                    return false;
                }

                _connection!.CallbackExceptionAsync += Connection_CallbackExceptionAsync;
                _connection.ConnectionShutdownAsync += Connection_ConnectionShutdownAsync;
                _connection.ConnectionBlockedAsync += Connection_ConnectionBlockedAsync;

                _logger.LogConnectionEstablished(_connection.Endpoint.HostName);
                return true;
            }
            finally
            {
                _connLock.Release();
            }
        }

        #region EventHandlers

        private async Task Connection_ConnectionBlockedAsync(object sender, ConnectionBlockedEventArgs @event)
        {
            _logger.LogConnectionBlocked();

            await TryConnectAsync(@event.CancellationToken);
        }

        private async Task Connection_ConnectionShutdownAsync(object sender, ShutdownEventArgs @event)
        {
            _logger.LogConnectionShutdown();

            await TryConnectAsync(@event.CancellationToken);
        }

        private async Task Connection_CallbackExceptionAsync(object sender, CallbackExceptionEventArgs @event)
        {
            _logger.LogConnectionCallbackException();

            await TryConnectAsync(@event.CancellationToken);
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

    internal static partial class LoggerExtensions
    {
        [LoggerMessage(Level = LogLevel.Warning, Message = "RabbitMQ client failed to establish connection {Time:n0} sec. Error: '{Error}'.")]
        public static partial void LogRetryFailedToEstablishConnection(this ILogger logger, double time, string error);

        [LoggerMessage(Level = LogLevel.Critical, Message = "FATAL: Cannot create or open new RabbitMQ connection.")]
        public static partial void LogCannotCreateOrOpenConnection(this ILogger logger);

        [LoggerMessage(Level = LogLevel.Information, Message = "The RabbitMQ client established a new connection to '{Host}'.")]
        public static partial void LogConnectionEstablished(this ILogger logger, string host);

        [LoggerMessage(Level = LogLevel.Warning, Message = "RabbitMQ connection blocked. Reconnecting...")]
        public static partial void LogConnectionBlocked(this ILogger logger);

        [LoggerMessage(Level = LogLevel.Warning, Message = "RabbitMQ connection shutdown. Reconnecting...")]
        public static partial void LogConnectionShutdown(this ILogger logger);

        [LoggerMessage(Level = LogLevel.Warning, Message = "RabbitMQ connection threw an exception. Reconnecting...")]
        public static partial void LogConnectionCallbackException(this ILogger logger);
    }
}
