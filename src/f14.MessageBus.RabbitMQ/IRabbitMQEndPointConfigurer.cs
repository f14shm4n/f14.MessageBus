namespace f14.MessageBus.RabbitMQ
{
    public interface IRabbitMQEndPointConfigurer
    {
        void EndPoint(Action<IRabbitMQEndPoint> configure);
    }
}
