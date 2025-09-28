using RabbitMQ.Client;

namespace Shared.RabbitMQ
{
    public interface IRabbitMQPublisher
    {
        Task PublishAsync<TMessage>(IReadOnlySet<string> exchanges, TMessage message, BasicProperties basicProperties, CancellationToken cancellationToken = default);
    }
}
