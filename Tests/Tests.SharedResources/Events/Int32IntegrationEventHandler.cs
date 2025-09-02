using Shared.EventBus;

namespace Tests.SharedResources.Events
{
    public class Int32IntegrationEventHandler : IIntegrationEventHandler<Int32IntegrationEvent>
    {
        public Task Handle(Int32IntegrationEvent @event)
        {
            return Task.CompletedTask;
        }
    }
}
