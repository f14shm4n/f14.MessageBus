namespace Shared.EventBus
{
    public interface IConsumer<TMessage> where TMessage : class
    {
        Task ConsumeAsync(TMessage message, CancellationToken token = default);
    }
}
