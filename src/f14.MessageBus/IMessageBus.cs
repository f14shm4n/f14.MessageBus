namespace f14.MessageBus
{
    public interface IMessageBus
    {
        Task SendAsync<TMessage>(TMessage message, CancellationToken cancellationToken = default) where TMessage : class;
    }
}
