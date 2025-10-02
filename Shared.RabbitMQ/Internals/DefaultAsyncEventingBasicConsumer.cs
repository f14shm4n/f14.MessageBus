using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Shared.EventBus;

namespace Shared.RabbitMQ.Internals
{
    internal sealed class DefaultAsyncEventingBasicConsumer : AsyncEventingBasicConsumer
    {
        private readonly IRabbitMQErrorResolver _errorResolver;
        private readonly IMessageProcessor _messageProcessor;

        public DefaultAsyncEventingBasicConsumer(IRabbitMQErrorResolver errorResolver, IMessageProcessor messageProcessor, IChannel channel) : base(channel)
        {
            _errorResolver = errorResolver;
            _messageProcessor = messageProcessor;

            ReceivedAsync += DefaultAsyncEventingBasicConsumer_ReceivedAsync;
        }

        private async Task DefaultAsyncEventingBasicConsumer_ReceivedAsync(object sender, BasicDeliverEventArgs @event)
        {
            ConsumerResolveAction action = ConsumerResolveAction.Ack;
            try
            {
                await _messageProcessor.ProcessMessageAsync(@event.RoutingKey, @event.Body, @event.CancellationToken);
            }
            catch (Exception ex)
            {
                action = await _errorResolver.ResolveProcessingErrorAsync(@event.RoutingKey, @event.Body, ex, @event.CancellationToken);
            }

            switch (action)
            {
                case ConsumerResolveAction.Ack:
                    await Channel.BasicAckAsync(@event.DeliveryTag, multiple: false, @event.CancellationToken);
                    break;
                default:
                    await Channel.BasicNackAsync(@event.DeliveryTag, multiple: false, requeue: false, @event.CancellationToken);
                    break;
            }
        }
    }
}
