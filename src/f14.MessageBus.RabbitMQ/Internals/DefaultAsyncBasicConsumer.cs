using RabbitMQ.Client;
using Shared.EventBus;

namespace f14.MessageBus.RabbitMQ.Internals
{
    internal sealed class DefaultAsyncBasicConsumer : AsyncDefaultBasicConsumer
    {
        private readonly IRabbitMQErrorResolver _errorResolver;
        private readonly IMessageProcessor _messageProcessor;

        public DefaultAsyncBasicConsumer(IRabbitMQErrorResolver errorResolver, IMessageProcessor messageProcessor, IChannel channel) : base(channel)
        {
            _errorResolver = errorResolver;
            _messageProcessor = messageProcessor;
        }

        public override async Task HandleBasicDeliverAsync(string consumerTag, ulong deliveryTag, bool redelivered, string exchange, string routingKey, IReadOnlyBasicProperties properties, ReadOnlyMemory<byte> body, CancellationToken cancellationToken = default)
        {
            ConsumerResolveAction action = ConsumerResolveAction.Ack;
            try
            {
                await _messageProcessor.ProcessMessageAsync(routingKey, body, cancellationToken);
            }
            catch (Exception ex)
            {
                action = await _errorResolver.ResolveProcessingErrorAsync(routingKey, body, ex, cancellationToken);
            }

            switch (action)
            {
                case ConsumerResolveAction.Ack:
                    await Channel.BasicAckAsync(deliveryTag, multiple: false, cancellationToken);
                    break;
                default:
                    await Channel.BasicNackAsync(deliveryTag, multiple: false, requeue: false, cancellationToken);
                    break;
            }
        }
    }
}
