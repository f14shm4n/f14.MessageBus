using RabbitMQ.Client;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Shared.RabbitMQ
{
    public sealed class RabbitMQConfigurationException : Exception
    {
        private RabbitMQConfigurationException(string message)
            : base(message)
        {

        }

        public static void ThrowIfConnectionFactoryIsNull([NotNull] IConnectionFactory? connectionFactory)
        {
            if (connectionFactory is null)
            {
                throw new RabbitMQConfigurationException($"The connection factory is not configured.");
            }
        }

        public static void ThrowIfDeclarationIsNullOrEmpty([NotNull] IEnumerable<Func<IChannel, CancellationToken, Task>>? declarations, [CallerArgumentExpression(nameof(declarations))] string? paramName = null)
        {
            if (declarations is null || !declarations.Any())
            {
                throw new RabbitMQConfigurationException($"RabbitMQ does not have any {paramName} declarations.");
            }
        }
    }
}
