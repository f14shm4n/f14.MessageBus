namespace f14.MessageBus.Internals
{
    internal sealed class ConsumerManager : IConsumerManager
    {
        private readonly Dictionary<Type, List<Type>> _consumers;

        public ConsumerManager()
        {
            _consumers = [];
        }

        public int MessageTypesCount => _consumers.Count;

        public int ConsumersCount => _consumers.Values.Select(x => x.Count).Sum();

        public bool TryAdd<TMessage, TConsumer>()
            where TConsumer : IConsumer<TMessage>
        {
            return TryAddInternal(typeof(TMessage), typeof(TConsumer));
        }

        public Type? GetMessageTypeByName(string messageTypeName) => _consumers.Keys.FirstOrDefault(k => k.Name == messageTypeName);

        public void TryGetConsumers(Type messageType, out IReadOnlyCollection<Type>? consumers)
        {
            consumers = null;
            if (_consumers.TryGetValue(messageType, out var list))
            {
                consumers = list;
            }
        }

        #region Private

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

        #endregion
    }
}
