using System.Diagnostics.CodeAnalysis;

namespace Shared.RabbitMQ
{
    public sealed class RabbitMQConfigurationException : Exception
    {
        public RabbitMQConfigurationException(string message)
            : base(message)
        {
        }

        public static RabbitMQConfigurationException PublisherAndConsumerIsNull()
        {
            throw new RabbitMQConfigurationException($"The RabbitMQ configuration does not have a publisher or consumer configured.");
        }
    }
}
