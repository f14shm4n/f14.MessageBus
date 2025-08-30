using RabbitMQ.Client;

namespace Shared.RabbitMQ
{
    public interface IRabbitMQPersistentConnection : IAsyncDisposable, IDisposable
    {
        bool IsConnected { get; }
        ValueTask<bool> TryConnectAsync();
        Task<IChannel> CreateChannelAsync();
    }
}
