using RabbitMQ.Client;

namespace f14.MessageBus.RabbitMQ.Internals
{
    internal sealed class BasicPropertiesProvider : IBasicPropertiesProvider
    {
        private readonly BasicProperties _basicProperties = new() { DeliveryMode = DeliveryModes.Persistent };

        public BasicProperties GetBasicProperties() => _basicProperties;
    }
}
