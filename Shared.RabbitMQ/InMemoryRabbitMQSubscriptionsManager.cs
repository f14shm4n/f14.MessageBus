using Shared.EventBus;

namespace Shared.RabbitMQ
{
    internal class InMemoryRabbitMQSubscriptionsManager : InMemoryEventBusSubscriptionsManager<SubscriptionInfo>, IRabbitMQSubscriptionsManager
    {
        protected override SubscriptionInfo CreateSubscriptionInfo(string eventKey, Type eventHandlerType)
        {
            return new SubscriptionInfo(eventHandlerType);
        }
    }
}
