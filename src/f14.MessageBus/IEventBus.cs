namespace f14.MessageBus
{
    public interface IEventBus
    {
        Task SendAsync<TMessage>(TMessage message, CancellationToken cancellationToken = default) where TMessage : class;
    }
}
