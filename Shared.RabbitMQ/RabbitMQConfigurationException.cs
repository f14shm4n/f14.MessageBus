using System.Diagnostics.CodeAnalysis;

namespace Shared.RabbitMQ
{
    public sealed class RabbitMQConfigurationException : Exception
    {
        public RabbitMQConfigurationException(string message)
            : base(message)
        {
        }
    }
}
