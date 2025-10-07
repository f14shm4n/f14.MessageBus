using f14.MessageBus;
using Tests.SharedResources.EventBus.Messages;

namespace Tests.SharedResources.EventBus.Consumers
{
    public class AnotherInt32MessageConsumer : IConsumer<Int32Message>
    {
        public Task ConsumeAsync(Int32Message message, CancellationToken token = default)
        {
            return Task.CompletedTask;
        }
    }
}
