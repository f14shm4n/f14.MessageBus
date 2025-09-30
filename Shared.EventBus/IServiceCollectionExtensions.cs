using Microsoft.Extensions.DependencyInjection;
using Shared.EventBus.Internals;

namespace Shared.EventBus
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddEventBus(this IServiceCollection services, Action<IEventBusSetup> configure)
        {
            var setup = new EventBusSetup(services);
            configure(setup);
            return services;
        }
    }
}
