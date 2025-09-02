namespace Shared.EventBus
{
    public class SubscriptionInfo : ISubscriptionInfo
    {

        public SubscriptionInfo(Type eventType, Type integrationEventHandlerType)
        {
            EventType = eventType;
            EventHandlerType = integrationEventHandlerType;
        }

        public Type EventType { get; private init; }
        public Type EventHandlerType { get; private init; }
    }
}
