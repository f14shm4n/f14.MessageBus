using RabbitMQ.Client;

namespace f14.MessageBus.RabbitMQ
{
    public interface IBasicPropertiesProvider
    {
        BasicProperties GetBasicProperties();
    }
}
