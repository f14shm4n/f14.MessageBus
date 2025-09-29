using Shared.EventBus;
using Tests.SharedResources.EventBus.Messages;

namespace Tests.SharedResources.EventBus.Consumers
{
    public class Int32MessageConsumer : IConsumer<Int32Message>
    {
        public Task ConsumeAsync(Int32Message message, CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }
    }
}
