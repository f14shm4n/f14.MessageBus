using f14.MessageBus.Internals;
using Microsoft.Extensions.DependencyInjection;

namespace f14.MessageBus
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
