using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Text;
using System.Text.Json;

namespace Shared.EventBus.Internals
{
    internal sealed class MessageProcessor : IMessageProcessor
    {
        private readonly ILogger<MessageProcessor> _logger;
        private readonly IConsumerManager _consumerManager;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly JsonSerializerOptions _jsonOptions = new() { PropertyNameCaseInsensitive = true };

        public MessageProcessor(ILogger<MessageProcessor> logger, IConsumerManager consumerManager, IServiceScopeFactory scopeFactory)
        {
            _logger = logger;
            _consumerManager = consumerManager;
            _scopeFactory = scopeFactory;
        }

        public async Task ProcessMessageAsync(string messageKey, ReadOnlyMemory<byte> messageBody, CancellationToken cancellationToken = default)
        {
            _logger.LogTrace("Processing message. MessageType: '{Type}'.", messageKey);

            var messageType = _consumerManager.GetMessageTypeByName(messageKey);
            if (messageType is null)
            {
                _logger.LogWarning("There are no registered consumers for this message type.: '{Type}'", messageKey);
                return;
            }
            _consumerManager.TryGetConsumers(messageType, out var consumerTypes);
            if (consumerTypes is null)
            {
                return;
            }

            using (var scope = _scopeFactory.CreateScope())
            {
                var messageAsString = Encoding.UTF8.GetString(messageBody.ToArray());
                foreach (var consumerType in consumerTypes)
                {
                    var consumer = ActivatorUtilities.CreateInstance(scope.ServiceProvider, consumerType);
                    var message = JsonSerializer.Deserialize(messageAsString, messageType, _jsonOptions);
                    var concreteType = typeof(IConsumer<>).MakeGenericType(messageType);

                    await Task.Yield();
                    await (Task)concreteType.GetMethod(Constants.ConsumeAsyncMethodName)!.Invoke(consumer, [message, cancellationToken])!;
                }
            }
        }
    }
}
