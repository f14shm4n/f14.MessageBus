namespace Shared.EventBus
{
    public interface IConsumerManager
    {
        Type? GetMessageTypeByName(string messageTypeName);
        bool TryAdd<TMessage, TConsumer>() where TConsumer : IConsumer<TMessage>;

        void TryGetConsumers(Type messageType, out IReadOnlyCollection<Type>? consumers);
    }
}
