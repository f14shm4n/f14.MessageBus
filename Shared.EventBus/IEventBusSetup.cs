namespace Shared.EventBus
{
    public interface IEventBusSetup
    {
        IEventBusSetup Consume<TMessage, TConsumer>() where TConsumer : IConsumer<TMessage>;
        IEventBusSetup UseEventBus<TIBusConfigurer>(Action<TIBusConfigurer> configurator) where TIBusConfigurer : IBusConfigurer;
    }
}
