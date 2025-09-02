using Shared.EventBus;

namespace Tests.SharedResources.Events
{
    public record PrimitiveIntegrationEvent<T> : IntegrationEvent
    {
        public T? Value { get; set; }
    }
}
