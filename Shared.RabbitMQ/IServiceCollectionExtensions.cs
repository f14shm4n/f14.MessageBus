using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shared.EventBus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.RabbitMQ
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddRabbitMQEventBus(this IServiceCollection services, IConfigurationSection configuration)
        {
            services
                .Configure<RabbitMQOptions>(configuration)
                .AddSingleton<IRabbitMQPersistentConnection, DefaultRabbitMQPersistentConnection>()
                .AddSingleton<IRabbitMQSubscriptionsManager, InMemoryRabbitMQSubscriptionsManager>()
                .AddSingleton<DefaultRabbitMQPersistentPublishChannel>()
                .AddSingleton<DefaultRabbitMQPersistentConsumerChannel>()
                .AddSingleton<IRabbitMQEventBus, RabbitMQEventBus>();
            return services;
        }
    }
}
