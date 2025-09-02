using Shared.EventBus;

namespace Tests.SharedResources.Events
{
    public class StringIntegrationEventHandler : IIntegrationEventHandler<StringIntegrationEvent>
    {
        public Task Handle(StringIntegrationEvent @event)
        {
            return Task.CompletedTask;
        }
    }
}
