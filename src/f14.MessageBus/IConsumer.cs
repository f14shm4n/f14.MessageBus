namespace f14.MessageBus
{
    public interface IConsumer<TMessage>
    {
        Task ConsumeAsync(TMessage message, CancellationToken token = default);
    }
}
