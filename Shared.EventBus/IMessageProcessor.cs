namespace Shared.EventBus
{
    public interface IMessageProcessor
    {
        Task ProcessMessageAsync(string messageKey, ReadOnlyMemory<byte> messageBody, CancellationToken cancellationToken = default);
    }
}
