using Microsoft.Extensions.DependencyInjection;

namespace Shared.EventBus
{
    internal sealed class InternalEventBusConfigurator : IEventBusConfigurator
    {
        private readonly ConsumerManager _consumerManager;
        private readonly Dictionary<Type, Action<IEventBusInstance>> _instancesConfigs = [];
        private readonly IServiceCollection _services;

        public InternalEventBusConfigurator(IServiceCollection services)
        {
            _services = services;
            _consumerManager = new();
            _services.AddSingleton<IConsumerManager>(_consumerManager);
        }

        public IEventBusConfigurator AddConsumer<TMessage, TConsumer>()
            where TMessage : class
            where TConsumer : IConsumer<TMessage>
        {
            _consumerManager.TryAdd<TMessage, TConsumer>();
            return this;
        }

        public IEventBusConfigurator UseEventBus<TBusBuilder>(Action<TBusBuilder> configurator)
            where TBusBuilder : IBusBuilder
        {
            _services.AddSingleton(sp =>
            {
                var builder = ActivatorUtilities.CreateInstance<TBusBuilder>(sp);
                configurator(builder);
                return builder.Build();
            });
            return this;
        }
    }
}
