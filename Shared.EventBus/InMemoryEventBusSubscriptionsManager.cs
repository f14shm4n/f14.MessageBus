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
    public class InMemoryEventBusSubscriptionsManager : IEventBusSubscriptionsManager<SubscriptionInfo>
    {
        private readonly Dictionary<string, List<SubscriptionInfo>> _subscriptions = [];

        public bool IsEmpty => _subscriptions.Count == 0;

        public event EventHandler<string>? OnEventRemoved;

        public void AddSubscription<E, H>()
            where E : IntegrationEvent
            where H : IIntegrationEventHandler<E>
        {
            var eKey = GetEventKey<E>();
            if (!_subscriptions.TryGetValue(eKey, out List<SubscriptionInfo>? subList))
            {
                subList = [];
                _subscriptions.Add(eKey, subList);
            }

            var hType = typeof(H);
            if (subList.Any(s => s.IntegrationEventHandlerType == hType))
            {
                throw new InvalidOperationException($"The {GetType().Name} already has a subscription with event handler type: {hType.Name}.");
            }
            subList.Add(new SubscriptionInfo(hType));
        }

        public string GetEventKey<E>() => typeof(E).Name;

        public IEnumerable<SubscriptionInfo>? GetSubscriptions<E>() where E : IntegrationEvent
        {
            _subscriptions.TryGetValue(GetEventKey<E>(), out var list);
            return list;
        }

        public void RemoveSubscription<E, H>()
            where E : IntegrationEvent
            where H : IIntegrationEventHandler<E>
        {
            var eKey = GetEventKey<E>();
            if (!_subscriptions.TryGetValue(eKey, out List<SubscriptionInfo>? subList))
            {
                subList = [];
                _subscriptions.Add(eKey, subList);
            }

            var hType = typeof(H);
            var sub = subList.Find(s => s.IntegrationEventHandlerType == hType);
            if (sub != null)
            {
                subList.Remove(sub);
                if (subList.Count == 0)
                {
                    _subscriptions.Remove(eKey);
                    OnEventRemoved?.Invoke(this, eKey);
                }
            }
        }
    }
}
