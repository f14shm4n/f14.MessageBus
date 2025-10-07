using RabbitMQ.Client;

namespace f14.MessageBus.RabbitMQ.Internals
{
    internal sealed class DefaultAsyncBasicConsumerFactory : IAsyncBasicConsumerFactory
    {
        private readonly IRabbitMQErrorResolver _errorResolver;
        private readonly IMessageProcessor _messageProcessor;

        public DefaultAsyncBasicConsumerFactory(IRabbitMQErrorResolver errorResolver, IMessageProcessor messageProcessor)
        {

            _messageProcessor = messageProcessor;
            _errorResolver = errorResolver;
        }

        public IAsyncBasicConsumer CreateAsyncBasicConsumer(IChannel channel)
        {
            return new DefaultAsyncBasicConsumer(_errorResolver, _messageProcessor, channel);
        }
    }
}
