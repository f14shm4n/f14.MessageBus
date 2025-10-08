using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace f14.MessageBus.RabbitMQ.Internals
{
    internal sealed class RabbitMQBusInstance : IBusInstance, IRabbitMQBusSender
    {
        private readonly ILogger<RabbitMQBusInstance> _logger;
        private readonly IRabbitMQPersistentConnection _connection;
        private readonly IBasicPropertiesProvider _basicPropertiesProvider;
        private readonly IRabbitMQDeclarationCollection _declarations;
        private readonly IRabbitMQPublisher? _publisher;
        private readonly IRabbitMQConsumerChannel? _consumerChannel;

        public RabbitMQBusInstance(
            ILogger<RabbitMQBusInstance> logger,
            IRabbitMQPersistentConnection connection,
            IBasicPropertiesProvider basicPropertiesProvider,
            IRabbitMQDeclarationCollection declarations,
            IRabbitMQPublisher? publisher)
            : this(logger, connection, basicPropertiesProvider, declarations, publisher, null)
        {
        }

        public RabbitMQBusInstance(
            ILogger<RabbitMQBusInstance> logger,
            IRabbitMQPersistentConnection connection,
            IBasicPropertiesProvider basicPropertiesProvider,
            IRabbitMQDeclarationCollection declarations,
            IRabbitMQConsumerChannel? consumerChannel)
            : this(logger, connection, basicPropertiesProvider, declarations, null, consumerChannel)
        {
        }

        public RabbitMQBusInstance(
            ILogger<RabbitMQBusInstance> logger,
            IRabbitMQPersistentConnection connection,
            IBasicPropertiesProvider basicPropertiesProvider,
            IRabbitMQDeclarationCollection declarations,
            IRabbitMQPublisher? publisher,
            IRabbitMQConsumerChannel? consumerChannel)
        {
            _logger = logger;
            _connection = connection;
            _basicPropertiesProvider = basicPropertiesProvider;
            _declarations = declarations;
            _publisher = publisher;
            _consumerChannel = consumerChannel;
        }

        public Task SendAsync<TMessage>(TMessage message, CancellationToken cancellationToken = default)
        {
            if (_publisher is null)
            {
                ThrowHelper.PublisherIsNotSet();
            }
            return _publisher.PublishAsync(message, _basicPropertiesProvider.GetBasicProperties(), cancellationToken);
        }

        public async Task StartAsync(CancellationToken cancellationToken = default)
        {
            _logger.LogStartBusInstance();

            // Open RabbitMQ connection
            await _connection.TryConnectAsync(cancellationToken).ConfigureAwait(false);
            if (!_connection.IsConnected)
            {
                _logger.LogCannotStartBusInstance();
                return;
            }
            // Apply declarations
            if (_declarations.Count > 0)
            {
                await ApplyConfiguratorsAsync(_connection, _declarations, cancellationToken).ConfigureAwait(false);
            }
            // Apply bindings from publisher
            if (_publisher is not null)
            {
                await ApplyConfiguratorsAsync(_connection, _publisher.EndPoints.SelectMany(x => x.Bindings), cancellationToken).ConfigureAwait(false);
            }
            // Apply bindings from consumer and start consuming
            if (_consumerChannel is not null)
            {
                await ApplyConfiguratorsAsync(_connection, _consumerChannel.EndPoints.SelectMany(x => x.Bindings), cancellationToken).ConfigureAwait(false);
                await _consumerChannel.TryOpenAsync(cancellationToken).ConfigureAwait(false);
            }

            _logger.LogBusInstanceStarted();
        }

        public async Task StopAsync(CancellationToken cancellationToken = default)
        {
            if (_consumerChannel != null)
            {
                await _consumerChannel.DisposeAsync().ConfigureAwait(false);
            }
        }

        private static async Task ApplyConfiguratorsAsync(IRabbitMQPersistentConnection connection, IEnumerable<Func<IChannel, CancellationToken, Task>> items, CancellationToken cancellationToken = default)
        {
            var channel = await connection.CreateChannelAsync(cancellationToken).ConfigureAwait(false);
            await using (channel.ConfigureAwait(false))
            {
                foreach (var d in items)
                {
                    await d(channel, cancellationToken).ConfigureAwait(false);
                }
            }
        }
    }

    internal static partial class LoggerExtensions
    {
        [LoggerMessage(Level = LogLevel.Information, Message = "Starting RabbitMQ bus instance.")]
        public static partial void LogStartBusInstance(this ILogger logger);

        [LoggerMessage(Level = LogLevel.Critical, Message = "CRITICAL: Cannot start RabbitMQ bus instance. Unable to establish connection with RabbitMQ host.")]
        public static partial void LogCannotStartBusInstance(this ILogger logger);

        [LoggerMessage(Level = LogLevel.Information, Message = "The RabbitMQ bus instance has started.")]
        public static partial void LogBusInstanceStarted(this ILogger logger);
    }
}
