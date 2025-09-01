using Shared.EventBus;

namespace Tests.RabbitMQ.Events
{
    public record PrimitiveIntegrationEvent<T> : IntegrationEvent
    {
        public T? Value { get; set; }
    }
}
