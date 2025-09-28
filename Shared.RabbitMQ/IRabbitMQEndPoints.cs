namespace Shared.RabbitMQ
{
    public interface IRabbitMQEndPoints
    {
        IRabbitMQEndPoint RegistedEndPoint(string exchange, string queue);
    }
}
