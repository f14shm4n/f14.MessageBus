using RabbitMQ.Client;

namespace Shared.RabbitMQ
{
    public interface IAsyncBasicConsumerFactory
    {
        IAsyncBasicConsumer CreateAsyncBasicConsumer(IChannel channel);
    }
}
