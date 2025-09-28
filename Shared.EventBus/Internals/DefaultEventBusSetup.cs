using Microsoft.Extensions.DependencyInjection;

namespace Shared.EventBus.Internals
{
    internal sealed class DefaultEventBusSetup : IEventBusSetup
    {
        private readonly IServiceCollection _services;
        private readonly ConsumerManager _consumerManager = new();

        public DefaultEventBusSetup(IServiceCollection services)
        {
            _services = services;
            _services.AddSingleton<IConsumerManager>(_consumerManager);
            _services.AddSingleton<IMessageProcessor, MessageProcessor>();
        }

        public IEventBusSetup Consume<TMessage, TConsumer>()
            where TConsumer : IConsumer<TMessage>
        {
            _consumerManager.TryAdd<TMessage, TConsumer>();
            return this;
        }

        public IEventBusSetup UseEventBus<IBusConfigurer>(Action<IBusConfigurer> configure) where IBusConfigurer : EventBus.IBusConfigurer
        {
            var configurer = (IBusConfigurer)Activator.CreateInstance(typeof(IBusConfigurer), _services)!;
            configure(configurer);
            configurer.Complete();
            return this;
        }
    }
}
