namespace Shared.EventBus
{
    public interface IEventBusPublisher
    {
        Task PublishAsync(IntegrationEvent @event);
    }
}
