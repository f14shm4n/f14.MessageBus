namespace Tests.SharedResources.EventBus.Messages
{
    public record PrimitiveMessage<T>
    {
        public T? Value { get; set; }
    }
}
