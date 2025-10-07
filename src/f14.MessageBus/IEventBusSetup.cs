namespace f14.MessageBus
{
    public interface IEventBusSetup
    {
        IEventBusSetup Consume<TMessage, TConsumer>() where TConsumer : IConsumer<TMessage>;
        IEventBusSetup UseEventBus<TIBusConfigurer>(Action<TIBusConfigurer> configurator) where TIBusConfigurer : IBusConfigurer;
    }
}
