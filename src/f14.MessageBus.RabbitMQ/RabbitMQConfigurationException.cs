using System.Diagnostics.CodeAnalysis;

namespace f14.MessageBus.RabbitMQ
{
    public sealed class RabbitMQConfigurationException : Exception
    {
        public RabbitMQConfigurationException(string message)
            : base(message)
        {
        }
    }
}
