namespace Shared.EventBus
{
    public class SubscriptionInfo : ISubscriptionInfo
    {

        public SubscriptionInfo(Type integrationEventHandlerType) => IntegrationEventHandlerType = integrationEventHandlerType;

        public Type IntegrationEventHandlerType { get; private init; }
    }
}
