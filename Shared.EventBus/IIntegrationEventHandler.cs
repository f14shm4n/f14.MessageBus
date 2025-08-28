namespace Shared.EventBus
{
    public interface IIntegrationEventHandler<in E> where E : IntegrationEvent
    {
        Task Handle(E @event);
    }
}