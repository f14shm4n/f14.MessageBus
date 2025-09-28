using Microsoft.Extensions.Logging;

namespace Shared.EventBus.Internals
{
    internal sealed class DefaultEventBus : IEventBus
    {
        private readonly ILogger<DefaultEventBus> _logger;
        private readonly IEnumerable<IEventBusInstance> _instances;

        public DefaultEventBus(ILogger<DefaultEventBus> logger, IEnumerable<IEventBusInstance> instances)
        {
            _logger = logger;
            _instances = instances;
        }

        public async Task SendAsync<TMessage>(TMessage message, CancellationToken cancellationToken = default) where TMessage : class
        {
            foreach (var instance in _instances)
            {
                await instance.SendAsync(message, cancellationToken);
            }
        }
    }
}
