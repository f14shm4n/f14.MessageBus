namespace f14.MessageBus.RabbitMQ
{
    public interface IRabbitMQConsumerChannel : IRabbitMQPersistentChannel
    {
        IRabbitMQEndPointCollection EndPoints { get; }
    }
}
