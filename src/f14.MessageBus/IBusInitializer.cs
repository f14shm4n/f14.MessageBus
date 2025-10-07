namespace f14.MessageBus
{
    public interface IBusInitializer
    {
        IBusInitializer Consume<TMessage, TConsumer>() where TConsumer : IConsumer<TMessage>;
        IBusInitializer UseBus<TIBusConfigurer>(Action<TIBusConfigurer> configurator) where TIBusConfigurer : IBusConfigurer;
    }
}
