using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using Shared.Commons;
using Shared.EventBus;
using System.Net;

namespace Shared.RabbitMQ.Internals
{
    internal sealed class RabbitMQBusInstance : IEventBusInstance, IRabbitMQBusSender
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
            _logger.LogInformation("Starting RabbitMQ event bus instance.");

            // 0. Open RabbitMQ connection
            await _connection.TryConnectAsync(cancellationToken);
            if (!_connection.IsConnected)
            {
                _logger.LogCritical("CRITICAL: The RabbitMQ event bus instance is not running. Unable to establish connection with RabbitMQ host.");
                return;
            }
            // 1. Apply declarations
            if (_declarations.Count > 0)
            {
                await ApplyConfiguratorsAsync(_connection, _declarations, cancellationToken);
            }
            // 2. Apply bindings from publisher
            if (_publisher is not null)
            {
                await ApplyConfiguratorsAsync(_connection, _publisher.EndPoints.SelectMany(x => x.Bindings), cancellationToken);
            }
            // 2. Apply bindings from consumer and start consuming
            if (_consumerChannel is not null)
            {
                await ApplyConfiguratorsAsync(_connection, _consumerChannel.EndPoints.SelectMany(x => x.Bindings), cancellationToken);
                await _consumerChannel.TryOpenAsync(cancellationToken);
            }
            _logger.LogInformation("The RabbitMQ event bus instance has started.");
        }

        public Task StopAsync(CancellationToken cancellationToken = default)
        {
            // TODO: Dispose all disposable objects here or implement IDisposable
            return Task.CompletedTask;
        }

        private static async Task ApplyConfiguratorsAsync(IRabbitMQPersistentConnection connection, IEnumerable<Func<IChannel, CancellationToken, Task>> items, CancellationToken cancellationToken = default)
        {
            await using (var channel = await connection.CreateChannelAsync(cancellationToken))
            {
                foreach (var d in items)
                {
                    await d(channel, cancellationToken);
                }
            }
        }
    }
}
