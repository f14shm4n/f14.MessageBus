namespace f14.MessageBus.RabbitMQ
{
    public interface IRabbitMQPersistentChannel : IAsyncDisposable, IDisposable
    {
        bool IsOpen { get; }
        ValueTask<bool> TryOpenAsync(CancellationToken cancellationToken = default);
    }
}
