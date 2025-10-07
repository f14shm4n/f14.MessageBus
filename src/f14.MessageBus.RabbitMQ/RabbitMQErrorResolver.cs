namespace f14.MessageBus.RabbitMQ
{
    public class RabbitMQErrorResolver : IRabbitMQErrorResolver
    {
        public virtual Task<ConsumerResolveAction> ResolveProcessingErrorAsync(string routingKey, ReadOnlyMemory<byte> body, Exception exception, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(ConsumerResolveAction.Ack);
        }
    }
}
