using Microsoft.Extensions.Logging;
using Polly;
using RabbitMQ.Client;
using RabbitMQ.Client.Exceptions;
using Shared.EventBus;
using System.Net.Sockets;

namespace Shared.RabbitMQ.Internals
{
    internal sealed class RabbitMQPublisher : IRabbitMQPublisher
    {
        private const int RetryCount = 5;
        private const int RetryDelay = 2000;

        private readonly ILogger<RabbitMQPublisher> _logger;
        private readonly IRabbitMQPersistentConnection _connection;
        private readonly IMessageSerializer _messageSerializer;
        private readonly IRabbitMQEndPointCollection _endPoints;

        private readonly Dictionary<string, HashSet<string>> _publishingMap = [];

        public RabbitMQPublisher(
            ILogger<RabbitMQPublisher> logger,
            IRabbitMQPersistentConnection connection,
            IMessageSerializer messageSerializer,
            IRabbitMQEndPointCollection endPoints)
        {
            _logger = logger;
            _connection = connection;
            _messageSerializer = messageSerializer;
            _endPoints = endPoints;

            _publishingMap = EndPoints.MapRoutingKeyToExchange();
        }

        public IRabbitMQEndPointCollection EndPoints => _endPoints;

        public async Task PublishAsync<TMessage>(TMessage message, BasicProperties basicProperties, CancellationToken cancellationToken = default)
        {
            if (!_connection.IsConnected)
            {
                // TODO: return error code or something
                return;
            }

            var messageTypeName = typeof(TMessage).Name;
            if (!_publishingMap.TryGetValue(messageTypeName, out var exchanges))
            {
                _logger.LogWarning("No exchanges are registered with this routing key. RoutingKey: '{Key}'", messageTypeName);
                return;
            }

            try
            {
                // TODO: I guess it can be published in parallel mode  

                _logger.LogTrace("Creating new RabbitMQ channel for publish.");
                await using (var channel = await _connection.CreateChannelAsync(cancellationToken))
                {
                    _logger.LogTrace("Sending message to RabbitMQ. MessageType: '{Type}'", messageTypeName);

                    var policy = Policy
                        .Handle<SocketException>()
                        .Or<BrokerUnreachableException>()
                        .WaitAndRetryAsync(RetryCount, retryAttempt => TimeSpan.FromMilliseconds(RetryDelay * retryAttempt), (ex, time) =>
                        {
                            _logger.LogWarning(ex, "Failed to publish message. MessageType: '{Type}', after {Time} sec. ({Error})", messageTypeName, $"{time.TotalSeconds:n0}", ex.Message);
                        });

                    var body = _messageSerializer.Serialize(message);
                    foreach (var exchange in exchanges)
                    {
                        await policy.ExecuteAsync(async ct =>
                        {
                            await channel.BasicPublishAsync(
                                exchange: exchange,
                                routingKey: messageTypeName,
                                mandatory: true,
                                basicProperties: basicProperties,
                                body: body,
                                cancellationToken: ct);

                        }, cancellationToken: cancellationToken);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "FATAL: Failed to send RabbitMQ message. MessageType: '{Type}'", messageTypeName);
            }
        }
    }
}
