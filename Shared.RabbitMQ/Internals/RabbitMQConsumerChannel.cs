using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Shared.EventBus;

namespace Shared.RabbitMQ
{
    internal sealed class RabbitMQConsumerChannel : RabbitMQPersistentChannel, IRabbitMQConsumerChannel
    {
        private readonly IMessageProcessor _eventProcessor;
        private readonly IRabbitMQEndPointCollection _endPoints;

        public RabbitMQConsumerChannel(
            ILogger<RabbitMQConsumerChannel> logger,
            IRabbitMQPersistentConnection connection,
            IMessageProcessor eventProcessor,
            IRabbitMQEndPointCollection endPoints)
            : base(logger, connection)
        {
            _eventProcessor = eventProcessor;
            _endPoints = endPoints;
        }

        public IRabbitMQEndPointCollection EndPoints => _endPoints;

        public override async ValueTask<bool> TryOpenAsync(CancellationToken cancellationToken = default)
        {
            if (_endPoints.Count == 0)
            {
                return false;
            }
            return await base.TryOpenAsync(cancellationToken);
        }

        protected override async Task OnChannelCreatedAsync(IChannel channel, CancellationToken cancellationToken = default)
        {
            Logger.LogTrace("Starting RabbitMQ consume.");

            // TODO: Need to create separate consumer class
            var consumer = new AsyncEventingBasicConsumer(channel);
            consumer.ReceivedAsync += async (s, a) =>
            {
                try
                {
                    await _eventProcessor.ProcessMessageAsync(a.RoutingKey, a.Body);
                }
                catch (Exception ex)
                {
                    Logger.LogWarning(ex, "Cannot process message. MessageType(routingKey): '{Type}'", a.RoutingKey);
                }

                // TODO: Handle message if error has been occurred (Dead message exchange).

                await channel.BasicAckAsync(a.DeliveryTag, multiple: false);
            };

            foreach (var name in _endPoints.Select(x => x.Queue).ToHashSet())
            {
                await channel.BasicConsumeAsync(
                    queue: name,
                    autoAck: false,
                    consumer: consumer,
                    cancellationToken: cancellationToken);
            }
        }
    }
}
