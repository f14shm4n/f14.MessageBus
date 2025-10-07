using RabbitMQ.Client;

namespace f14.MessageBus.RabbitMQ
{
    public interface IRabbitMQEndPoint
    {
        string Queue { get; }
        string Exchange { get; }
        IReadOnlyCollection<Func<IChannel, CancellationToken, Task>> Bindings { get; }
        IReadOnlySet<string> RoutingKeys { get; }
        void Message<TMessage>();
    }
}
