using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.EventBus
{
    /// <summary>
    /// The event bus subscription manager stores and maps event names and their handlers.
    /// </summary>
    public interface IEventBusSubscriptionsManager
    {
        /// <summary>
        /// Determines whether the manager contains any subscriptions.
        /// </summary>
        bool IsEmpty { get; }

        /// <summary>
        /// Occurs when the manager does not contain any handlers for the corresponding event name.
        /// </summary>
        event EventHandler<EventRemovedEventArgs>? OnEventRemoved;

        /// <summary>
        /// Adds a new event subscription with the specified event handler.
        /// </summary>        
        /// <typeparam name="E">Type of the event.</typeparam>
        /// <typeparam name="H">Type of the event handler.</typeparam>
        void AddSubscription<E, H>()
            where E : IntegrationEvent
            where H : IIntegrationEventHandler<E>;

        /// <summary>
        /// Gets the event type by the specified type name.
        /// </summary>
        /// <param name="eventTypeName">The name of the type to get.</param>
        /// <returns>Type or null.</returns>
        Type? GetEventTypeByName(string eventTypeName);

        /// <summary>
        /// Gets the event type using provided event key.
        /// </summary>
        /// <param name="eventType">Type of the event.</param>
        /// <returns>Collection with handler types or null.</returns>
        IEnumerable<SubscriptionInfo>? GetSubscriptions(Type eventType);

        /// <summary>
        /// Removes the subscription for the given event and handler.
        /// </summary>
        /// <typeparam name="E">Type of the event.</typeparam>
        /// <typeparam name="H">Type of the event handler.</typeparam>
        void RemoveSubscription<E, H>()
            where E : IntegrationEvent
            where H : IIntegrationEventHandler<E>;

        /// <summary>
        /// Removes all subscriptions.
        /// </summary>
        void Clear();
    }
}
