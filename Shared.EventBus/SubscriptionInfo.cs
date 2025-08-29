namespace Shared.EventBus
{
    public class SubscriptionInfo : ISubscriptionInfo
    {

        public SubscriptionInfo(Type integrationEventHandlerType) => EventHandlerType = integrationEventHandlerType;

        public Type EventHandlerType { get; private init; }
    }
}
