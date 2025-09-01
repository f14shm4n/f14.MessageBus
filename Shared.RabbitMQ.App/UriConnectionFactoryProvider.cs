using RabbitMQ.Client;

namespace Shared.RabbitMQ.App
{
    internal sealed class UriConnectionFactoryProvider : IConnectionFactoryProvider
    {
        private readonly IConnectionFactory _factory;

        public UriConnectionFactoryProvider(RabbitMQOptions options)
        {
            _factory = new ConnectionFactory
            {
                Uri = new Uri(options.ConnectionString)
            };
        }

        public IConnectionFactory GetConnectionFactory() => _factory;
    }
}
