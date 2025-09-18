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

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.StartingEventBusInstances();

            foreach (var instance in _instances)
            {
                await instance.StartAsync(cancellationToken);
            }

            _logger.EventBusInstanceStarted();
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.StoppingEventBusInstances();

            foreach (var instance in _instances)
            {
                await instance.StopAsync(cancellationToken);
            }

            _logger.EventBusInstanceStopped();
        }
    }

    internal static partial class LoggingExtensions
    {
        [LoggerMessage(Level = LogLevel.Information, Message = "Starting event bus instances...")]
        internal static partial void StartingEventBusInstances(this ILogger logger);

        [LoggerMessage(Level = LogLevel.Information, Message = "Event bus instances are running.")]
        internal static partial void EventBusInstanceStarted(this ILogger logger);

        [LoggerMessage(Level = LogLevel.Information, Message = "Stopping event bus instances...")]
        internal static partial void StoppingEventBusInstances(this ILogger logger);

        [LoggerMessage(Level = LogLevel.Information, Message = "Event bus instances have stopped.")]
        internal static partial void EventBusInstanceStopped(this ILogger logger);
    }
}
