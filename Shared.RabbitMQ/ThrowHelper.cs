using System.Diagnostics.CodeAnalysis;

namespace Shared.RabbitMQ
{
    internal static class ThrowHelper
    {
        [DoesNotReturn]
        public static InvalidOperationException NoConnection()
        {
            throw new InvalidOperationException($"RabbitMQ connection not established yet.");
        }

        [DoesNotReturn]
        public static InvalidOperationException NoExchangesRegisteredForRoutingKey(string routingKey)
        {
            throw new InvalidOperationException($"No exchanges are registered with this routing key. RoutingKey: '{routingKey}'.");
        }

        [DoesNotReturn]
        public static InvalidOperationException NoAvailableConnection()
        {
            throw new InvalidOperationException("No RabbitMQ connection available.");
        }

        [DoesNotReturn]
        public static InvalidOperationException ExchangeOrQueueNamesIsNotSet()
        {
            throw new InvalidOperationException("The Exchange and Queue name cannot be empty and must be configured.");
        }

        [DoesNotReturn]
        public static InvalidOperationException PublisherIsNotSet()
        {
            throw new InvalidOperationException("The RabbitMQ publisher is not set.");
        }

        [DoesNotReturn]
        public static InvalidOperationException ConnectionFactoryIsNotConfigured()
        {
            throw new InvalidOperationException("The RabbitMQ connection factory is not configured.");
        }

        [DoesNotReturn]
        public static RabbitMQConfigurationException PublisherAndConsumerNotDefined()
        {
            throw new RabbitMQConfigurationException($"The RabbitMQ configuration does not have a publisher or consumer configured.");
        }
    }
}
