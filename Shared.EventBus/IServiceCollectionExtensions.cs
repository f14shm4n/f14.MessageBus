using Microsoft.Extensions.DependencyInjection;
using Shared.EventBus.Internals;

namespace Shared.EventBus
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddEventBus(this IServiceCollection services, Action<IEventBusSetup> configure)
        {
            var setup = new DefaultEventBusSetup(services);
            configure(setup);

            services.AddSingleton<IEventBus, DefaultEventBus>();
            services.AddHostedService<EventBusStartingHostedService>();            

            return services;
        }
    }
}
