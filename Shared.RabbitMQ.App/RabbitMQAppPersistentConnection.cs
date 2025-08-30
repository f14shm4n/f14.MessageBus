using Microsoft.Extensions.Logging;

namespace Shared.RabbitMQ.App
{
    public sealed class RabbitMQAppPersistentConnection : RabbitMQPersistentConnection
    {
        public RabbitMQAppPersistentConnection(ILogger<RabbitMQPersistentConnection> logger, RabbitMQAppOptions options) : base(logger, options)
        {
        }
    }
}
