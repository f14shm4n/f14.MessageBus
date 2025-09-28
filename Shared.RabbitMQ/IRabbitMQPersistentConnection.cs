using RabbitMQ.Client;

namespace Shared.RabbitMQ
{
    public interface IRabbitMQPersistentConnection : IAsyncDisposable, IDisposable
    {
        bool IsConnected { get; }
        ValueTask<bool> TryConnectAsync(CancellationToken cancellationToken = default);
        Task<IChannel> CreateChannelAsync(CancellationToken cancellationToken = default);
    }
}
