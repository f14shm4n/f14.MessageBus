namespace Shared.EventBus
{
    public interface IEventBusInstance
    {
        Task SendAsync<TMessage>(TMessage message, CancellationToken cancellationToken = default) where TMessage : class;
        Task StartAsync(CancellationToken cancellationToken = default);
        Task StopAsync(CancellationToken cancellationToken = default);
    }
}
