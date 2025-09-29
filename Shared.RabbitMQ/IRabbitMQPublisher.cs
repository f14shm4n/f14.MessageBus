using RabbitMQ.Client;

namespace Shared.RabbitMQ
{
    public interface IRabbitMQPublisher
    {
        IRabbitMQEndPointCollection EndPoints { get; }

        Task PublishAsync<TMessage>(TMessage message, BasicProperties basicProperties, CancellationToken cancellationToken = default);
    }
}
