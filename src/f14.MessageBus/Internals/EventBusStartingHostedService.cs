using f14.MessageBus;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Shared.EventBus
{
    internal sealed class EventBusStartingHostedService : IHostedService
    {
        private readonly ILogger<EventBusStartingHostedService> _logger;
        private readonly IEnumerable<IEventBusInstance> _instances;

        public EventBusStartingHostedService(IEnumerable<IEventBusInstance> instances, ILogger<EventBusStartingHostedService> logger)
        {
            _instances = instances;
            _logger = logger;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.StartingEventBusInstances();

            foreach (var instance in _instances)
            {
                instance.StartAsync(cancellationToken);
            }

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.StoppingEventBusInstances();

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
        internal static partial void StartingEventBusInstances(this ILogger logger);

        [LoggerMessage(Level = LogLevel.Information, Message = "Stopping event bus instances...")]
        internal static partial void StoppingEventBusInstances(this ILogger logger);
    }
}
