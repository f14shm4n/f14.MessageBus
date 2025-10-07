using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace f14.MessageBus.Internals
{
    internal sealed class BusInitializer : IBusInitializer
    {
        private readonly IServiceCollection _services;
        private readonly ConsumerManager _consumerManager = new();

        public BusInitializer(IServiceCollection services)
        {
            _services = services;
            // TODO: Some classes should be redefinable
            // Redefinable?
            _services.AddSingleton<IConsumerManager>(_consumerManager);
            _services.AddSingleton<IMessageProcessor, MessageProcessor>();
            _services.AddSingleton<IMessageSerializer, JsonTextMessageSerializer>();
            _services.AddSingleton<IConsumerInvokerFabric, ConsumerInvokerFabric>();
            // Non redefinable
            _services.AddHostedService<MessageBusStartingHostedService>();
        }

        public IBusInitializer Consume<TMessage, TConsumer>()
            where TConsumer : IConsumer<TMessage>
        {
            _consumerManager.TryAdd<TMessage, TConsumer>();
            return this;
        }

        public IBusInitializer UseBus<TBusConfigurer>(Action<TBusConfigurer> configure) where TBusConfigurer : IBusConfigurer
        {
            var configurer = (TBusConfigurer)Activator.CreateInstance(typeof(TBusConfigurer), _services)!;
            configure(configurer);
            configurer.Complete();
            return this;
        }

        public IBusInitializer ReplaceMessageSerializer<TMessageSerializerImpl>() where TMessageSerializerImpl : class, IMessageSerializer
        {
            _services.Replace(ServiceDescriptor.Singleton<IMessageSerializer, TMessageSerializerImpl>());
            return this;
        }
    }
}
