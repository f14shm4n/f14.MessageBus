using RabbitMQ.Client;

namespace f14.MessageBus.RabbitMQ
{
    public interface IAsyncBasicConsumerFactory
    {
        IAsyncBasicConsumer CreateAsyncBasicConsumer(IChannel channel);
    }
}
