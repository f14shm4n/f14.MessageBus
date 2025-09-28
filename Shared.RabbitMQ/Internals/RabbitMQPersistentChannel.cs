using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Shared.EventBus;

namespace Shared.RabbitMQ
{
    internal sealed class RabbitMQPersistentChannel : IRabbitMQPersistentChannel
    {
        private readonly IRabbitMQPersistentConnection _connection;
        private readonly ILogger<RabbitMQPersistentChannel> _logger;
        private readonly IRabbitMQEndPointCollection _endPoints;
        private readonly IMessageProcessor _eventProcessor;

        private readonly SemaphoreSlim _channelLock = new(1, 1);
        private IChannel? _channel;

        public RabbitMQPersistentChannel(
            ILogger<RabbitMQPersistentChannel> logger,
            IRabbitMQPersistentConnection connection,
            IMessageProcessor eventProcessor,
            IRabbitMQEndPointCollection endPoints)
        {
            _logger = logger;
            _connection = connection;
            _eventProcessor = eventProcessor;
            _endPoints = endPoints;
        }

        public bool IsOpen => _channel is not null && _channel.IsOpen;

        public async ValueTask<bool> TryOpenAsync(CancellationToken cancellationToken = default)
        {
            if (_endPoints.Count == 0)
            {
                return false;
            }

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
                    _logger.LogTrace("Created new RabbitMQ channel: '{ChannelType}'.", GetType().Name);
                    _logger.LogTrace("Starting RabbitMQ consume.");

                    var consumer = new AsyncEventingBasicConsumer(_channel);
                    consumer.ReceivedAsync += Consumer_Received;

                    foreach (var name in _endPoints.Select(x => x.Queue).ToHashSet())
                    {
                        await _channel.BasicConsumeAsync(
                            queue: name,
                            autoAck: false,
                            consumer: consumer,
                            cancellationToken: cancellationToken);
                    }

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

        #region EventHandler

        private async Task Consumer_Received(object sender, BasicDeliverEventArgs args)
        {
            try
            {
                await _eventProcessor.ProcessMessageAsync(args.RoutingKey, args.Body);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Cannot process message. MessageType(routingKey): '{Type}'", args.RoutingKey);
            }

            // TODO: Handle message if error has been occurred (Dead message exchange).

            await _channel!.BasicAckAsync(args.DeliveryTag, multiple: false);
        }

        private async Task Channel_CallbackExceptionAsync(object sender, global::RabbitMQ.Client.Events.CallbackExceptionEventArgs @event)
        {
            _logger.LogWarning("RabbitMQ channel threw an exception. Recreating channel...");

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
