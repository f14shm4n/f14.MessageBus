using Shared.EventBus;

namespace Tests.RabbitMQ.Events
{
    public class Int32IntegrationEventHandler : IIntegrationEventHandler<Int32IntegrationEvent>
    {
        public Task Handle(Int32IntegrationEvent @event)
        {
            return Task.CompletedTask;
        }
    }
    public class Int32CustomizedIntegrationEventHandler : IIntegrationEventHandler<Int32IntegrationEvent>
    {
        public Task Handle(Int32IntegrationEvent @event)
        {
            return Task.CompletedTask;
        }
    }
}
