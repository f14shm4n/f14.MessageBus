namespace Shared.EventBus
{
    public interface IConsumerManager
    {
        bool TryAdd<TMessage, TConsumer>()
           where TMessage : class
           where TConsumer : IConsumer<TMessage>;

        void TryGetConsumers(Type messageType, out IReadOnlyCollection<Type>? consumers);
    }
}
