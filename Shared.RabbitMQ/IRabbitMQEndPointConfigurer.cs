namespace Shared.RabbitMQ
{
    public interface IRabbitMQEndPointConfigurer
    {
        void EndPoint(Action<IRabbitMQEndPoint> configure);
    }
}
