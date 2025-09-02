using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;
using Shared.EventBus;

namespace Shared.RabbitMQ.App
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddRabbitMQEventBus(this IServiceCollection services, IConfigurationSection configuration)
        {
            services
                .Configure<RabbitMQAppOptions>(configuration.GetSection("RabbitMQ"))
                .AddSingleton<IIntegrationEventProcessor, IntegrationEventProcessor>()
                .AddSingleton<IConnectionFactoryProvider, UriConnectionFactoryProvider>()
                .AddSingleton<IBasicPropertiesProvider, SimpleBasicPropertiesProvider>(sp => new SimpleBasicPropertiesProvider(new BasicProperties { DeliveryMode = DeliveryModes.Persistent }))
                .AddSingleton<IRabbitMQPersistentConnection, RabbitMQPersistentConnection>()
                .AddSingleton<IEventBusSubscriptionsManager, InMemoryEventBusSubscriptionsManager>()
                .AddSingleton<IEventBusPublisher, RabbitMQPublisher>()
                .AddSingleton<IEventBusReceiver, RabbitMQReceiver>();
            return services;
        }
    }
}
