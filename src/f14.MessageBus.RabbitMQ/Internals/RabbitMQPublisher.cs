using Microsoft.Extensions.Logging;
using Polly;
using RabbitMQ.Client;
using RabbitMQ.Client.Exceptions;
using System.Net.Sockets;

namespace f14.MessageBus.RabbitMQ.Internals
{
    internal sealed class RabbitMQPublisher : IRabbitMQPublisher
    {
        private const int RetryCount = 5;
        private const int RetryDelay = 2000;

        private readonly IRabbitMQPersistentConnection _connection;
        private readonly IMessageSerializer _messageSerializer;
        private readonly ILogger<RabbitMQPublisher> _logger;
        private readonly IRabbitMQEndPointCollection _endPoints;

        private readonly Dictionary<string, HashSet<string>> _publishingMap = [];

        public RabbitMQPublisher(
            IRabbitMQPersistentConnection connection,
            IMessageSerializer messageSerializer,
            ILogger<RabbitMQPublisher> logger,
            IRabbitMQEndPointCollection endPoints)
        {
            _connection = connection;
            _messageSerializer = messageSerializer;
            _logger = logger;
            _endPoints = endPoints;

            _publishingMap = EndPoints.MapRoutingKeyToExchange();
        }

        public IRabbitMQEndPointCollection EndPoints => _endPoints;

        public async Task PublishAsync<TMessage>(TMessage message, BasicProperties basicProperties, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(message);


            if (!_connection.IsConnected)
            {
                ThrowHelper.NoConnection();
            }

            var messageTypeName = typeof(TMessage).Name;
            if (!_publishingMap.TryGetValue(messageTypeName, out var exchanges))
            {
                ThrowHelper.NoExchangesRegisteredForRoutingKey(messageTypeName);
            }

            // TODO: replace logger by metrics
            // TODO: I guess it can be published in parallel mode  

            var channel = await _connection.CreateChannelAsync(cancellationToken).ConfigureAwait(false);
            await using (channel.ConfigureAwait(false))
            {
                //_logger.LogTrace("Sending message to RabbitMQ. MessageType: '{Type}'", messageTypeName);

                var policy = Policy
                    .Handle<SocketException>()
                    .Or<BrokerUnreachableException>()
                    .WaitAndRetryAsync(RetryCount, retryAttempt => TimeSpan.FromMilliseconds(RetryDelay * retryAttempt), (ex, time) =>
                    {
                        _logger.LogRetryFailedToPublishMessage(messageTypeName, time.TotalSeconds, ex.Message);
                    });

                var body = await _messageSerializer.SerializeAsync(message, cancellationToken).ConfigureAwait(false);
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
                            cancellationToken: ct).ConfigureAwait(false);

                    }, cancellationToken: cancellationToken).ConfigureAwait(false);
                }
            }
        }
    }

    internal static partial class LoggerExtensions
    {
        [LoggerMessage(Level = LogLevel.Warning, Message = "Failed to publish message. MessageType: '{MessageTypeName}', after {Time:n0} sec. Error: '{errorMessage}.")]
        public static partial void LogRetryFailedToPublishMessage(this ILogger logger, string messageTypeName, double time, string errorMessage);
    }
}
