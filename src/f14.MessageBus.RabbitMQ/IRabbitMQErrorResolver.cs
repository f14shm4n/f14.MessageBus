namespace f14.MessageBus.RabbitMQ
{
    public interface IRabbitMQErrorResolver
    {
        Task<ConsumerResolveAction> ResolveProcessingErrorAsync(string routingKey, ReadOnlyMemory<byte> body, Exception exception, CancellationToken cancellationToken = default);
    }
}
