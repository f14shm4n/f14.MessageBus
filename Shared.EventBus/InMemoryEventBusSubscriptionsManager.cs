namespace Shared.EventBus
{
    /// <summary>
    /// In memory implementation of the <see cref="IEventBusSubscriptionsManager"/>.
    /// <inheritdoc cref="IEventBusSubscriptionsManager"/>
    /// </summary>
    public class InMemoryEventBusSubscriptionsManager : IEventBusSubscriptionsManager
    {
        private readonly Dictionary<Type, List<SubscriptionInfo>> _subscriptions = [];

        #region Properties

        public bool IsEmpty => _subscriptions.Count == 0;

        #endregion

        #region Events

        public event EventHandler<EventRemovedEventArgs>? OnEventRemoved;

        #endregion

        public void AddSubscription<E, H>()
            where E : IntegrationEvent
            where H : IIntegrationEventHandler<E>
        {
            var eventType = typeof(E);
            if (!_subscriptions.TryGetValue(eventType, out List<SubscriptionInfo>? subList))
            {
                subList = [];
                _subscriptions.Add(eventType, subList);
            }

            var hType = typeof(H);
            if (subList.Any(s => s.EventHandlerType == hType))
            {
                throw new InvalidOperationException($"The {GetType().Name} already has a subscription with event handler type: {hType.Name}.");
            }
            subList.Add(new SubscriptionInfo(eventType, hType));
        }

        public void Clear()
        {
            _subscriptions.Clear();
        }

        public Type? GetEventTypeByName(string eventTypeName)
        {
            return _subscriptions.Keys.FirstOrDefault(k => k.Name == eventTypeName);
        }

        public IEnumerable<SubscriptionInfo>? GetSubscriptions(Type eventType)
        {
            _subscriptions.TryGetValue(eventType, out var list);
            return list;
        }

        public void RemoveSubscription<E, H>()
            where E : IntegrationEvent
            where H : IIntegrationEventHandler<E>
        {
            var eventType = typeof(E);
            if (!_subscriptions.TryGetValue(eventType, out List<SubscriptionInfo>? subList))
            {
                subList = [];
                _subscriptions.Add(eventType, subList);
            }

            var hType = typeof(H);
            var sub = subList.Find(s => s.EventHandlerType == hType);
            if (sub == null)
            {
                return;
            }

            subList.Remove(sub);
            if (subList.Count == 0)
            {
                _subscriptions.Remove(eventType);
                OnEventRemoved?.Invoke(this, new EventRemovedEventArgs(eventType));
            }
        }
    }
}
