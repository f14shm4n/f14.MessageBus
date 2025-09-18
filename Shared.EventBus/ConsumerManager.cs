namespace Shared.EventBus
{
    internal sealed class ConsumerManager : IConsumerManager
    {
        private readonly Dictionary<Type, List<Type>> _consumers;

        public ConsumerManager()
        {
            _consumers = [];
        }

        public bool TryAdd<TMessage, TConsumer>()
            where TMessage : class
            where TConsumer : IConsumer<TMessage>
        {
            return TryAddInternal(typeof(TMessage), typeof(TConsumer));
        }

        private bool TryAddInternal(Type messageType, Type consumerType)
        {
            if (!_consumers.TryGetValue(messageType, out var list))
            {
                list = [];
                _consumers[messageType] = list;
            }

            if (list.Any(x => x == consumerType))
            {
                return false;
            }
            list.Add(consumerType);
            return true;
        }

        public void TryGetConsumers(Type messageType, out IReadOnlyCollection<Type>? consumers)
        {
            consumers = null;
            if (_consumers.TryGetValue(messageType, out var list))
            {
                consumers = list;
            }
        }
    }
}
