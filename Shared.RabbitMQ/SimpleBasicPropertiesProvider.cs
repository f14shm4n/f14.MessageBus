using RabbitMQ.Client;

namespace Shared.RabbitMQ
{
    public sealed class SimpleBasicPropertiesProvider : IBasicPropertiesProvider
    {
        private readonly BasicProperties _basicProperties;

        public SimpleBasicPropertiesProvider(BasicProperties basicProperties)
        {
            _basicProperties = basicProperties;
        }

        public BasicProperties GetBasicProperties() => _basicProperties;
    }
}
