using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using Shared.EventBus;

namespace Shared.RabbitMQ.Internals
{
    internal sealed class DefaultAsyncEventingBasicConsumerFactory : IAsyncBasicConsumerFactory
    {
        private readonly IRabbitMQErrorResolver _errorResolver;
        private readonly IMessageProcessor _messageProcessor;

        public DefaultAsyncEventingBasicConsumerFactory(IRabbitMQErrorResolver errorResolver, IMessageProcessor messageProcessor)
        {

            _messageProcessor = messageProcessor;
            _errorResolver = errorResolver;
        }

        public IAsyncBasicConsumer CreateAsyncBasicConsumer(IChannel channel)
        {
            return new DefaultAsyncEventingBasicConsumer(_errorResolver, _messageProcessor, channel);
        }
    }
}
