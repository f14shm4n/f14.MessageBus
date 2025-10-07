using Microsoft.Extensions.DependencyInjection;

namespace f14.MessageBus
{
    public interface IConsumerInvokerFabric
    {
        ConsumerInvoker GetInvoker(IServiceScope scope, Type consumerType, Type messageType);
    }
}
