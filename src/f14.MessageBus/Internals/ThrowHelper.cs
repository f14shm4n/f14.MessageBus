using f14.MessageBus;
using System.Diagnostics.CodeAnalysis;

namespace f14.MessageBus.Internals
{
    internal static class ThrowHelper
    {
        [DoesNotReturn]
        public static void UnknownMessageType(string messageTypeName)
        {
            throw new InvalidOperationException($"Unknown message type name: '{messageTypeName}'. Make sure the message type name is registered in {nameof(IConsumerManager)}.");
        }

        [DoesNotReturn]
        public static void NoRegisteredConsumers(string messageTypeName)
        {
            throw new InvalidOperationException($"There are no registered consumers for given message type name: '{messageTypeName}'.");
        }

        [DoesNotReturn]
        public static void CantInvokeConsumerInstanceIsNull()
        {
            throw new InvalidOperationException("Cannot invoke consumer method because no consumer instance has been set.");
        }
    }
}
