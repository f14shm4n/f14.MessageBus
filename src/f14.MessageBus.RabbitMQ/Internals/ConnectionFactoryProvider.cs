using RabbitMQ.Client;

namespace f14.MessageBus.RabbitMQ.Internals
{
    internal sealed class ConnectionFactoryProvider : IConnectionFactoryProvider
    {
        private IConnectionFactory? _connectionFactory;

        public void SetConnectionFactory(IConnectionFactory connectionFactory) => _connectionFactory = connectionFactory;

        public IConnectionFactory GetConnectionFactory()
        {
            if (_connectionFactory is null)
            {
                ThrowHelper.ConnectionFactoryIsNotConfigured();
            }
            return _connectionFactory;
        }
    }
}
