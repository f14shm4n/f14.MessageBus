namespace Shared.RabbitMQ
{
    public interface IRabbitMQBusSender
    {
        Task SendAsync<TMessage>(TMessage message, CancellationToken cancellationToken = default);
    }
}
