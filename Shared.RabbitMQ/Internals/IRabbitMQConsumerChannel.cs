namespace Shared.RabbitMQ
{
    public interface IRabbitMQConsumerChannel : IRabbitMQPersistentChannel
    {
        IRabbitMQEndPointCollection EndPoints { get; }
    }
}
