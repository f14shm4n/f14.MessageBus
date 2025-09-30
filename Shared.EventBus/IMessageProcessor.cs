namespace Shared.EventBus
{
    public interface IMessageProcessor
    {
        Task ProcessMessageAsync(string messageTypeName, ReadOnlyMemory<byte> messageBody, CancellationToken cancellationToken = default);
    }
}
