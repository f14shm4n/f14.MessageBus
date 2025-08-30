using Shared.EventBus;

namespace Shared.RabbitMQ
{
    /// <summary>
    /// Provides the default implementation of the <see cref="InMemoryEventBusSubscriptionsManager{T}"/> and <see cref="IRabbitMQSubscriptionsManager{T}"/>
    /// with <see cref="ISubscriptionInfo"/> as <see cref="SubscriptionInfo"/>.
    /// </summary>
    public sealed class DefaultInMemoryRabbitMQSubscriptionsManager : InMemoryEventBusSubscriptionsManager<SubscriptionInfo>, IRabbitMQSubscriptionsManager<SubscriptionInfo>
    {
        protected override SubscriptionInfo CreateSubscriptionInfo(Type eventType, Type eventHandlerType)
        {
            return new SubscriptionInfo(eventHandlerType);
        }
    }
}
