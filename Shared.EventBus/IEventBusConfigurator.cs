using Microsoft.Extensions.DependencyInjection;

namespace Shared.EventBus
{
    public interface IEventBusConfigurator
    {
        IEventBusConfigurator AddConsumer<TMessage, TConsumer>()
           where TMessage : class
           where TConsumer : IConsumer<TMessage>;
        IEventBusConfigurator UseEventBus<TBusBuilder>(Action<TBusBuilder> configurator) where TBusBuilder : IBusBuilder;
    }
}
