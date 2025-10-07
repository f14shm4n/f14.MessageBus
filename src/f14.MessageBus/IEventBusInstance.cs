namespace f14.MessageBus
{
    public interface IEventBusInstance
    {
        Task StartAsync(CancellationToken cancellationToken = default);
        Task StopAsync(CancellationToken cancellationToken = default);
    }
}
