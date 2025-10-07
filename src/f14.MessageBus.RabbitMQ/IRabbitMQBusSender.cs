namespace f14.MessageBus.RabbitMQ
{
    public interface IRabbitMQBusSender
    {
        Task SendAsync<TMessage>(TMessage message, CancellationToken cancellationToken = default);
    }
}
