using RabbitMQ.Client;

namespace Shared.RabbitMQ
{
    public interface IRabbitMQDeclarationCollection : IReadOnlyCollection<Func<IChannel, CancellationToken, Task>>
    {
    }
}
