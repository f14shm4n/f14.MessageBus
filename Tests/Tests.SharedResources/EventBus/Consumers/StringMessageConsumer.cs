using Shared.EventBus;
using Tests.SharedResources.EventBus.Messages;

namespace Tests.SharedResources.EventBus.Consumers
{
    public class StringMessageConsumer : IConsumer<StringMessage>
    {
        public Task ConsumeAsync(StringMessage message, CancellationToken token = default)
        {
            return Task.CompletedTask;
        }
    }
}
