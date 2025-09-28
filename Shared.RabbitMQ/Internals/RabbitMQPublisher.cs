using Microsoft.Extensions.Logging;
using Polly;
using RabbitMQ.Client;
using RabbitMQ.Client.Exceptions;
using System.Net.Sockets;
using System.Text.Json;

namespace Shared.RabbitMQ.Internals
{
    internal sealed class RabbitMQPublisher : IRabbitMQPublisher
    {
        private const int RetryCount = 5;
        private const int RetryDelay = 2000;

        private readonly ILogger<RabbitMQPublisher> _logger;
        private readonly IRabbitMQPersistentConnection _connection;

        public RabbitMQPublisher(ILogger<RabbitMQPublisher> logger, IRabbitMQPersistentConnection connection)
        {
            _logger = logger;
            _connection = connection;
        }

        public async Task PublishAsync<TMessage>(IReadOnlySet<string> exchanges, TMessage message, BasicProperties basicProperties, CancellationToken cancellationToken = default)
        {
            if (!_connection.IsConnected)
            {
                // TODO: return error code or something
                return;
            }

            var messageTypeName = typeof(TMessage).Name;
            try
            {
                _logger.LogTrace("Creating new RabbitMQ channel for publish.");
                await using (var channel = await _connection.CreateChannelAsync(cancellationToken))
                {
                    var body = JsonSerializer.SerializeToUtf8Bytes(message);

                    _logger.LogTrace("Sending message to RabbitMQ. MessageType: '{Type}'", messageTypeName);

                    var policy = Policy
                        .Handle<SocketException>()
                        .Or<BrokerUnreachableException>()
                        .WaitAndRetryAsync(RetryCount, retryAttempt => TimeSpan.FromMilliseconds(RetryDelay * retryAttempt), (ex, time) =>
                        {
                            _logger.LogWarning(ex, "Failed to publish message. MessageType: '{Type}', after {Time} sec. ({Error})", messageTypeName, $"{time.TotalSeconds:n0}", ex.Message);
                        });
                    // TODO: I guess it can be published in parallel mode
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
