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
                .AddSingleton<IRabbitMQPersistentConnection, RabbitMQAppPersistentConnection>()
                .AddSingleton<IRabbitMQSubscriptionsManager<SubscriptionInfo>, DefaultInMemoryRabbitMQSubscriptionsManager>()
                .AddSingleton<RabbitMQPersistentPublishChannel>()
                .AddSingleton<RabbitMQPersistentConsumerChannel>()
                .AddSingleton<IRabbitMQEventBus, RabbitMQEventBus>();
            return services;
        }
    }
}
