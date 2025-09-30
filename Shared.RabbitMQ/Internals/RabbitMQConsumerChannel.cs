using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using Shared.EventBus;

namespace Shared.RabbitMQ
{
    internal sealed class RabbitMQConsumerChannel : RabbitMQPersistentChannel, IRabbitMQConsumerChannel
    {
        private readonly IAsyncBasicConsumerFactory _asyncBasicConsumerFactory;
        private readonly IRabbitMQEndPointCollection _endPoints;

        public RabbitMQConsumerChannel(
            ILogger<RabbitMQConsumerChannel> logger,
            IRabbitMQPersistentConnection connection,
            IAsyncBasicConsumerFactory asyncBasicConsumerFactory,
            IRabbitMQEndPointCollection endPoints)
            : base(logger, connection)
        {
            _asyncBasicConsumerFactory = asyncBasicConsumerFactory;
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

            var consumer = _asyncBasicConsumerFactory.CreateAsyncBasicConsumer(channel);
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
