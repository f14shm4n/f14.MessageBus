using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shared.EventBus;

namespace Shared.RabbitMQ.App
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddRabbitMQEventBus(this IServiceCollection services, IConfigurationSection configuration)
        {
            services
                .Configure<RabbitMQAppOptions>(configuration.GetSection("RabbitMQ"))
                .AddSingleton<IConnectionFactoryProvider, UriConnectionFactoryProvider>()
                .AddSingleton<IRabbitMQPersistentConnection, RabbitMQPersistentConnection>()
                .AddSingleton<IEventBusSubscriptionsManager, InMemoryEventBusSubscriptionsManager>()
                .AddSingleton<RabbitMQPersistentPublishChannel>()
                .AddSingleton<RabbitMQPersistentConsumerChannel>()
                .AddSingleton<IEventBusPublisher, RabbitMQEventBus>()
                .AddSingleton<IEventBusReceiver, RabbitMQEventBus>();
            return services;
        }
    }
}
