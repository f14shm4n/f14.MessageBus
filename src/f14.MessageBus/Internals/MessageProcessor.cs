using Microsoft.Extensions.DependencyInjection;

namespace f14.MessageBus.Internals
{
    internal sealed class MessageProcessor : IMessageProcessor
    {
        private readonly IConsumerManager _consumerManager;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IMessageSerializer _messageSerializer;
        private readonly IConsumerInvokerFabric _consumerMetaFabric;

        public MessageProcessor(
            IServiceScopeFactory scopeFactory,
            IConsumerManager consumerManager,
            IMessageSerializer messageSerializer,
            IConsumerInvokerFabric consumerMetaFabric)
        {
            _scopeFactory = scopeFactory;
            _consumerManager = consumerManager;
            _messageSerializer = messageSerializer;
            _consumerMetaFabric = consumerMetaFabric;
        }

        public async Task ProcessMessageAsync(string messageTypeName, ReadOnlyMemory<byte> messageBody, CancellationToken cancellationToken = default)
        {
            // Check if message type is registered
            var messageType = _consumerManager.GetMessageTypeByName(messageTypeName);
            if (messageType == null)
            {
                ThrowHelper.UnknownMessageType(messageTypeName);
            }
            // Check if we have any consumer for the registered message
            _consumerManager.TryGetConsumers(messageType, out var consumerTypes);
            if (consumerTypes is null)
            {
                ThrowHelper.NoRegisteredConsumers(messageTypeName);
            }

            using (var scope = _scopeFactory.CreateScope())
            {
                var message = _messageSerializer.Deserialize(messageBody.ToArray(), messageType);
                foreach (var consumerType in consumerTypes)
                {
                    // We using IDisposable to clear consumer instance after invoke
                    using (var invoker = _consumerMetaFabric.GetInvoker(scope, consumerType, messageType))
                    {
                        await Task.Yield();
                        await invoker.InvokeAsync([message, cancellationToken]);
                    }
                }
            }
        }
    }
}
