namespace Shared.RabbitMQ
{
    public interface IRabbitMQQueueConfigurer
    {
        IRabbitMQEndPointConfigurer Queue(string queue, bool durable = false, bool exclusive = true, bool autoDelete = true, IDictionary<string, object?>? arguments = null, bool noWait = false);
    }
}
