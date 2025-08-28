using RabbitMQ.Client;

namespace Shared.RabbitMQ
{
    internal interface IRabbitMQPersistentConnection : IAsyncDisposable
    {
        bool IsConnected { get; }
        ValueTask<bool> TryConnectAsync();
        Task<IChannel> CreateChannelAsync();
    }
}
