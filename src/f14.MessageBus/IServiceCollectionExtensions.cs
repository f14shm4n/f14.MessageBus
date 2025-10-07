using f14.MessageBus.Internals;
using Microsoft.Extensions.DependencyInjection;

namespace f14.MessageBus
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddMessageBus(this IServiceCollection services, Action<IBusInitializer> configure)
        {
            var initializer = new BusInitializer(services);
            configure(initializer);
            return services;
        }
    }
}
