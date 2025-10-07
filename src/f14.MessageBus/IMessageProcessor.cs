namespace f14.MessageBus
{
    public interface IMessageProcessor
    {
        Task ProcessMessageAsync(string messageTypeName, ReadOnlyMemory<byte> messageBody, CancellationToken cancellationToken = default);
    }
}
