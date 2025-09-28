namespace Shared.RabbitMQ
{
    public static class RabbitMQConstants
    {
        public const string DefaultUri = "amqp://guest:guest@localhost:5672/";

        public static class ExchangeTypes
        {
            public const string Fanout = "fanout";
            public const string Direct = "direct";
            public const string Topic = "topic";
        }
    }
}
