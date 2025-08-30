using Shared.EventBus;

namespace Shared.RabbitMQ
{
    public interface IRabbitMQSubscriptionsManager<T> : IEventBusSubscriptionsManager<T> where T : ISubscriptionInfo
    {
    }
}
