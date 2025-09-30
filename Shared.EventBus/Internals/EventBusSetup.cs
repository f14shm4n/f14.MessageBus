using Microsoft.Extensions.DependencyInjection;

namespace Shared.EventBus.Internals
{
    internal sealed class EventBusSetup : IEventBusSetup
    {
        private readonly IServiceCollection _services;
        private readonly ConsumerManager _consumerManager = new();

        public EventBusSetup(IServiceCollection services)
        {
            _services = services;
            // TODO: Some classes should be redefinable
            // Redefinable
            _services.AddSingleton<IConsumerManager>(_consumerManager);
            _services.AddSingleton<IMessageProcessor, MessageProcessor>();
            _services.AddSingleton<IMessageSerializer, JsonTextMessageSerializer>();
            _services.AddSingleton<IEventBus, DefaultEventBus>();
            // Non redefinable
            _services.AddHostedService<EventBusStartingHostedService>();
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
