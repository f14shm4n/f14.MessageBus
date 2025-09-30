using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using Shared.EventBus;

namespace Shared.RabbitMQ.Internals
{
    internal sealed class DefaultAsyncEventingBasicConsumerFactory : IAsyncBasicConsumerFactory
    {
        private readonly ILogger<DefaultAsyncEventingBasicConsumer> _logger;
        private readonly IMessageProcessor _messageProcessor;

        public DefaultAsyncEventingBasicConsumerFactory(ILogger<DefaultAsyncEventingBasicConsumer> logger, IMessageProcessor messageProcessor)
        {
            _logger = logger;
            _messageProcessor = messageProcessor;
        }

        public IAsyncBasicConsumer CreateAsyncBasicConsumer(IChannel channel)
        {
            return new DefaultAsyncEventingBasicConsumer(_logger, _messageProcessor, channel);
        }
    }
}
