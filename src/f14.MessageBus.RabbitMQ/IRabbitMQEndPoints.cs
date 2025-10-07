namespace f14.MessageBus.RabbitMQ
{
    public interface IRabbitMQEndPoints
    {
        IRabbitMQEndPoint RegistedEndPoint(string exchange, string queue);
    }
}
