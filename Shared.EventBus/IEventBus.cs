namespace Shared.EventBus
{
    public interface IEventBus
    {
        Task SendAsync<TMessage>(TMessage message, CancellationToken cancellationToken = default) where TMessage : class;
    }
}
