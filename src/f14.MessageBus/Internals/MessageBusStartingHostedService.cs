using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace f14.MessageBus.Internals
{
    internal sealed class MessageBusStartingHostedService : IHostedService
    {
        private readonly ILogger<MessageBusStartingHostedService> _logger;
        private readonly IEnumerable<IBusInstance> _instances;

        public MessageBusStartingHostedService(IEnumerable<IBusInstance> instances, ILogger<MessageBusStartingHostedService> logger)
        {
            _instances = instances;
            _logger = logger;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.StartingBusInstances();

            foreach (var instance in _instances)
            {
                instance.StartAsync(cancellationToken);
            }

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.StoppingBusInstances();

            foreach (var instance in _instances)
            {
                instance.StopAsync(cancellationToken);
            }

            return Task.CompletedTask;
        }
    }

    internal static partial class LoggingExtensions
    {
        [LoggerMessage(Level = LogLevel.Information, Message = "Starting event bus instances...")]
        internal static partial void StartingBusInstances(this ILogger logger);

        [LoggerMessage(Level = LogLevel.Information, Message = "Stopping event bus instances...")]
        internal static partial void StoppingBusInstances(this ILogger logger);
    }
}
