namespace Shared.RabbitMQ
{
    public class RabbitMQErrorResolver : IRabbitMQErrorResolver
    {
        public virtual Task<ConsumerResolveAction> ResolveProcessingErrorAsync(string routingKey, ReadOnlyMemory<byte> body, Exception error, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(ConsumerResolveAction.Ack);
        }
    }
}
