using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using Shared.EventBus;

namespace Shared.RabbitMQ.Internals
{
    internal sealed class RabbitMQBusInstance : IEventBusInstance
    {
        private readonly ILogger<RabbitMQBusInstance> _logger;
        private readonly IRabbitMQPersistentConnection _connection;
        private readonly IRabbitMQPublisher _publisher;
        private readonly IBasicPropertiesProvider _basicPropertiesProvider;
        private readonly IRabbitMQPersistentChannel _consumerChannel;
        private readonly IRabbitMQDeclarationCollection _declarations;
        private readonly IRabbitMQEndPointCollection _endPoints;

        public RabbitMQBusInstance(
            ILogger<RabbitMQBusInstance> logger,
            IRabbitMQPersistentConnection connection,
            IRabbitMQPublisher publisher,
            IBasicPropertiesProvider basicPropertiesProvider,
            IRabbitMQPersistentChannel consumerChannel,
            IRabbitMQDeclarationCollection declarations,
            IRabbitMQEndPointCollection endPoints)
        {
            _logger = logger;
            _connection = connection;
            _publisher = publisher;
            _basicPropertiesProvider = basicPropertiesProvider;
            _consumerChannel = consumerChannel;
            _declarations = declarations;
            _endPoints = endPoints;
        }

        public async Task SendAsync<TMessage>(TMessage message, CancellationToken cancellationToken = default) where TMessage : class
        {
            // TODO: Can be cached
            var exchanges = _endPoints
                .Where(x => x.RoutingKeys.Contains(message.GetType().Name))
                .Select(x => x.Exchange)
                .ToHashSet();

            await _publisher.PublishAsync(exchanges, message, _basicPropertiesProvider.GetBasicProperties(), cancellationToken);
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
            // 1. Prepare and execute declarations and bindings 
            // 1.1 Prepare list of declarations and bindings
            List<Func<IChannel, CancellationToken, Task>>? declarationAndBindings = [];
            if (_declarations is not null)
            {
                declarationAndBindings.AddRange(_declarations);
            }
            if (_endPoints?.Count > 0)
            {
                declarationAndBindings.AddRange(_endPoints.SelectMany(x => x.Bindings));
            }
            // 1.2 Execute declarations and bindings
            await using (var channel = await _connection.CreateChannelAsync(cancellationToken))
            {
                foreach (var d in declarationAndBindings)
                {
                    await d(channel, cancellationToken);
                }
            }
            // 2. Open consumer channel and start basic consuming
            await _consumerChannel.TryOpenAsync(cancellationToken);

            _logger.LogInformation("The RabbitMQ event bus instance has started.");
        }

        public Task StopAsync(CancellationToken cancellationToken = default)
        {
            // TODO: Dispose all disposable objects here or implement IDisposable
            return Task.CompletedTask;
        }
    }
}
