namespace Shared.EventBus
{
    public interface IConsumer<TMessage>
    {
        Task ConsumeAsync(TMessage message, CancellationToken token = default);
    }
}
