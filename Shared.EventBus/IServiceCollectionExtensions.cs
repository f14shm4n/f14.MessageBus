using Microsoft.Extensions.DependencyInjection;

namespace Shared.EventBus
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddEventBus(this IServiceCollection services, Action<IEventBusConfigurator> configure)
        {
            var configurator = new InternalEventBusConfigurator(services);
            configure(configurator);            

            services.AddHostedService<EventBusStartingHostedService>();
            services.AddSingleton<IEventBus, InternalEventBus>();

            return services;
        }
    }
}
