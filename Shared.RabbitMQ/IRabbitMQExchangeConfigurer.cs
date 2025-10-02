namespace Shared.RabbitMQ
{
    public interface IRabbitMQExchangeConfigurer
    {
        IRabbitMQQueueConfigurer Exchange(string exchange, ExchangeType type, bool durable = false, bool autoDelete = false, IDictionary<string, object?>? arguments = null, bool passive = false, bool noWait = false);
    }
}
