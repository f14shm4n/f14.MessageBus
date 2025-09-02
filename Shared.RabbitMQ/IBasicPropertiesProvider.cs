using RabbitMQ.Client;

namespace Shared.RabbitMQ
{
    public interface IBasicPropertiesProvider
    {
        BasicProperties GetBasicProperties();
    }
}
