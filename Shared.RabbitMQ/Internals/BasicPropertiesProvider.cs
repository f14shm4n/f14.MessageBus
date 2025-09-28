using RabbitMQ.Client;

namespace Shared.RabbitMQ.Internals
{
    internal sealed class BasicPropertiesProvider : IBasicPropertiesProvider
    {
        private readonly BasicProperties _basicProperties = new() { DeliveryMode = DeliveryModes.Persistent };

        public BasicProperties GetBasicProperties() => _basicProperties;
    }
}
