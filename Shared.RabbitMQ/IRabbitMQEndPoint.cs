using RabbitMQ.Client;

namespace Shared.RabbitMQ
{
    public interface IRabbitMQEndPoint
    {
        string Queue { get; }
        string Exchange { get; }
        IReadOnlyCollection<Func<IChannel, CancellationToken, Task>> Bindings { get; }
        IReadOnlySet<string> RoutingKeys { get; }
        void Consume<TMessage>();
    }
}
