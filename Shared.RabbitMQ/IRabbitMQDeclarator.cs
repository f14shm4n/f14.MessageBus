namespace Shared.RabbitMQ
{
    public interface IRabbitMQDeclarator
    {
        void ExchangeDeclare(string exchange, string type, bool durable = false, bool autoDelete = false, IDictionary<string, object?>? arguments = null, bool passive = false, bool noWait = false);
        void QueueDeclare(string queue, bool durable = false, bool exclusive = true, bool autoDelete = true, IDictionary<string, object?>? arguments = null, bool noWait = false);
    }
}
