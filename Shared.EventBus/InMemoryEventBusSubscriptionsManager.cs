using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.EventBus
{
    /// <summary>
    /// In memory implementation of the <see cref="IEventBusSubscriptionsManager{I}"/>.
    /// <inheritdoc cref="IEventBusSubscriptionsManager{I}"/>
    /// </summary>
    public abstract class InMemoryEventBusSubscriptionsManager<I> : IEventBusSubscriptionsManager<I>
        where I : ISubscriptionInfo
    {
        private readonly Dictionary<Type, List<I>> _subscriptions = [];

        public bool IsEmpty => _subscriptions.Count == 0;

        public event EventHandler<EventRemovedEventArgs>? OnEventRemoved;

        public void AddSubscription<E, H>()
            where E : IntegrationEvent
            where H : IIntegrationEventHandler<E>
        {
            var eventType = typeof(E);
            if (!_subscriptions.TryGetValue(eventType, out List<I>? subList))
            {
                subList = [];
                _subscriptions.Add(eventType, subList);
            }

            var hType = typeof(H);
            if (subList.Any(s => s.EventHandlerType == hType))
            {
                throw new InvalidOperationException($"The {GetType().Name} already has a subscription with event handler type: {hType.Name}.");
            }
            subList.Add(CreateSubscriptionInfo(eventType, hType));
        }

        public void Clear()
        {
            _subscriptions.Clear();
        }

        public bool Contains(Type eventType)
        {
            return _subscriptions.ContainsKey(eventType);
        }

        public bool Contains<E>() where E : IntegrationEvent
        {
            return Contains(typeof(E));
        }

        public Type? GetEventTypeByName(string eventTypeName)
        {
            return _subscriptions.Keys.FirstOrDefault(k => k.Name == eventTypeName);
        }

        public IEnumerable<I>? GetSubscriptions(Type eventType)
        {
            _subscriptions.TryGetValue(eventType, out var list);
            return list;
        }

        public void RemoveSubscription<E, H>()
            where E : IntegrationEvent
            where H : IIntegrationEventHandler<E>
        {
            var eventType = typeof(E);
            if (!_subscriptions.TryGetValue(eventType, out List<I>? subList))
            {
                subList = [];
                _subscriptions.Add(eventType, subList);
            }

            var hType = typeof(H);
            var sub = subList.Find(s => s.EventHandlerType == hType);
            if (sub != null)
            {
                subList.Remove(sub);
                if (subList.Count == 0)
                {
                    _subscriptions.Remove(eventType);
                    OnEventRemoved?.Invoke(this, new EventRemovedEventArgs(eventType));
                }
            }
        }

        protected abstract I CreateSubscriptionInfo(Type eventType, Type eventHandlerType);
    }
}
