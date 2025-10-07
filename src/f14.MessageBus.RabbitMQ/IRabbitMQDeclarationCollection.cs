using RabbitMQ.Client;

namespace f14.MessageBus.RabbitMQ
{
    public interface IRabbitMQDeclarationCollection : IReadOnlyCollection<Func<IChannel, CancellationToken, Task>>
    {
    }
}
