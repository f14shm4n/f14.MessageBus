namespace Shared.EventBus
{
    public interface IEventBusInstance
    {
        Task StartAsync(CancellationToken cancellationToken = default);
        Task StopAsync(CancellationToken cancellationToken = default);
    }
}
