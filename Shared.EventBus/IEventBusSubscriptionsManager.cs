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
    /// <typeparam name="I">Type of the specific subscription info.</typeparam>
    public interface IEventBusSubscriptionsManager<I> where I : ISubscriptionInfo
    {
        /// <summary>
        /// Determines whether the manager contains any subscriptions.
        /// </summary>
        bool IsEmpty { get; }

        /// <summary>
        /// Occurs when the manager does not contain any handlers for the corresponding event name.
        /// </summary>
        event EventHandler<string>? OnEventRemoved;

        /// <summary>
        /// Adds a new event subscription with the specified event handler.
        /// </summary>
        /// <typeparam name="E">Type of the event.</typeparam>
        /// <typeparam name="H">Type of the event handler.</typeparam>
        void AddSubscription<E, H>()
            where E : IntegrationEvent
            where H : IIntegrationEventHandler<E>;

        /// <summary>
        /// Gets the event key.
        /// </summary>
        /// <typeparam name="E">Type of the event.</typeparam>
        /// <returns>Event key as string value.</returns>
        string GetEventKey<E>();

        /// <summary>
        /// Gets a collection of event handlers for the specified event type.
        /// </summary>
        /// <typeparam name="E">Type of the event.</typeparam>
        /// <returns>Collection with handler types or null.</returns>
        IEnumerable<I>? GetSubscriptions<E>() where E : IntegrationEvent;

        /// <summary>
        /// Removes the subscription for the given event and handler.
        /// </summary>
        /// <typeparam name="E">Type of the event.</typeparam>
        /// <typeparam name="H">Type of the event handler.</typeparam>
        void RemoveSubscription<E, H>()
            where E : IntegrationEvent
            where H : IIntegrationEventHandler<E>;
    }
}
