namespace Shared.EventBus
{
    public static class IntegrationEventHandlerHelper
    {
        public static string HandleMethodName => nameof(IIntegrationEventHandler<IntegrationEvent>.Handle);

    }
}