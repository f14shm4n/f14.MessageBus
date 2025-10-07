using RabbitMQ.Client;

namespace f14.MessageBus.RabbitMQ
{
    public interface IRabbitMQPublisher
    {
        IRabbitMQEndPointCollection EndPoints { get; }

        Task PublishAsync<TMessage>(TMessage message, BasicProperties basicProperties, CancellationToken cancellationToken = default);
    }
}
