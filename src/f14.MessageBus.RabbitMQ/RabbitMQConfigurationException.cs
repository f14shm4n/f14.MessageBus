namespace f14.MessageBus.RabbitMQ
{
    public sealed class RabbitMQConfigurationException : Exception
    {
        public RabbitMQConfigurationException()
        {
        }

        public RabbitMQConfigurationException(string message) : base(message)
        {
        }

        public RabbitMQConfigurationException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
