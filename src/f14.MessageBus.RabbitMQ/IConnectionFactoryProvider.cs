using RabbitMQ.Client;

namespace f14.MessageBus.RabbitMQ
{
    public interface IConnectionFactoryProvider
    {
        IConnectionFactory GetConnectionFactory();
    }
}
