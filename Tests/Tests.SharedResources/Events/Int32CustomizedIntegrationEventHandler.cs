using Shared.EventBus;

namespace Tests.SharedResources.Events
{
    public class Int32CustomizedIntegrationEventHandler : IIntegrationEventHandler<Int32IntegrationEvent>
    {
        public Task Handle(Int32IntegrationEvent @event)
        {
            return Task.CompletedTask;
        }
    }
}
