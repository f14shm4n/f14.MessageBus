using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Shared.EventBus;

namespace Shared.RabbitMQ.Internals
{
    internal sealed class DefaultAsyncEventingBasicConsumer : AsyncEventingBasicConsumer
    {
        private readonly ILogger<DefaultAsyncEventingBasicConsumer> _logger;
        private readonly IMessageProcessor _messageProcessor;

        public DefaultAsyncEventingBasicConsumer(ILogger<DefaultAsyncEventingBasicConsumer> logger, IMessageProcessor messageProcessor, IChannel channel) : base(channel)
        {
            _logger = logger;
            _messageProcessor = messageProcessor;

            ReceivedAsync += DefaultAsyncEventingBasicConsumer_ReceivedAsync;
        }

        private async Task DefaultAsyncEventingBasicConsumer_ReceivedAsync(object sender, BasicDeliverEventArgs @event)
        {
            try
            {
                await _messageProcessor.ProcessMessageAsync(@event.RoutingKey, @event.Body, @event.CancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Cannot process message. RoutingKey: '{Key}'", @event.RoutingKey);
            }
            // TODO: Handle message if error has been occurred (Dead message exchange).
            await Channel.BasicAckAsync(@event.DeliveryTag, multiple: false);
        }
    }
}
