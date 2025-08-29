using Shared.EventBus;

namespace Shared.RabbitMQ
{
    public interface IRabbitMQEventBus : IEventBusPublisher, IEventBusReceiver
    {

    }
}
