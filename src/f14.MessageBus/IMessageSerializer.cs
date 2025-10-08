namespace f14.MessageBus
{
    public interface IMessageSerializer
    {
        ValueTask<byte[]> SerializeAsync(object? message, CancellationToken cancellationToken = default);
        ValueTask<object?> DeserializeAsync(byte[] message, Type messageType, CancellationToken cancellationToken = default);
    }
}
