using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace Shared.EventBus
{
    //public sealed class IntegrationEventProcessor : IIntegrationEventProcessor
    //{
    //    private readonly ILogger<IntegrationEventProcessor> _logger;
    //    private readonly IEventBusSubscriptionsManager _subscriptionsManager;
    //    private readonly IServiceScopeFactory _scopeFactory;

    //    public IntegrationEventProcessor(ILogger<IntegrationEventProcessor> logger, IEventBusSubscriptionsManager subscriptionsManager, IServiceScopeFactory scopeFactory)
    //    {
    //        _logger = logger;
    //        _subscriptionsManager = subscriptionsManager;
    //        _scopeFactory = scopeFactory;
    //    }

    //    public async Task ProcessEventAsync(string eventName, string message)
    //    {
    //        _logger.LogTrace("Processing event: {EventName}", eventName);

    //        var eventType = _subscriptionsManager.GetEventTypeByName(eventName);
    //        if (eventType is null)
    //        {
    //            _logger.LogWarning("No subscription for event: {EventName}", eventName);
    //            return;
    //        }

    //        using (var scope = _scopeFactory.CreateScope())
    //        {
    //            var subscriptions = _subscriptionsManager.GetSubscriptions(eventType);
    //            if (subscriptions is not null)
    //            {
    //                var jsonOptions = new JsonSerializerOptions()
    //                {
    //                    PropertyNameCaseInsensitive = true
    //                };

    //                foreach (var subscription in subscriptions)
    //                {
    //                    var handler = ActivatorUtilities.CreateInstance(scope.ServiceProvider, subscription.EventHandlerType);
    //                    var integrationEvent = JsonSerializer.Deserialize(message, eventType, jsonOptions);
    //                    var concreteType = typeof(IIntegrationEventHandler<>).MakeGenericType(eventType);

    //                    await Task.Yield();
    //                    await (Task)concreteType.GetMethod(IntegrationEventHandlerHelper.HandleMethodName)!.Invoke(handler, [integrationEvent])!;
    //                }
    //            }
    //            else
    //            {
    //                _logger.LogWarning("No event handlers was found for the event: '{EventName}', the event handler collection may be empty or was unsubscribed from the current event while attempting to handle it.", eventName);
    //            }
    //        }
    //    }
    //}
}
