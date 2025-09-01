using Shared.EventBus;

namespace Tests.RabbitMQ.Events
{
    public class StringIntegrationEventHandler : IIntegrationEventHandler<StringIntegrationEvent>
    {
        public Task Handle(StringIntegrationEvent @event)
        {
            return Task.CompletedTask;
        }
    }
}
