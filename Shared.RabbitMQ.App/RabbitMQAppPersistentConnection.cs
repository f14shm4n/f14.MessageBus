using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace Shared.RabbitMQ.App
{
    public sealed class RabbitMQAppPersistentConnection : RabbitMQPersistentConnection
    {
        public RabbitMQAppPersistentConnection(ILogger<RabbitMQPersistentConnection> logger, RabbitMQAppOptions options) : base(logger, options)
        {
        }

        protected override void ConfigureConnectionFactory(ConnectionFactory connectionFactory, RabbitMQOptions options)
        {
            connectionFactory.HostName = options.ConnectionString;
        }
    }
}
