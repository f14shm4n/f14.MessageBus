using RabbitMQ.Client;

namespace Shared.RabbitMQ
{
    public interface IConnectionFactoryProvider
    {
        IConnectionFactory GetConnectionFactory();
    }
}
