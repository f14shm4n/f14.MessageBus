namespace Shared.EventBus
{
    public class EventRemovedEventArgs : EventArgs
    {
        public EventRemovedEventArgs(Type eventType)
        {
            EventType = eventType;
        }

        public Type EventType { get; init; }
    }
}
