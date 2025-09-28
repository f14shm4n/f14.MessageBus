using RabbitMQ.Client;

namespace Shared.RabbitMQ.Internals
{
    internal sealed class ConnectionFactoryProvider : IConnectionFactoryProvider
    {
        private IConnectionFactory? _connectionFactory;

        public void SetConnectionFactory(IConnectionFactory connectionFactory) => _connectionFactory = connectionFactory;

        public IConnectionFactory GetConnectionFactory() => _connectionFactory ?? throw new InvalidOperationException("The RabbitMQ connection factory is not configured.");
    }
}
