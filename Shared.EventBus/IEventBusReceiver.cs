namespace Shared.EventBus
{
    public interface IEventBusReceiver
    {
        Task SubscribeAsync<E, H>()
            where E : IntegrationEvent
            where H : IIntegrationEventHandler<E>;

        Task UnsubscribeAsync<E, H>()
            where E : IntegrationEvent
            where H : IIntegrationEventHandler<E>;
    }
}
